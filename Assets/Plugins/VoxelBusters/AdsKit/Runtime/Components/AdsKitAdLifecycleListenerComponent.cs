using UnityEngine;
using UnityEngine.Events;
using VoxelBusters.CoreLibrary;

namespace VoxelBusters.AdsKit
{
    [AddComponentMenu("Voxel Busters/Ads Kit/Ad Lifecycle")]
    [DisallowMultipleComponent]
    public class AdsKitAdLifecycleListenerComponent : MonoBehaviour, IAdLifecycleEventListener
    {
        #region Events
        [Space]
        [Header("Initialisation Events")]
        [Space]
        [SerializeField]
        private UnityEvent<InitialiseResult> m_onInitialisationComplete;

        [SerializeField]
        private UnityEvent<Error> m_onInitialisationFail;

        [Space]
        [Header("Load Ad Events")]
        [SerializeField]
        private UnityEvent<string, LoadAdResult> m_onLoadAdComplete;

        [SerializeField]
        private UnityEvent<string, Error> m_onLoadAdFail;


        [Space]
        [Header("Show Ad Events")]
        [SerializeField]
        private UnityEvent<string> m_onShowAdStart;

        [SerializeField]
        private UnityEvent<string> m_onShowAdClick;

        [SerializeField]
        private UnityEvent<string, ShowAdResult> m_onShowAdComplete;

        [SerializeField]
        private UnityEvent<string, AdReward> m_onAdReward;

        [SerializeField]
        private UnityEvent<string, Error> m_onShowAdFail;


        [Space]
        [Header("Other Events")]
        
        [SerializeField]
        private UnityEvent<string> m_onAdImpressionRecorded;


        [SerializeField]
        private UnityEvent<string, AdTransaction> m_onAdPaid;

        #endregion

        #region Unity methods

        private void OnEnable()
        {
            AdsManager.RegisterListener(this);
        }

        private void OnDisable()
        {
            AdsManager.UnregisterListener(this);
        }

        #endregion

        #region IAdLifecycleEventListener implementation
        public int CallbackOrder => 0;

        public void OnInitialisationComplete(InitialiseResult result)
        {
            m_onInitialisationComplete?.Invoke(result);
        }

        public void OnInitialisationFail(Error error)
        {
            m_onInitialisationFail?.Invoke(error);
        }

        public void OnLoadAdComplete(string placement, LoadAdResult result)
        {
            m_onLoadAdComplete?.Invoke(placement, result);
        }

        public void OnLoadAdFail(string placement, Error error)
        {
            m_onLoadAdFail?.Invoke(placement, error);
        }

        public void OnShowAdStart(string placement)
        {
            m_onShowAdStart?.Invoke(placement);
        }

        public void OnShowAdClick(string placement)
        {
            m_onShowAdClick?.Invoke(placement);
        }

        public void OnShowAdComplete(string placement, ShowAdResult result)
        {
            m_onShowAdComplete?.Invoke(placement, result);
        }

        public void OnShowAdFail(string placement, Error error)
        {
            m_onShowAdFail?.Invoke(placement, error);
        }

        public void OnAdImpressionRecorded(string placement)
        {
            m_onAdImpressionRecorded?.Invoke(placement);
        }

        public void OnAdPaid(string placement, AdTransaction transaction)
        {
            m_onAdPaid?.Invoke(placement, transaction);
        }

        public void OnAdReward(string placement, AdReward reward)
        {
            m_onAdReward?.Invoke(placement, reward);
        }

        #endregion
    }
}