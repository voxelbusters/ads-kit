using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelBusters.AdsKit.Adapters;
using VoxelBusters.CoreLibrary;

namespace VoxelBusters.AdsKit
{
    /// <summary>
    /// The ad content base class type.
    /// </summary>
    [IncludeInDocs]
    public abstract class AdContent : AsyncOperation<ShowAdResult>
    {
        #region Fields

        private     LoadAdRequest           m_loadRequest;

        protected   ContentStatus           m_status;

        private     bool                    m_showOnLoad;

        #endregion

        #region Properties

        public string AdUnitId { get; private set; }

        public AdType AdType { get; private set; }

        public string Placement { get; private set; }

        public string ProviderId => Provider?.NetworkId;

        public bool IsReady => (Provider != null) && !IsDone;

        public bool IsShowing => m_status == ContentStatus.Showing;

        public bool IsShowingOrWillShow => IsShowing || (m_status == ContentStatus.WillShow);

        public ShowAdStateInfo CurrentState { get; protected set; }

        internal AdNetworkAdapter Provider { get; private set; }

        internal AdsManagerImpl Manager { get; private set; }

        #endregion

        #region Events

        public event Callback<AdContent> OnAdStart;

        public event Callback<AdContent> OnAdClick;

        public event Callback<AdContent> OnAdFinish;

        public event Callback<(AdContent content, AdReward reward)> OnAdReward;

        public event Callback<AdContent> OnAdFail;

        #endregion

        #region Constructors

        public AdContent(AdType adType,
                         string placement,
                         AdsManagerImpl manager = null,
                         AdNetworkAdapter provider = null)
        {
            Assert.IsArgNotNull(placement, nameof(placement));
            Assert.IsArgNotNull(manager, nameof(manager));

            // Set properties
            AdType              = adType;
            Placement           = placement;
            Manager             = manager;
            Provider            = provider;
            SetDefaultProperties();
        }

        #endregion

        #region Base class methods

        protected override void OnStart()
        {
            base.OnStart();

            ShowInSafeMode();
        }

        protected override void OnEnd()
        {
            Cleanup();
        }

        #endregion

        #region Public methods

        public LoadAdRequest LoadAd()
        {
            if (!CanMakeLoadRequest())
            {
                DebugLogger.LogError("The operation could not be completed because you cannot create new requests.");
                return null;
            }

            // Create load request
            m_loadRequest   = GetLoadRequest(create: true);
            SetContentStatus(value: ContentStatus.Waiting);

            return m_loadRequest;
        }

        public void ShowAd(bool forcefully = false)
        {
            if (Status == AsyncOperationStatus.NotStarted)
            {
                Start();
                return;
            }

            if (!forcefully && (IsDone || IsShowingOrWillShow)) return;

            // Check whether we need to set default configuration
            if (CanMakeLoadRequest())
            {
                LoadAd();
            }

            // Check whether we can present the ad
            if ((m_loadRequest != null) && !m_loadRequest.IsDone)
            {
                SetContentStatus(value: ContentStatus.Waiting);
                m_showOnLoad    = true;
                return;
            }
            if ((m_loadRequest == null) || (m_loadRequest.Status ==  AsyncOperationStatus.Succeeded))
            {
                EnsureDefaultProviderIsSet();
                SetContentStatus(value: ContentStatus.WillShow);
                CurrentState    = new ShowAdStateInfo(adUnitId: AdUnitId,
                                                      adType: AdType,
                                                      placement: Placement,
                                                      networkId: Provider.NetworkId,
                                                      state: 0);
                UnregisterEvents();
                RegisterForEvents();
                Provider.ShowAd(Placement);
                return;
            }
        }

        public void DestroyAd()
        {
            if (!IsDone)
            {
                SetIsCompleted(AdError.Unknown());
                return;
            }
        }

        #endregion

        #region Private methods

        protected bool CanAcceptEvent(string placement) => string.Equals(Placement, placement);

        private void RegisterForEvents()
        {
            Provider.OnShowAdStateChange    += HandleShowAdStateChange;
            Provider.OnAdImpressionRecorded += HandleAdImpressionRecorded;
            Provider.OnAdPaid               += HandleAdPaid;
            Provider.OnAdReward             += HandleAdReward;
        }

        private void UnregisterEvents()
        {
            Provider.OnShowAdStateChange    -= HandleShowAdStateChange;
            Provider.OnAdImpressionRecorded -= HandleAdImpressionRecorded;
            Provider.OnAdPaid               -= HandleAdPaid;
            Provider.OnAdReward             -= HandleAdReward;
        }

        private bool CanMakeLoadRequest() => (m_loadRequest == null) && (Provider == null);

        private LoadAdRequest GetLoadRequest(bool create = true)
        {
            if (Manager == null)
            {
                Manager     = AdsManager.SharedInstance;
            }

            // Check whether we have already requesting for ad with specified placement id
            var     activeRequest   = Manager.GetLoadAdRequest(placement: Placement);
            if ((activeRequest == null) && create)
            {
                activeRequest       = Manager.CreateLoadAdRequest(placement: Placement,
                                                                  options: BuildContentOptions());
            }
            if (activeRequest != null)
            {
                activeRequest.OnComplete   += HandleLoadAdResult;
            }
            return activeRequest;
        }

