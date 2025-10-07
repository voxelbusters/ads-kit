using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using VoxelBusters.AdsKit.Adapters;
using VoxelBusters.CoreLibrary;

namespace VoxelBusters.AdsKit
{
    public enum LoadAdMode
    {
        Sequential = 1,

        Concurrent
    }

    internal abstract class LoadAdOperation : AsyncOperation<LoadAdResult>
    {
        #region Fields

        private     List<AdNetworkLoadAdStateInfo>  m_cachedStates;

        private     AdContentOptions                m_contentOptions;

        private     bool                            m_initiatedLoad;

        #endregion

        #region Properties

        public AdType AdType { get; private set; }

        public string Placement { get; private set; }

        public AdNetworkAdapter[] AdNetworks { get; private set; }

        public AdContentOptions ContentOptions
        {
            get => m_contentOptions;
            set
            {
                //@@Assert.IsPropertyNotNull(value, nameof(value)); - This is optional property

                if (m_contentOptions == null)
                {
                    m_contentOptions    = value;
                }
            }
        }

        internal AdsManagerImpl Manager { get; private set; }

        protected int AvailableAdNetworksCount => AdNetworks != null ? AdNetworks.Length : 0;

        protected int CachedStateCount => m_cachedStates.Count;

        #endregion

        #region Constructors

        public LoadAdOperation(string placement,
                               AdsManagerImpl manager)
        {
            // Set properties
            Placement       = placement;
            Manager         = manager;
            m_initiatedLoad = false;
        }

        #endregion

        #region Abstract methods

        protected abstract void PerformLoad();

        protected abstract void DidReceiveLoadAdResponse(AdNetworkAdapter adNetwork, AdNetworkLoadAdStateInfo stateInfo);

        #endregion

        #region Base class methods

        protected override void OnStart()
        {
            TryPerformLoad();
        }

        protected override void OnUpdate()
        {
            TryPerformLoad();
        }

        #endregion

        #region Private methods

        protected bool CanAcceptEvent(string placement) => string.Equals(Placement, placement);

        protected AdNetworkLoadAdStateInfo FindCachedStateWithAdNetworkId(string networkId)
        {
            return m_cachedStates.Find((item) => string.Equals(networkId, item.NetworkId));
        }

        protected AdNetworkLoadAdStateInfo GetCachedStateAt(int index)
        {
            return m_cachedStates[index];
        }

        private bool TryPerformLoad()
        {
            // Ensure load is initiated only once during operations lifecycle.
            if (m_initiatedLoad) return false;

            // Wait until manager is ready
            if (!Manager.IsInitialised) return false;

            // Start load operation
            m_initiatedLoad = true;
            PrepareForLoad();
            PerformLoad();
            return true;
        }

        private void PrepareForLoad()
        {
            // Check whether pre-requisite conditions to run ad system is available
            if (!Manager.PreprocessAdRequest(out Error error))
            {
                SetFailedWithError(error);
                return;
            }
            var     placementMeta   = Manager.GetAdPlacementMetaWithPlacement(placement: Placement);
            if (placementMeta == null)
            {
                DebugLogger.LogError(AdsKitSettings.Domain, $"No details found for placement id: {Placement}. Configure it in AdsKit settings -> Ad Placements.");
                SetFailedWithError(AdError.ConfigurationNotFound("No details found for the given placement."));
                return;
            }
            var     adNetworks      = Manager.GetPreferredAdNetworks(placementMeta.AdType);
            if (adNetworks.Length == 0)
            {
                DebugLogger.LogError(AdsKitSettings.Domain, $"No ad networks configured for placement id: {placementMeta.Name} of ad type: {placementMeta.AdType}.");
                SetFailedWithError(AdError.ConfigurationNotFound("No ad networks configured for the given placement."));
                return;
            }

            // Cache information
            AdType          = placementMeta.AdType;
            AdNetworks      = adNetworks;
            m_cachedStates  = new List<AdNetworkLoadAdStateInfo>(capacity: adNetworks.Length);
            if ((m_contentOptions == null) || !m_contentOptions.IsCompatibleWithAdType(AdType))
            {
                m_contentOptions    = placementMeta.ContentOptions ?? Manager.GetDefaultAdContentOptions(adType: AdType);
            }
        }

        protected void StartLoadAdOn(AdNetworkAdapter adNetwork)
        {
            // Check whether we are requesting for compatible type
            if (!adNetwork.HasPlacement(Placement))
            {
                var     stateInfo   = new AdNetworkLoadAdStateInfo(adUnitId: null,
                                                                   adType: 0,
                                                                   networkId: adNetwork.NetworkId,
                                                                   placement: Placement,
                                                                   placementState: AdPlacementState.NotAvailable,
                                                                   error: AdError.ConfigurationNotFound("No details found for the given placement."));

                DebugLogger.LogError(AdsKitSettings.Domain, $"{Placement} (placement id) and Ad Unit id mapping not found in {adNetwork.Name} settings. Open AdKitSettings to Configure in services section of {adNetwork.Name}");

                HandleOnLoadAdStateChange(adNetwork: adNetwork,
                                          stateInfo: stateInfo);
                return;
            }

            // Register for callbacks
            adNetwork.OnLoadAdStateChange   += HandleOnLoadAdStateChange;

            // Make request
            adNetwork.LoadAd(Placement, m_contentOptions);
        }

