using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelBusters.CoreLibrary;

namespace VoxelBusters.AdsKit.Adapters
{    
    public abstract class AdNetworkAdapter : MonoBehaviour
    {
        #region Fields

        private     AdMeta[]                                m_adMetaArray;

        private     Dictionary<string, bool>                m_adClickData;

        private     Dictionary<string, AdContentOptions>    m_adOptionsData;
        
        protected   bool                                    m_isTestMode;

        #endregion

        #region Properties

        public AdsManagerImpl Manager { get; private set; }

        public string NetworkId { get; private set; }

        public string Name { get; private set; }

        public AdMeta[] AdMetaArray
        {
            get => m_adMetaArray;
            set
            {
                Assert.IsPropertyNotNull(value, nameof(value));

                // Set new value
                m_adMetaArray   = value;
            }
        }

        public abstract bool IsInitialised { get; }

        public abstract bool IsSupported { get; }

        public bool ForceSendEventOnMainThread { get; protected set; } = true;

        #endregion

        #region Delegates

        public delegate void InitialiseStateChangeDelegate(AdNetworkAdapter adNetwork, AdNetworkInitialiseStateInfo stateInfo);

        public delegate void LoadAdStateChangeDelegate(AdNetworkAdapter adNetwork, AdNetworkLoadAdStateInfo stateInfo);

        public delegate void ShowAdStateChangeDelegate(AdNetworkAdapter adNetwork, AdNetworkShowAdStateInfo stateInfo);

        public delegate void AdPaidDelegate(AdNetworkAdapter adNetwork, AdTransaction transaction);

        public delegate void AdImpressionRecordedDelegate(AdNetworkAdapter adNetwork, AdNetworkAdImpressionInfo impressionInfo);

        public delegate void AdRewardDelegate(AdNetworkAdapter adNetwork, AdReward rewardInfo);

        #endregion

        #region Events

        public event InitialiseStateChangeDelegate OnInitialiseComplete;

        public event LoadAdStateChangeDelegate OnLoadAdStateChange;

        public event ShowAdStateChangeDelegate OnShowAdStateChange;

        public event AdPaidDelegate OnAdPaid;

        public event AdImpressionRecordedDelegate OnAdImpressionRecorded;

        public event AdRewardDelegate   OnAdReward;

        #endregion

        #region Unity methods

        protected virtual void OnDestroy()
        { }

        #endregion

        #region Init methods

        public abstract void Initialise(AdNetworkInitialiseProperties properties);

        #endregion

        #region Testing methods

        public abstract void LaunchAdDebugger(EventCallback<bool> callback = null);

        #endregion

        #region Ad methods

        public abstract AdPlacementState GetPlacementState(string placement);

        public abstract void LoadAd(string placement, AdContentOptions options = null);

        public abstract void ShowAd(string placement);

        public abstract void HideBanner(string placement, bool destroy = false);

        #endregion

        #region Setter methods

        public abstract void SetPaused(bool pauseStatus);

        public abstract void SetPrivacyConfiguration(ApplicationPrivacyConfiguration config);

        public abstract void SetUser(User user);

        public abstract void SetUserSettings(UserSettings settings);

        public abstract void SetOrientation(ScreenOrientation orientation);

        #endregion

        #region Getter methods

        public AdPlacementMeta GetAdPlacementMeta(string placement)
        {
            return Manager.GetAdPlacementMetaWithPlacement(placement);
        }

        public AdPlacementMeta GetAdPlacementMetaWithAdUnitId(string adUnitId)
        {
            var     adMeta  = GetAdMetaWithAdUnitId(adUnitId);
            return (adMeta != null)
                ? Manager.GetAdPlacementMetaWithPlacement(adMeta.Placement)
                : null;
        }

        public AdMeta GetAdMetaWithPlacement(string placement)
        {
            return System.Array.Find(AdMetaArray, (item) => string.Equals(placement, item.Placement));
        }

        public AdMeta GetAdMetaWithAdUnitId(string adUnitId, RuntimePlatform platform)
        {
            return System.Array.Find(AdMetaArray, (item) => string.Equals(adUnitId, item.GetAdUnitIdForPlatform(platform, m_isTestMode)));
        }

        public AdMeta GetAdMetaWithAdUnitId(string adUnitId)
        {
            return System.Array.Find(AdMetaArray, (item) => string.Equals(adUnitId, item.GetAdUnitIdForActiveOrSimulationPlatform(m_isTestMode)));
        }

        public AdViewProxy GetAdViewProxy(string placement)
        {
            var adMeta = GetAdMetaWithPlacement(placement);
            string adUnitId = adMeta.GetAdUnitIdForActiveOrSimulationPlatform(m_isTestMode);
            var placementMeta = GetAdPlacementMeta(placement);

            return GetAdViewProxy(placementMeta.AdType, adUnitId);
        }

        #endregion

        #region Private methods

        protected bool IsAdClicked(string adUnitId)
        {
            return (m_adClickData != null) && m_adClickData.TryGetValue(adUnitId, out bool value) && value;
        }

        protected void MarkAdClicked(string adUnitId)
        {
            UpdateAdClickData(adUnitId, true);
        }

        protected void ClearAdClicked(string adUnitId)
        {
            UpdateAdClickData(adUnitId, false);
        }

        private void UpdateAdClickData(string adUnitId, bool value)
        {
            if (m_adClickData == null)
            {
                m_adClickData       = new Dictionary<string, bool>(capacity: 8);
            }
            m_adClickData[adUnitId] = value;
        }