        protected virtual AdContentOptions BuildContentOptions() => null;

        private void SetDefaultProperties()
        {
            m_status        = 0;
            m_showOnLoad    = false;
        }

        protected void SetContentStatus(ContentStatus value)
        {
            m_status        = value;
        }

        private void EnsureDefaultProviderIsSet()
        {
            if (Provider == null)
            {
                Provider    = Manager.SelectAdNetwork(AdType);
            }
        }

        private bool ShowInSafeMode()
        {
            if (IsShowing) return false;

            ShowAd();
            return true;
        }

        private void CopyStateProperties(AdNetworkShowAdStateInfo source, ShowAdStateInfo target)
        {
            target.State        = source.State;
            target.Error        = source.Error;
            if (source.State == ShowAdState.Finished)
            {
                target.Result   = new ShowAdResult(adUnitId: source.AdUnitId,
                                                   adType: source.AdType,
                                                   placement: source.Placement,
                                                   networkId: source.NetworkId,
                                                   clicked: source.Clicked,
                                                   resultCode: source.ResultCode.GetValueOrDefault(),
                                                   watchDuration: source.WatchDuration.GetValueOrDefault());
            }
        }

        private void Cleanup()
        {
            // Reset state
            if (Provider != null)
            {
                UnregisterEvents();
            }
        }

        #endregion

        #region Event handler methods

        private void HandleLoadAdResult(IAsyncOperationHandle<LoadAdResult> operationHandle)
        {
            if (operationHandle.Error == null)
            {
                var     result  = operationHandle.Result;

                // Update properties
                Manager         = (operationHandle as LoadAdRequest).InternalOp.Manager;
                Provider        = Manager.GetAdNetworkWithId(result.PreferredAdProvider);

                if (m_showOnLoad)
                {
                    ShowInSafeMode();
                }
            }
            else
            {
                SetIsCompleted(operationHandle.Error);
            }
        }

        private void HandleShowAdStateChange(AdNetworkAdapter adNetwork,
                                             AdNetworkShowAdStateInfo stateInfo)
        {
            if (!CanAcceptEvent(stateInfo.Placement)) return;

            // Update state properties
            if (ShowAdState.Started == stateInfo.State)
            {
                SetContentStatus(ContentStatus.Showing);
            }
            else if (ShowAdState.Finished == stateInfo.State)
            {
                SetContentStatus(ContentStatus.Done);

                var     result  = new ShowAdResult(adUnitId: stateInfo.AdUnitId,
                                                   adType: stateInfo.AdType,
                                                   placement: stateInfo.Placement,
                                                   networkId: stateInfo.NetworkId,
                                                   clicked: stateInfo.Clicked,
                                                   resultCode: stateInfo.ResultCode.GetValueOrDefault(),
                                                   watchDuration: stateInfo.WatchDuration);
                SetIsCompleted(result);
            }
            else if (ShowAdState.Failed == stateInfo.State)
            {
                SetContentStatus(ContentStatus.Done);
                SetIsCompleted(stateInfo.Error);
            }
            CopyStateProperties(source: stateInfo, target: CurrentState);
            SendStateChangeEvent(CurrentState);
        }

        private void HandleAdImpressionRecorded(AdNetworkAdapter adNetwork,
                                                AdNetworkAdImpressionInfo impressionInfo)
        {
            Manager?.OnAdImpressionRecorded(sender: this, impressionInfo: impressionInfo);
        }

        private void HandleAdPaid(AdNetworkAdapter adNetwork,
                                  AdTransaction transaction)
        {
            Manager?.OnAdPaid(sender: this, transaction: transaction);
        }

        private void HandleAdReward(AdNetworkAdapter adNetwork,
                                  AdReward reward)
        {
            if (!CanAcceptEvent(reward.Placement)) return;

            OnAdReward?.Invoke((this, reward));
            Manager?.OnAdReward(sender: this, reward: reward);
        }
        

        private void SendStateChangeEvent(ShowAdStateInfo stateInfo)
        {
            switch (stateInfo.State)
            {
                case ShowAdState.Started:
                    OnAdStart?.Invoke(this);
                    Manager?.OnShowAdStart(sender: this, stateInfo: stateInfo);
                    break;

                case ShowAdState.Clicked:
                    OnAdClick?.Invoke(this);
                    Manager?.OnShowAdClick(sender: this, stateInfo: stateInfo);
                    break;

                case ShowAdState.Finished:
                    OnAdFinish?.Invoke(this);
                    Manager?.OnShowAdComplete(sender: this, stateInfo: stateInfo);
                    break;

                case ShowAdState.Failed:
                    OnAdFail?.Invoke(this);
                    Manager?.OnShowAdFail(sender: this, stateInfo: stateInfo);
                    break;

                default:
                    throw VBException.SwitchCaseNotImplemented(stateInfo.State);
            }
        }

        #endregion

        #region Nested types

        protected enum ContentStatus
        {
            Waiting = 1,

            WillShow,

            Showing,

            Hidden,

            Done
        }

        #endregion
    }
}