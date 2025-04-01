using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using VoxelBusters.CoreLibrary;

namespace VoxelBusters.AdsKit
{
    [AddComponentMenu("Voxel Busters/Ads Kit/Hide Ad(works for Banners only)")]
    [DisallowMultipleComponent]
    public class AdsKitHideAdComponent : ActionTriggerComponent
    {
        #region Fields

        [SerializeField, AdPlacement]
        private     string              m_placement;

        [SerializeField]
        private bool m_destroy;

        #endregion

        #region Events
        [Space]
        [Header("Events")]
        [Space]
        [SerializeField]
        private UnityEvent<string> m_onError;

        #endregion


        #region Public methods

        public void HideAd()
        {
            HideAdInternal();
        }

        public override void ExecuteAction()
        {
            HideAd();
        }

        #endregion

        #region Private methods

        private void HideAdInternal()
        {
            if (string.IsNullOrEmpty(m_placement))
            {
                SendErrorEvent($"Placement is null or empty.");
                return;
            }

            if (!AdsManager.IsInitialised)
            {
                SendErrorEvent($"Ads Kit not initialised. Either initialise with AdsKitIniitialiseComponent or AdsKitManager.Initialise");
                return;
            }

            IsDone = true;
            AdsManager.HideAd(m_placement, destroy: m_destroy);
        }

        private void SendErrorEvent(string errorDescription)
        {
            DebugLogger.LogError(AdsKitSettings.Domain, errorDescription);
            m_onError?.Invoke(errorDescription);
        }

        #endregion
    }
}