using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using VoxelBusters.CoreLibrary;

namespace VoxelBusters.AdsKit
{
    [AddComponentMenu("Voxel Busters/Ads Kit/Initialise")]
    [DisallowMultipleComponent]
    public class AdsKitInitialiseComponent : ActionTriggerComponent
    {
        #region Events
        [Header("Events")]
        [Space]
        [SerializeField]
        private UnityEvent          m_onInitialise;

        [SerializeField]
        private UnityEvent<Error>   m_onError;

        #endregion


        #region Public methods

        public void Initialise()
        {
            var consentFormProvider = AdServices.GetConsentFormProvider();
            Assert.IsNotNull(consentFormProvider, "There are no consent form provider implementations available.Either implement IConsentFormProvider or enable AdMob Ad Network for Google's UMP service.");

            InitialiseInternal(consentFormProvider);
        }

        public override void ExecuteAction()
        {
            Initialise();
        }

        #endregion

        #region Private methods

        private void InitialiseInternal(IConsentFormProvider consentFormProvider)
        {
            var operation = AdsManager.Initialise(consentFormProvider);
            operation.OnComplete += (op) =>
            {
                if (op.Error != null)
                {
                    DebugLogger.LogError("Failed initialising with: " + op.Error);
                    m_onError?.Invoke(op.Error);
                }
                else
                {
                    m_onInitialise?.Invoke();
                }
            };
        }

        #endregion
    }
}