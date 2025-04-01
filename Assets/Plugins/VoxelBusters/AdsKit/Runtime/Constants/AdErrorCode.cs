using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelBusters.CoreLibrary;

namespace VoxelBusters.AdsKit
{
    /// <summary>
    /// Error codes.
    /// </summary>
    [IncludeInDocs]
    public static class AdErrorCode
    {
        /// <summary> Error code indicating that an unknown or unexpected error occurred. </summary>
        public  const   int     kUnknown                = 0;

        /// <summary> Error code indicating that system failed to initialize. </summary>
        public  const   int     kNotInitialized         = 1;

        /// <summary> Error code indicating that system doesn't have required permissions. </summary>
        public  const   int     kConsentNotAvailable    = 2;

        /// <summary> Error code indicating that internet connection is not available. </summary>
        public  const   int     kNoInternet             = 3;
        
        /// <summary> Error code indicating that system failed to load ad content. </summary>
        public  const   int     kLoadFail               = 4;

        /// <summary> Error code indicating that system failed to show ad content. </summary>
        public  const   int     kShowFail               = 5;

        /// <summary> Error code indicating that system failed to show ad content. </summary>
        public  const   int     kUserCancelled          = 6;

        /// <summary> Error code indicating that system failed to show ad content. </summary>
        public  const   int     kNotSupported           = 7;

        /// <summary> Error code indicating that system failed to show ad content. </summary>
        public  const   int     kConfigurationNotFound  = 8;
    }
}