using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VoxelBusters.AdsKit.Adapters;
using VoxelBusters.CoreLibrary;
using VoxelBusters.CoreLibrary.NativePlugins.DemoKit;

namespace VoxelBusters.AdsKit.Demo
{
	public class AdsApiDemo : DemoActionPanelBase<AdsApiDemoAction, AdsApiDemoActionType>, IAdLifecycleEventListener
    {
        #region Fields

        [SerializeField]
        private     InputField      m_placementIdField;

        [SerializeField]
        private     Dropdown        m_consentDropdown;

        [SerializeField]
        private     GameObject      m_selectConsentOptionGO;

        [SerializeField]
        private     GameObject      m_requestConsentGO;

        #endregion

        #region Base class methods

        protected override void Start() 
        {
            base.Start();
        }

        protected override void OnActionSelectInternal(AdsApiDemoAction selectedAction)
        {
            switch (selectedAction.ActionType)
            {
                case AdsApiDemoActionType.RegisterListener:
                    Log("Register listener.");
                    RegisterListener();
                    break;
                    
                case AdsApiDemoActionType.UnregisterListener:
                    Log("Unregister listener.");
                    UnregisterListener();
                    break;

                case AdsApiDemoActionType.ConsentFormProviderAvailable:
                    Log("Checking if any consent form provider available.");
                    CheckIfConsentFormProviderIsAvailable();
                    break;

                case AdsApiDemoActionType.Initialise:
                    Log("Init with consent form provider.");
                    var consentFormProvider = GetConsentFormProvider();
                    //consentFormProvider.IsAgeRestrictedUser = false; //Enable if you want to set this property.
                    InitAdsManager(consentFormProvider);
                    break;

                case AdsApiDemoActionType.LoadAd:
                    if (!AssertPropertyIsValid("placementId", m_placementIdField.text)) return;

                    Log("Initiating LoadAd request.");
                    LoadAd(m_placementIdField.text);
                    break;

                case AdsApiDemoActionType.ShowAd:
                    if (!AssertPropertyIsValid("placementId", m_placementIdField.text)) return;

                    Log("Initiating ShowAd request.");
                    ShowAd(m_placementIdField.text);
                    break;

                case AdsApiDemoActionType.HideAd:
                    if (!AssertPropertyIsValid("placementId", m_placementIdField.text)) return;

                    Log("Initiating HideAd request.");
                    HideAd(m_placementIdField.text);
                    break;

                case AdsApiDemoActionType.AdPlacementState:
                    if (!AssertPropertyIsValid("placementId", m_placementIdField.text)) return;

                    AdPlacementState state = AdsManager.GetAdPlacementState(placement: m_placementIdField.text);
                    Log("Ad placement state: " + state);
                    break;
            }
        }

        private void CheckIfConsentFormProviderIsAvailable()
        {
            var consentFormProvider = AdServices.GetConsentFormProvider();
            if (consentFormProvider == null)
            {
                Log("No consent form providers available. Create a class which implements IConsentFormProvider interface or enable AdMob ad network for Google's UMP consent provider.");
            }
            else
            {
                Log("IConsentFormProvider implementation exists. You can initilise AdsKit with this consent form provider.");
            }
        }

        #endregion

        #region Example methods

        private void RegisterListener()
        {
            AdsManager.RegisterListener(this);
        }

        private void UnregisterListener()
        {
            AdsManager.UnregisterListener(this);
        }

        private void InitAdsManager(IConsentFormProvider consentFormProvider)
        {
            if(AdsManager.IsInitialisedOrWillChange)
            {
                Log("Initialisation is in progress or already initialised.");
                return;
            }
            
            var operation = AdsManager.Initialise(consentFormProvider);
            operation.OnComplete += (op) =>
            {
                if (op.Error == null)
                {
                    Log("Initialise complete. You can start loading or showing the ads from now.");
                }
                else
                {
                    Log("Failed initialising Ads Kit with: " + op.Error);
                }
            };
        }
        
        private void LoadAd(string placement)
        {
            AdsManager.LoadAd(placement);
        }

        private void ShowAd(string placement)
        {
            var operation = AdsManager.ShowAd(placement);
            operation.OnAdReward += ((AdContent content, AdReward reward) result) =>
            {
                //Note that OnAdReward can be called way before ad presentation finishes. So if you want to load an ad, don't do it in OnAdReward callback. Instead do in OnAdFinish callback.
                Debug.Log($"Ad reward received: {result.reward}");
            };
        }

        private void HideAd(string placement)
        {
            AdsManager.HideAd(placement);
        }

        #endregion

        #region Utility methods

        private IConsentFormProvider GetConsentFormProvider()
        {
            var consentFormProvider = AdServices.GetConsentFormProvider();

            if(consentFormProvider == null)
            {
                Log("There are no IConsentFormProvider implementations available. For a default plugin's implementation, enable Ad Mob network or implement a custom IConsentFormProvider on your own");
            }

            return consentFormProvider;
        }

        #endregion

        #region IAdLifecycleEventListener implementation

        public void OnInitialisationComplete(InitialiseResult result)
        {
            Log("AdsKit is initialised successfully.");
        }

        public void OnInitialisationFail(Error error)
        {
            Log($"AdsKit failed to initialise with error {error}.");
        }

        public void OnLoadAdComplete(string placementId, LoadAdResult result)
        {
            Log($"AdsKit has successfully loaded ad for placementId: {placementId}.");
        }

        public void OnLoadAdFail(string placementId, Error error)
        {
            Log($"AdsKit has failed to load ad for placementId: {placementId} with error: {error}.");
        }

        public void OnShowAdStart(string placementId)
        {
            Log($"AdsKit has started showing ad for placementId: {placementId}.");
        }

        public void OnShowAdClick(string placementId)
        {
            Log($"AdsKit has recognised click on ad for placementId: {placementId}.");
        }

        public void OnShowAdComplete(string placementId, ShowAdResult result)
        {
            Log($"AdsKit has completed showing ad for placementId: {placementId} with result: {result}.");
        }

        public void OnShowAdFail(string placementId, Error error)
        {
            Log($"AdsKit has failed to show ad for placementId: {placementId} with Error: {error}.");
        }

        public void OnAdImpressionRecorded(string placementId)
        {
            Log($"AdsKit has recorded impression for ad for placementId: {placementId}.");
        }

        public void OnAdPaid(string placementId, AdTransaction transaction)
        {
            Log($"AdsKit has recorded pay for ad: {placementId} with transaction: {transaction}.");
        }

        public void OnAdReward(string placementId, AdReward reward)
        {
            if (reward.IsRewardAmountAvailable())
            {
                Log($"AdsKit has recorded reward for ad: {placementId} with reward info: {reward}.");
            }
            else //Some adnetworks won't support passing reward amounts in the callbacks. For those handle with a fallback value.
            {
                Log($"AdsKit has recorded reward for ad: {placementId} with reward unknown reward amount. Reward the user with a fallback value");
            }
        }

        #endregion

        #region IEventHandler implementation

        public int CallbackOrder => 1;

        #endregion

        #region Utility methods

        private void OnDestroy()
        {
            UnregisterListener();
        }

        #endregion
    }
}