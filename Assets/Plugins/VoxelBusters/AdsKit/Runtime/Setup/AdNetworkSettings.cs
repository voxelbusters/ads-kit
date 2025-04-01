using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelBusters.CoreLibrary;

namespace VoxelBusters.AdsKit
{
    [Serializable]
    public partial class AdNetworkSettings : IValidator
    {
        #region Fields

        [SerializeField, HideInInspector]
        private     string                      m_networkId;

        [SerializeField]
        private     bool                        m_isEnabled;

        [SerializeField]
        private     string                      m_runtimeApiKey;

        [SerializeField]
        private     RuntimePlatformConstantSet  m_runtimeApiKeyOverrides;

        [SerializeField]
        private     string                      m_runtimeApiSecret;

        [SerializeField]
        private     RuntimePlatformConstantSet  m_runtimeApiSecretOverrides;

        [SerializeField]
        private     string                      m_debugApiKey;

        [SerializeField]
        private     string                      m_debugApiSecret;

        [SerializeField]
        private     string                      m_data;

        [SerializeField]
        private     AdMeta[]                    m_adMetaArray;

        #endregion

        #region Properties

        public string NetworkId
        {
            get => m_networkId;
            private set => m_networkId  = value;
        }

        public bool IsEnabled
        {
            get => m_isEnabled;
            private set => m_isEnabled  = value;
        }

        public string RuntimeApiKey
        {
            get => m_runtimeApiKey;
            private set => m_runtimeApiKey  = value;
        }

        public RuntimePlatformConstantSet RuntimeApiKeyOverrides
        {
            get => m_runtimeApiKeyOverrides;
            private set => m_runtimeApiKeyOverrides = value;
        }

        public string RuntimeApiSecret
        {
            get => m_runtimeApiSecret;
            private set => m_runtimeApiSecret   = value;
        }

        public RuntimePlatformConstantSet RuntimeApiSecretOverrides
        {
            get => m_runtimeApiSecretOverrides;
            private set => m_runtimeApiSecretOverrides  = value;
        }

        public string DebugApiKey
        {
            get => m_debugApiKey;
            private set => m_debugApiKey    = value;
        }

        public string DebugApiSecret
        {
            get => m_debugApiSecret;
            private set => m_debugApiSecret = value;
        }

        public string Data
        {
            get => m_data;
            private set => m_data   = value;
        }

        public AdMeta[] AdMetaArray
        {
            get => m_adMetaArray;
            private set => m_adMetaArray    = value;
        }

        #endregion

        #region Constructors

        public AdNetworkSettings(string networkId, bool isEnabled,
                                 string runtimeApiKey = null, RuntimePlatformConstantSet runtimeApiKeyOverrides = null,
                                 string runtimeApiSecret = null, RuntimePlatformConstantSet runtimeApiSecretOverrides = null,
                                 string debugApiKey = null, string debugApiSecret = null,
                                 string data = null, AdMeta[] adMetaArray = null)
        {
            Assert.IsNotNullOrEmpty(networkId, nameof(networkId));
            
            // Set default properties
            NetworkId                   = networkId;
            IsEnabled                   = isEnabled;
            RuntimeApiKey               = runtimeApiKey;
            RuntimeApiKeyOverrides      = runtimeApiKeyOverrides;
            RuntimeApiSecret            = runtimeApiSecret;
            RuntimeApiSecretOverrides   = runtimeApiSecretOverrides;
            DebugApiKey                 = debugApiKey;
            DebugApiSecret              = debugApiSecret;
            Data                        = PropertyHelper.GetValueOrDefault(data, null);
            AdMetaArray                 = adMetaArray;
        }

        #endregion

        #region Public methods

        public void GetApiKeysForPlatform(RuntimePlatform platform, bool isDebugMode,
                                          out string apiKey, out string apiSecret)
        {
            if (isDebugMode && !string.IsNullOrEmpty(DebugApiKey))
            {
                apiKey      = DebugApiKey;
                apiSecret   = DebugApiSecret;
            }
            else
            {
                apiKey      = RuntimeApiKeyOverrides.GetConstantForPlatform(platform, m_runtimeApiKey);
                apiSecret   = RuntimeApiSecretOverrides.GetConstantForPlatform(platform, m_runtimeApiSecret);
            }
        }

        #endregion

        #region IValidator implementation

        public ValidationResult Validate()
        {
            return string.IsNullOrEmpty(NetworkId)
                ? ValidationResult.CreateError(description: "Invalid network id.")
                : ValidationResult.Success;
        }

        #endregion
    }
}