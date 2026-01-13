using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelBusters.CoreLibrary;

namespace VoxelBusters.AdsKit
{
    public static class AdError
    {
        #region Constants

        public const   string   kDomainName          = "AdError";

        #endregion

        #region Static properties

        public static Error InitializationError(string message = null)
        {
            return CreateError(code: AdErrorCode.kNotInitialized,
                description: message ?? "Ad network is not initialized.");  
        } 

        public static Error ConsentNotAvailable(string message = null)
        {
            return CreateError(code: AdErrorCode.kConsentNotAvailable,
                description: message ?? "Consent is missing.");
        }

        public static Error NoInternet(string message = null)
        {
            return CreateError(code: AdErrorCode.kNoInternet,
                description: message ?? "Internet connection not found.");
        }

        public static Error LoadFail(string message = null)
        {
            return CreateError(code: AdErrorCode.kLoadFail,
                description: message ?? "Load failed.");
        }

        public static Error ShowFail(string message = null)
        {
            return CreateError(code: AdErrorCode.kShowFail,
                description: message ?? "Show error.");
        }

        public static Error UserCancelled(string message = null)
        {
            return CreateError(code: AdErrorCode.kUserCancelled,
                description: message ?? "User cancelled.");
        }

        public static Error Unknown(string message = null)
        {
            return CreateError(code: AdErrorCode.kUnknown,
                description: message ?? "Unknown error.");
        }

        public static Error NotSupported(string message = null)
        {
            return CreateError(code: AdErrorCode.kNotSupported,
                description: message ?? "Not supported.");
        }

        public static Error ConfigurationNotFound(string message = null)
        {
            return CreateError(code: AdErrorCode.kConfigurationNotFound,
                description: message ?? "Configuration not found.");
        }

        public static Error NotTestMode(string message = null)
        {
            return CreateError(code: AdErrorCode.kConfigurationNotFound,
                description: message ?? "Test mode configuration is not enabled.");
        }

        public static Error CreateError(int code, string description) => new Error(domain: kDomainName,
                                                                                   code: code,
                                                                                   description: description);

        #endregion
    }
}
