using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using VoxelBusters.AdsKit.Adapters;
using VoxelBusters.CoreLibrary;

namespace VoxelBusters.AdsKit
{
    public class InitialiseAfterConsentOperation : AsyncOperation<InitialiseResult>
    {
        #region Fields

        private IConsentFormProvider m_consentFormProvider;
        private Func<ApplicationPrivacyConfiguration, IAsyncOperation<InitialiseResult>> m_initialiseFunc;


        #endregion

        #region Constructors

        internal InitialiseAfterConsentOperation(IConsentFormProvider consentFormProvider, Func<ApplicationPrivacyConfiguration, IAsyncOperation<InitialiseResult>> initialiseFunc)
        {
            Assert.IsArgNotNull(consentFormProvider, nameof(consentFormProvider));

            m_consentFormProvider   = consentFormProvider;
            m_initialiseFunc        = initialiseFunc;
        }

        #endregion

        #region Base class methods

        protected override void OnStart()
        {
            m_consentFormProvider.ShowConsentForm(callback: (result, error) =>
            {
                if (error == null)
                {
                    DebugLogger.Log(AdsKitSettings.Domain, $"Successfully got consent status: {result.UsageConsent}");
                    var operation = m_initialiseFunc(result);
                    operation.OnComplete += (operation) =>
                    {
                        if (operation.Error == null)
                        {
                            SetIsCompleted(operation.Result);
                        }
                        else
                        {
                            SetIsCompleted(operation.Error);
                        }
                    };
                }
                else
                {
                    DebugLogger.LogError(AdsKitSettings.Domain, $"Failed to get consent status with error: {error}.");
                    SetIsCompleted(error);
                }
            });
        }

        #endregion
    }
}