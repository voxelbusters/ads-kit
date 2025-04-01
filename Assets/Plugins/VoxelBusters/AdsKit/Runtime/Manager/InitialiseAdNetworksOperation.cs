using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using VoxelBusters.AdsKit.Adapters;
using VoxelBusters.CoreLibrary;

namespace VoxelBusters.AdsKit
{
    public class InitialiseAdNetworksOperation : AsyncOperation<InitialiseResult>
    {
        #region Fields

        private     AdNetworkSettings[]             m_networkSettingsArray;

        private     bool                            m_isDebugBuild;

        private     AdNetworkRuntimeImplMeta[]      m_runtimeImplMetaArray;

        private     AdNetworkAdapter[]              m_adNetworks;

        private     AdNetworkInitialiseStateInfo[]  m_stateArray;

        private     ApplicationPrivacyConfiguration m_privacyConfiguration;

        #endregion

        #region Properties

        internal AdsManagerImpl Manager { get; private set; }

        #endregion

        #region Constructors

        internal InitialiseAdNetworksOperation(AdsManagerImpl manager,
                                     AdNetworkSettings[] networkSettingsArray,
                                     ApplicationPrivacyConfiguration privacyConfiguration,
                                     bool isDebugBuild
                                     )
        {
            Assert.IsNotNullOrEmpty(networkSettingsArray, "Enable atleast one ad network in AdsKit Settings -> Services. Ad network settings array");

            // Set properties
            Manager                     = manager;
            m_networkSettingsArray      = networkSettingsArray;
            m_isDebugBuild              = isDebugBuild;
            m_runtimeImplMetaArray      = Helpers.FindAllRuntimeImplMeta();
            m_privacyConfiguration      = privacyConfiguration;
            m_stateArray                = null;
        }

        #endregion

        #region Static methods

        private static AdNetworkAdapter CreateClientInstance(string name, Type type)
        {
            var     gameObject  = new GameObject(name);
            return gameObject.AddComponent(type) as AdNetworkAdapter;
        }

        #endregion

        #region Base class methods

        protected override void OnStart()
        {
            // Find all the attribute definitions related to the Ads network
            var     instanceList    = new List<AdNetworkAdapter>();
            var     stateList       = new List<AdNetworkInitialiseStateInfo>();
            foreach (var networkSettings in m_networkSettingsArray)
            {
                if (!Helpers.TryFindRuntimeImpl(m_runtimeImplMetaArray, networkSettings.NetworkId, out AdNetworkRuntimeImplMeta meta)) continue;

                // Create instance
                var     newInstance     = (meta.CreateMethodInfo != null)
                    ? meta.CreateMethodInfo.Invoke(null, null) as AdNetworkAdapter
                    : CreateClientInstance(name: meta.Name, type: meta.ImplementationType);
                if (newInstance == null) continue;

                // Set parent of the created adapter to be the manager
                newInstance.transform.parent = Manager.transform;

                // Check whether this adapter is supported in current platform
                if (!newInstance.IsSupported)
                {
                    newInstance.gameObject.SetActive(false);
                    GameObject.Destroy(newInstance.gameObject);
                    continue;
                }

                // Set properties
                newInstance.AdMetaArray = networkSettings.AdMetaArray;
                newInstance.SetInternalProperties(name: meta.Name,
                                                  networkId: networkSettings.NetworkId,
                                                  manager: Manager);

                // Mark that specified network is compatible with the SDK
                var     instanceState   = new AdNetworkInitialiseStateInfo(networkId: newInstance.NetworkId);
                instanceList.Add(newInstance);
                stateList.Add(instanceState);
            }
            m_adNetworks        = instanceList.ToArray();
            m_stateArray        = stateList.ToArray();

            // Initiate request on all networks
            foreach (var state in m_stateArray)
            {
                var     instance        = Helpers.FindAdNetwork(m_adNetworks, state.NetworkId);
                var     networkSettings = FindSettings(state.NetworkId);
                networkSettings.GetApiKeysForPlatform(ApplicationServices.GetActiveOrSimulationPlatform(), m_isDebugBuild, out string apiKey, out string apiSecret);

                var     initProperties  = new AdNetworkInitialiseProperties(apiKey,
                                                                            apiSecret,
                                                                            m_privacyConfiguration,
                                                                            m_isDebugBuild,
                                                                            networkSettings.Data);
                instance.OnInitialiseComplete   += HandleOnInitialiseStateChange;
                instance.Initialise(initProperties);
            }
        }

