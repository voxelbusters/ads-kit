using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using VoxelBusters.CoreLibrary;

namespace VoxelBusters.AdsKit
{
    [AddComponentMenu("Voxel Busters/Ads Kit/Load Ad")]
    [DisallowMultipleComponent]
    public class AdsKitLoadAdComponent : ActionTriggerComponent
    {
        #region Fields

        [SerializeField, AdPlacement]
        private     string              m_placement;


        [SerializeField]
        [Tooltip("Set this for additionally configuring an Ad. Options are currently available of Banner Ad Type ads only.")]
        private AdContentOptionsAsset   m_adContentOptionsAsset;

        #endregion

        #region Events
        [Space]
        [Header("Events")]
        [Space]
        [SerializeField]
        private UnityEvent<string> m_onLoad;

        [SerializeField]
        private UnityEvent<string, Error> m_onError;

        #endregion

        #region Public methods

        public void LoadAd()
        {
            LoadAdInternal();
        }

        public override void ExecuteAction()
        {
            LoadAd();
        }

        #endregion

        #region Private methods

        private void LoadAdInternal()
        {
            if (string.IsNullOrEmpty(m_placement))
            {
                SendErrorEvent(new Error($"Placement is null or empty."));
                return;
            }

            if (!AdsManager.IsInitialised)
            {
                SendErrorEvent(new Error($"Ads Kit not initialised. Either initialise with AdsKitIniitialiseComponent or AdsKitManager.Initialise"));
                return;
            }

            IsDone = true;
            var state = AdsManager.GetAdPlacementState(placement: m_placement);

            if(state == AdPlacementState.Ready)
            {
                m_onLoad?.Invoke(m_placement);
            }
            else
            {
                var operation = AdsManager.LoadAd(m_placement, m_adContentOptionsAsset?.GetRawData());
                operation.OnComplete += (op) =>
                {
                    if(op.Error == null)
                    {
                        m_onLoad?.Invoke(m_placement);
                    }
                    else
                    {
                        SendErrorEvent(op.Error);
                    }
                };
            }
        }

        private void SendErrorEvent(Error error)
        {
            DebugLogger.LogError(AdsKitSettings.Domain, error.Description);
            m_onError?.Invoke(m_placement, error);
        }

        #endregion
    }
}