        /*protected void CacheContentOption(string adUnitId, AdContentOptions options)
        {
            if (options == null) return;

            if (m_adOptionsData == null)
            {
                m_adOptionsData = new Dictionary<string, AdContentOptions>(capacity: 8);
            }
            m_adOptionsData[adUnitId] = options;
        }

        protected T GetContentOptions<T>(string adUnitId, T defaultValue = default(T)) where T : AdContentOptions
        {
            if (m_adOptionsData.TryGetValue(adUnitId, out AdContentOptions target) && target is T)
            {
                return target as T;
            }
            return defaultValue;
        }*/

        protected virtual AdViewProxy GetAdViewProxy(AdType adType, string adUnitId) => null;

        protected bool IsConfigValidForAdDebuggerUsage()
        {
            if (!m_isTestMode)
            {
                DebugLogger.LogWarning($"{AdsKitSettings.Domain}:{Name}", "Ad Debugger can only be used in test mode (Development build mode or enabling Force Test Mode in Ads Kit Settings.)");
                return false;
            }
            return true;
        } 

        #endregion

        #region Internal methods

        internal void SetInternalProperties(string name,
                                            string networkId,
                                            AdsManagerImpl manager)
        {
            if (NetworkId != null) return;

            Assert.IsArgNotNull(networkId, nameof(name));
            Assert.IsArgNotNull(networkId, nameof(networkId));
            Assert.IsArgNotNull(manager, nameof(manager));

            // Set property values
            Name = name;
            NetworkId = networkId;
            Manager = manager;
        }

        internal bool HasPlacement(string placement)
        {
            var     adMeta  = GetAdMetaWithPlacement(placement);
            return (adMeta != null);
        }

        #endregion

        #region Report events methods

        protected void SendInitaliseStateChangeEvent(AdNetworkInitialiseStateInfo stateInfo)
        {
            Callback    eventFunc   = () =>
            {
                var message = $"Sending Initalise state change data. State: {stateInfo.Status}.";
                if (stateInfo.Status == AdNetworkInitialiseStatus.Success)
                    DebugLogger.Log($"{AdsKitSettings.Domain}:{Name}", message);
                else
                    DebugLogger.LogWarning($"{AdsKitSettings.Domain}:{Name}", message);

                OnInitialiseComplete?.Invoke(this, stateInfo);
            };
            if (ForceSendEventOnMainThread)
            {
                CallbackDispatcher.InvokeOnMainThread(eventFunc);
            }
            else
            {
                eventFunc();
            }
        }

        protected void SendLoadAdStateChangeEvent(AdNetworkLoadAdStateInfo stateInfo)
        {
            Callback    eventFunc   = () =>
            {
                DebugLogger.Log($"{AdsKitSettings.Domain}:{Name}", $"Sending LoadAd state change data. Placement: {stateInfo.Placement}. State: {stateInfo.PlacementState}.");

                OnLoadAdStateChange?.Invoke(this, stateInfo);
            };
            if (ForceSendEventOnMainThread)
            {
                CallbackDispatcher.InvokeOnMainThread(eventFunc);
            }
            else
            {
                eventFunc();
            }
        }

        protected void SendShowAdStateChangeEvent(AdNetworkShowAdStateInfo stateInfo)
        {
            Callback    eventFunc   = () =>
            {
                DebugLogger.Log($"{AdsKitSettings.Domain}:{Name}", $"Sending ShowAd state change data. Placement: {stateInfo.Placement}. State: {stateInfo.State}. ");

                OnShowAdStateChange?.Invoke(this, stateInfo);
            };
            if (ForceSendEventOnMainThread)
            {
                CallbackDispatcher.InvokeOnMainThread(eventFunc);
            }
            else
            {
                eventFunc();
            }
        }

        protected void SendAdPaidEvent(AdTransaction transaction)
        {
            Callback    eventFunc   = () =>
            {
                DebugLogger.Log($"{AdsKitSettings.Domain}:{Name}", $"Sending AdPaid data. Placement: {transaction.Placement}. Payment: {transaction}. ");

                OnAdPaid?.Invoke(this, transaction);
            };
            if (ForceSendEventOnMainThread)
            {
                CallbackDispatcher.InvokeOnMainThread(eventFunc);
            }
            else
            {
                eventFunc();
            }
        }

        protected void SendAdImpressionRecordedEvent(AdNetworkAdImpressionInfo impressionInfo)
        {
            Callback    eventFunc   = () =>
            {
                DebugLogger.Log($"{AdsKitSettings.Domain}:{Name}", $"Sending AdRecordedImpression data. Placement: {impressionInfo.Placement}. Impression: {impressionInfo}. ");

                OnAdImpressionRecorded?.Invoke(this, impressionInfo);
            };
            if (ForceSendEventOnMainThread)
            {
                CallbackDispatcher.InvokeOnMainThread(eventFunc);
            }
            else
            {
                eventFunc();
            }
        }

        protected void SendAdRewardEvent(AdReward reward)
        {
            //Caching this event as on some ad networks, this is not gaurenteed to be called in order (can be on or before ad closed event). As we are triggerring event on main thread, by the time its dispatched, OnAdReward can be cleared in UnregisterEvents during Cleanup call.
            var cachedCallback = OnAdReward;
            Callback    eventFunc   = () =>
            {
                DebugLogger.Log($"{AdsKitSettings.Domain}:{Name}", $"Sending AdReward data. Placement: {reward.Placement}. Reward: {reward}. ");

                cachedCallback?.Invoke(this, reward);
            };
            if (ForceSendEventOnMainThread)
            {
                CallbackDispatcher.InvokeOnMainThread(eventFunc);
            }
            else
            {
                eventFunc();
            }
        }

        #endregion
    }
}