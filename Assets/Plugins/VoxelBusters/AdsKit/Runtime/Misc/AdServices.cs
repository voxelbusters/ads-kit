using System;
using System.Collections;
using System.Collections.Generic;
using VoxelBusters.AdsKit.Adapters;
using VoxelBusters.CoreLibrary;

namespace VoxelBusters.AdsKit
{
    public static class AdServices
    {
        #region Static fields

        [ClearOnReload]
        private     static      IConsentFormProvider[]      s_cachedFormProviders;

        #endregion

        #region Static methods

        public static void ShowConsentForm(IConsentFormProvider provider, bool forceShow = false,
                                           CompletionCallback<ApplicationPrivacyConfiguration> callback = null)
        {
            Assert.IsArgNotNull(provider, nameof(provider));

            provider.ShowConsentForm(forceShow: forceShow, callback: callback);
        }

        public static IConsentFormProvider[] FindConsentFormProviders()
        {
            if (s_cachedFormProviders == null)
            {
                var     baseInterfaceType   = typeof(IConsentFormProvider);
                var     providerTypes       = ReflectionUtility.FindAllTypes(predicate: (type) => type.IsClass && !type.IsAbstract && baseInterfaceType.IsAssignableFrom(type));
                s_cachedFormProviders       = Array.ConvertAll(providerTypes, (type) => ReflectionUtility.CreateInstance(type, nonPublic: true) as IConsentFormProvider);
            }
            return s_cachedFormProviders;
        }

        public static void ResetConsentInformation()
        {
            var     providers   = FindConsentFormProviders();
            foreach (var provider  in providers)
            {
                provider.ResetConsentInformation();
            }
        }

        public static IConsentFormProvider GetConsentFormProvider()
        {
            var allAvailableProviders = FindConsentFormProviders();
            if (allAvailableProviders.Length == 0)
            {
                return null;
            }

            // Select provider with highest priority value
            Array.Sort(allAvailableProviders, (a, b) => a.Priority.CompareTo(b.Priority));

            return allAvailableProviders[0];
        }

        #endregion
    }
}