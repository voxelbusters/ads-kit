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

        public static Error InitializationError(string message = "Ad network is not initialized.")
        {
            return CreateError(code: AdErrorCode.kNotInitialized,
                description: message);  
        } 

        public static Error ConsentNotAvailable { get; private set; } = CreateError(code: AdErrorCode.kConsentNotAvailable,
                                                                                    description: "Consent is missing.");

        public static Error NoInternet { get; private set; } = CreateError(code: AdErrorCode.kNoInternet,
                                                                           description: "Internet connection not found.");

        public static Error LoadFail { get; private set; } = CreateError(code: AdErrorCode.kLoadFail,
                                                                         description: "Load failed.");

        public static Error ShowFail { get; private set; } = CreateError(code: AdErrorCode.kShowFail,
                                                                         description: "Show error.");

        public static Error UserCancelled { get; private set; } = CreateError(code: AdErrorCode.kUserCancelled,
                                                                              description: "User cancelled.");

        public static Error Unknown { get; private set; } = CreateError(code: AdErrorCode.kUnknown,
                                                                        description: "Unknown error.");

        public static Error NotSupported { get; private set; } = CreateError(code: AdErrorCode.kNotSupported,
                                                                             description: "Not supported.");

        public static Error ConfigurationNotFound { get; private set; } = CreateError(code: AdErrorCode.kConfigurationNotFound,
                                                                             description: "Configuration not found.");
        #endregion

        #region Static methods

        public static Error CreateError(int code, string description) => new Error(domain: kDomainName,
                                                                                   code: code,
                                                                                   description: description);

        #endregion
    }
}