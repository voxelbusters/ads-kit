using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelBusters.CoreLibrary;

namespace VoxelBusters.AdsKit.Adapters
{
    public class AdNetworkInitialiseProperties
    {
        #region Properties

        public string ApiKey { get; private set; }

        public string ApiSecret { get; private set; }

        public ApplicationPrivacyConfiguration PrivacyConfiguration { get; private set; }

        public bool IsTestMode { get; private set; }

        public string Data { get; private set; }


        #endregion

        #region Constructors

        public AdNetworkInitialiseProperties(string apiKey, string apiSecret, ApplicationPrivacyConfiguration privacyConfiguration,
            bool isTestMode, string data = null)
        {
            // Set properties
            ApiKey                  = apiKey;
            ApiSecret               = apiSecret;
            PrivacyConfiguration    = privacyConfiguration;
            IsTestMode           = isTestMode;
            Data                    = data;
        }

        #endregion
    }
}