        protected void UpdateCachedStateInfo(AdNetworkAdapter adNetwork, AdNetworkLoadAdStateInfo newState)
        {
            var     cachedState         = FindCachedStateWithAdNetworkId(adNetwork.NetworkId);
            if (cachedState == null)
            {
                var     stateCopy       = new AdNetworkLoadAdStateInfo(adUnitId: newState.AdUnitId,
                                                                       adType: newState.AdType,
                                                                       networkId: newState.NetworkId,
                                                                       placement: newState.Placement,
                                                                       placementState: newState.PlacementState,
                                                                       error: newState.Error);
                m_cachedStates.Add(stateCopy);
            }
            else
            {
                cachedState.PlacementState  = newState.PlacementState;
                cachedState.Error           = newState.Error;
            }
        }

        protected void ProcessLoadAdData()
        {
            var     preferredProvider   = m_cachedStates.Find((item) => item.PlacementState == AdPlacementState.Ready);
            if (preferredProvider != null)
            {
                var     result          = new LoadAdResult(adUnitId: preferredProvider.AdUnitId,
                                                           adType: preferredProvider.AdType,
                                                           placement: Placement,
                                                           placementState: AdPlacementState.Ready,
                                                           loadStateArray: m_cachedStates.ToArray(),
                                                           preferredAdProvider: preferredProvider.NetworkId);
                SetIsCompleted(result);
            }
            else
            {
                var     lastAdState     = m_cachedStates.Last();
                var     error           = AdError.LoadFail(lastAdState.Error?.Description);
                SetIsCompleted(error);
            }
        }

        private void SetFailedWithError(Error error)
        {
            SetIsCompleted(error);
        }

        #endregion

        #region Event handler methods

        private void HandleOnLoadAdStateChange(AdNetworkAdapter adNetwork, AdNetworkLoadAdStateInfo stateInfo)
        {
            // Check whether callback is about the requested placement id
            if (!CanAcceptEvent(stateInfo.Placement)) return;

            // Unregister for events
            adNetwork.OnLoadAdStateChange   -= HandleOnLoadAdStateChange;

            // Forward result to the handler function
            DidReceiveLoadAdResponse(adNetwork, stateInfo);
        }

        #endregion
    }

    internal class SequentialLoadAdOperation : LoadAdOperation
    {
        #region Fields

        private     int     m_currentNetworkIndex;

        #endregion

        #region Constructors

        public SequentialLoadAdOperation(string placement,
                                         AdsManagerImpl manager)
            : base(placement, manager)
        {
            // Set properties
            m_currentNetworkIndex   = 0;
        }

        #endregion

        #region Base class implementation

        protected override void PerformLoad()
        {
            if (NextAdNetwork(out AdNetworkAdapter adNetwork))
            {
                StartLoadAdOn(adNetwork);
            }
        }

        protected override void DidReceiveLoadAdResponse(AdNetworkAdapter adNetwork, AdNetworkLoadAdStateInfo stateInfo)
        {
            DebugLogger.Log(AdsKitSettings.Domain, $"Received LoadAd Response from {adNetwork.Name}. Placement: {stateInfo.Placement}. State: {stateInfo.PlacementState}. Error: {stateInfo.Error}.");
            UpdateCachedStateInfo(adNetwork, stateInfo);

            // Check whether any operation is completed
            if ((stateInfo.PlacementState == AdPlacementState.Ready) || !NextAdNetwork(out AdNetworkAdapter nextNetwork))
            {
                ProcessLoadAdData();
            }
            else
            {
                StartLoadAdOn(nextNetwork);
            }
        }

        #endregion

        #region Private methods

        private bool NextAdNetwork(out AdNetworkAdapter adNetwork)
        {
            // Set default reference values
            adNetwork   = null;

            int     adNetworkCount  = AvailableAdNetworksCount;
            while (m_currentNetworkIndex < adNetworkCount)
            {
                var     current     = AdNetworks[m_currentNetworkIndex++];
                if (current != null)
                {
                    adNetwork       = current;
                    return true;
                }
            }
            return false;
        }

        #endregion
    }

    internal class ConcurrentLoadAdOperation : LoadAdOperation
    {
        #region Constructors

        public ConcurrentLoadAdOperation(string placement,
                                         AdsManagerImpl manager)
            : base(placement, manager)
        { }

        #endregion

        #region Base class implementation

        protected override void PerformLoad()
        {
            // Initiate load request on all the specified networks
            foreach (var current in AdNetworks)
            {
                if (current == null) continue;

                StartLoadAdOn(current);
            }
        }

        protected override void DidReceiveLoadAdResponse(AdNetworkAdapter adNetwork, AdNetworkLoadAdStateInfo stateInfo)
        {
            UpdateCachedStateInfo(adNetwork, stateInfo);

            if (AreAllRequestsCompleted()) //TODO: Ideally we need to pick the network which responds first to the request and then proceed - Cross check this
            {
                ProcessLoadAdData();
            }
        }

        #endregion

        #region Private methods

        private bool AreAllRequestsCompleted()
        {
            int     trackedStateCount   = CachedStateCount;
            if (trackedStateCount != AvailableAdNetworksCount) return false;

            for (int iter = 0; iter < trackedStateCount; iter++)
            {
                var     current         = GetCachedStateAt(iter);
                if ((current.Error == null) && (current.PlacementState == AdPlacementState.Unknown))  return false;
            }
            return true;
        }

        #endregion
    }
}