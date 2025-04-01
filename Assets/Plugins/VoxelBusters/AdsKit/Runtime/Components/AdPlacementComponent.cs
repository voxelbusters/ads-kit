using System.Collections;
using UnityEngine;
using VoxelBusters.CoreLibrary;

namespace VoxelBusters.AdsKit
{
    internal class AdPlacementComponent : ActionTriggerComponent
    {
        #region Fields

        [SerializeField, AdPlacement]
        private     string              m_placement;

        #endregion

        #region Public methods

        public void ShowAd()
        {
            StartCoroutine(ShowAdInternal());
        }

        public override void ExecuteAction()
        {
            ShowAd();
        }

        #endregion

        #region Private methods

        private IEnumerator ShowAdInternal()
        {
            if (string.IsNullOrEmpty(m_placement))
            {
                DebugLogger.LogWarning(AdsKitSettings.Domain, $"Placement is null or empty.");
                yield break;
            }

            // Wait for Ads Kit to get initialised
            if(!AdsManager.IsInitialised)
            {
                var operation = InitialiseWithConsentFormProvider();
                operation.OnComplete += (operation) =>
                {
                    if (operation.Error == null)
                    {
                        FinishShowingAd(m_placement);
                    }
                    else
                    {
                        DebugLogger.LogError(AdsKitSettings.Domain, $"Failed to initialise Ads Kit with : {operation.Error}");
                    }
                };

            }
            else
            {
                FinishShowingAd(m_placement);
            }
        }

        private void FinishShowingAd(string placement)
        {
            IsDone = true;
            AdsManager.ShowAd(placement);
        }

        private IAsyncOperation<InitialiseResult> InitialiseWithConsentFormProvider()
        {

            var consentFormProvider = AdServices.GetConsentFormProvider();
            Assert.IsNotNull(consentFormProvider, "There are no consent form provider implementations available.Either implement IConsentFormProvider or enable AdMob Ad Network for Google's UMP service.");

            return AdsManager.Initialise(consentFormProvider);
        }

        #endregion
    }
}