        protected override void OnEnd()
        {
            // Cleanup properties
            UnregisterFromEvents();
        }

        #endregion

        #region Private methods

        private AdNetworkSettings FindSettings(string networkId)
        {
            return Array.Find(m_networkSettingsArray, (item) => string.Equals(item.NetworkId, networkId));
        }

        private AdNetworkInitialiseStateInfo FindState(string networkId)
        {
            return Array.Find(m_stateArray, (item) => string.Equals(item.NetworkId, networkId));
        }

        private void UnregisterFromEvents()
        {
            foreach (var state in m_stateArray)
            {
                var     instance                = Helpers.FindAdNetwork(m_adNetworks, state.NetworkId);
                instance.OnInitialiseComplete   -= HandleOnInitialiseStateChange;
            }
        }

        private bool TryUpdatingInternalState()
        {
            // Check whether all the requests are completed
            foreach (var state in m_stateArray)
            {
                if ((state.Status == 0) && (state.Error == null)) return false;
            }

            // Cleanup failed to register networks
            var     subscribedNetworkList  = new List<AdNetworkAdapter>();
            foreach (var state in m_stateArray)
            {
                var     adNetwork           = Helpers.FindAdNetwork(m_adNetworks, state.NetworkId);
                if (state.Status == AdNetworkInitialiseStatus.Success)
                {
                    subscribedNetworkList.Add(adNetwork);
                }
                else
                {
                    adNetwork.gameObject.SetActive(false);
                }
            }

            if (m_stateArray.Length > subscribedNetworkList.Count)
            {
                DebugLogger.LogError($"Not all ad networks initialised successfully. Double check their settings in AdsKitSettings.");

                foreach (var state in m_stateArray)
                {
                    if (state.Status == AdNetworkInitialiseStatus.Fail)
                    {
                        DebugLogger.LogError($"AdNetwork : {state.NetworkId} failed to initialise with error: {state.Error}");    
                    }
                }
            }

            // Gather required properties
            if (subscribedNetworkList.Count > 0)
            {
                var     subscribedNetworks  = subscribedNetworkList.ToArray();
                var     invalidAdNetworks   = CollectionUtility.ConvertAll(source: m_networkSettingsArray,
                                                                           converter: (item) => item.NetworkId,
                                                                           match: (item) => Helpers.FindAdNetwork(subscribedNetworks, item.NetworkId) == null);
                var     result              = new InitialiseResult(subscribedAdNetworks: subscribedNetworks,
                                                                   invalidAdNetworks: invalidAdNetworks);
                SetIsCompleted(result);
            }
            else
            {
                var     error   = AdError.InitializationError("Failed to register any ad networks.");
                SetIsCompleted(error);
            }
            return true;
        }

        #endregion

        #region Event handling methods

        private void HandleOnInitialiseStateChange(AdNetworkAdapter adNetwork, AdNetworkInitialiseStateInfo stateInfo)
        {
            var     targetState     = FindState(adNetwork.NetworkId);
            if (targetState == null)
            {
                DebugLogger.LogError($"Could not find state for Ad-network: {adNetwork.NetworkId}.");
                return;
            }

            // Update state information
            targetState.Status      = stateInfo.Status;
            targetState.Error       = stateInfo.Error;

            TryUpdatingInternalState();
        }

        #endregion
    }
}