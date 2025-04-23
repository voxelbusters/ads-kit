using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelBusters.CoreLibrary;
using VoxelBusters.CoreLibrary.NativePlugins;

namespace VoxelBusters.AdsKit
{
    public partial class AdsKitSettings : SettingsObject
    {
        #region Static fields

        private     static      AdsKitSettings          s_sharedInstance;

        private     static      UnityPackageDefinition  s_package;

        #endregion

        #region Fields

        [SerializeField]
        private     bool                    m_isEnabled;

        [SerializeField]
        private     bool                    m_isDebugBuild;

        [SerializeField]
        [Tooltip("When auto load is enabled for an ad placement, this value (in seconds) determines the wait time before attempting another load request after a failure.")]
        private     int                     m_autoLoadRetryDelay = 15;

        [SerializeField]
        private     bool                    m_autoInitOnStart;

        [SerializeField]
        private     LoadAdMode              m_loadAdMode;

        [SerializeField]
        private     AdNetworkSettings[]     m_networkSettingsArray;

        [SerializeField]
        private     AdNetworkPreferenceMeta m_networkPreferenceMeta;

        [SerializeField]
        private     AdPlacementMeta[]       m_placementMetaArray;

        [SerializeField]
        private     AdContentSettings       m_adContentDefaultSettings;

        [SerializeField]
        private     DeviceCollection        m_testDevices;

        [SerializeField]
        private     List<string>            m_errorLogList = new List<string>();

        #endregion

        #region Static properties

        public static UnityPackageDefinition Package
        {
            get
            {
                if (s_package == null)
                {
                    s_package   = new UnityPackageDefinition(
                        name: "com.voxelbusters.adskit",
                        displayName: "Ads Kit",
                        version: "2.0.2",
                        defaultInstallPath: $"Assets/Plugins/VoxelBusters/AdsKit");
                }
                return s_package;
            }
        }

        public static string PackageName => Package.Name;

        public static string DisplayName => Package.DisplayName;

        public static string Version => Package.Version;

        public static string DefaultSettingsAssetName => "AdsKitSettings";

        public static string DefaultSettingsAssetPath => $"{Package.GetMutableResourcesPath()}/{DefaultSettingsAssetName}.asset";

        public static string DefaultBannerAdOptionsAssetPath => $"{Package.GetMutableResourcesPath()}/DefaultBannerAdOptions.asset";

        public static string PersistentDataPath => Package.PersistentDataPath;

        public static AdsKitSettings Instance => GetSharedInstanceInternal();

        public static string Domain => "VoxelBusters.AdsKit";

        #endregion

        #region Properties

        public bool IsEnabled
        {
            get => m_isEnabled;
            private set => m_isEnabled = value;
        }

        public bool AutoInitOnStart
        {
            get => m_autoInitOnStart;
            private set => m_autoInitOnStart    = value;
        }

        public bool IsDebugBuild
        {
            get => m_isDebugBuild;
            internal set => m_isDebugBuild  = value;
        }

        public int AutoLoadRetryDelay
        {
            get => m_autoLoadRetryDelay;
            internal set => m_autoLoadRetryDelay = value;
        }

        public LoadAdMode LoadAdMode
        {
            get => m_loadAdMode;
            internal set => m_loadAdMode    = value;
        }

        public AdNetworkSettings[] NetworkSettingsArray
        {
            get => m_networkSettingsArray;
            internal set => m_networkSettingsArray = value;
        }

        public AdNetworkPreferenceMeta NetworkPreferenceMeta
        {
            get => m_networkPreferenceMeta;
            internal set => m_networkPreferenceMeta = value;
        }

        public AdPlacementMeta[] PlacementMetaArray
        {
            get => m_placementMetaArray;
            internal set => m_placementMetaArray    = value;
        }

        internal AdContentSettings AdContentDefaultSettings
        {
            get => m_adContentDefaultSettings;
            set => m_adContentDefaultSettings  = value;
        }

        public DeviceCollection TestDevices
        {
            get => m_testDevices;
            internal set => m_testDevices  = value;
        }

        #endregion

        #region Static methods

        public static AdsKitSettings Create(bool isEnabled = true,
                                            bool isDebugBuild = false,
                                            bool autoInitOnStart = false,
                                            LoadAdMode loadAdMode = LoadAdMode.Sequential,
                                            AdPlacementMeta[] placementMetaArray = null,
                                            AdNetworkPreferenceMeta networkPreferenceMeta = null,
                                            AdContentSettings adContentDefaultSettings = null,
                                            DeviceCollection deviceCollection = null,
                                            params AdNetworkSettings[] networkSettingsArray)
        {
            var     newInstance                 = CreateInstance<AdsKitSettings>();
            newInstance.IsEnabled               = isEnabled;
            newInstance.IsDebugBuild            = isDebugBuild;
            newInstance.AutoInitOnStart         = autoInitOnStart;
            newInstance.LoadAdMode              = loadAdMode;
            newInstance.NetworkPreferenceMeta   = networkPreferenceMeta ?? new AdNetworkPreferenceMeta();
            newInstance.PlacementMetaArray      = placementMetaArray ?? new AdPlacementMeta[0];
            newInstance.AdContentDefaultSettings= adContentDefaultSettings ?? new AdContentSettings();
            newInstance.NetworkSettingsArray    = networkSettingsArray ?? new AdNetworkSettings[0];
            newInstance.TestDevices             = deviceCollection ?? new DeviceCollection();

            return newInstance;
        }

        private static AdsKitSettings GetSharedInstanceInternal(bool throwError = true)
        {
            if (null == s_sharedInstance)
            {
                // Check whether we are accessing in edit or play mode
                var     assetPath   = DefaultSettingsAssetName;
                var     settings    = Resources.Load<AdsKitSettings>(assetPath);
                if (throwError && (null == settings))
                {
                    throw Diagnostics.PluginNotConfiguredException();
                }

                // Store reference
                s_sharedInstance    = settings;
            }

            return s_sharedInstance;
        }

#endregion

#region Public methods

        public AdContentOptions GetDefaultSettings(AdType adType)
        {
            if (adType == AdType.Banner)
            {
                return (BannerAdOptions)m_adContentDefaultSettings.Banner;
            }
            return null;
        }

        public T GetDefaultSettings<T>(AdType adType) where T : AdContentOptions
        {
            var     options = GetDefaultSettings(adType);
            if (options is T)
            {
                return (T)options;
            }
            return null;
        }

        public bool HasAdNetworkEnabled(string adNetworkId)
        {
            foreach (var eachAdNetwork in NetworkSettingsArray)
            {
                if (eachAdNetwork.NetworkId.Equals(adNetworkId))
                    return true;
            }

            return false;
        }

        #endregion

#region Overriden methods

#if UNITY_EDITOR

        protected override void OnValidate()
        {
            base.OnValidate();
            MakeErrorLog();
        }

        private void MakeErrorLog()
        {
            m_errorLogList.Clear();

            if (HasAnyInvalidPlacements())
            {
                m_errorLogList.Add($"Found invalid placements. Set Ad Type and give a unique name in the text field of the placement in 'Ad Placements' section of 'General Tab'.");
            }

            // Case for not listing any Ad networks in the preferred array for a specific ad type with in a placement.
            foreach (var placement in m_placementMetaArray)
            {
                //There should be atleast one preferred network for each ad type
                if (placement.AdType != default(AdType) && !m_networkPreferenceMeta.HasAnyPreferredNetworksConfigured(placement.AdType))
                {
                    var error = $"There are no preferred Ad networks added for '{placement.AdType}' ad type. Make sure you add atleast one network under 'Ad Network Preferences' section of 'General' tab.";
                    if (!HasAnyAdNetworksEnabled())
                    {
                        error += "\n Note: You don't have any Ad Network services enabled yet. You first need to enable an Ad Network from 'Services' tab.";
                    }
                    m_errorLogList.Add(error);
                }
            }
        }

        private bool HasAnyInvalidPlacements()
        {
            bool anyInvalidPlacements = false;

            foreach (var eachPlacement in m_placementMetaArray)
            {
                if(string.IsNullOrEmpty(eachPlacement.Name) || eachPlacement.AdType == default(AdType))
                {
                    anyInvalidPlacements = true;
                    break;
                }
            }

            return anyInvalidPlacements;
        }

        private bool HasAnyAdNetworksEnabled()
        {
            bool anyEnabled = false;

            foreach (var each in m_networkSettingsArray)
            {
                if (each.IsEnabled)
                {
                    anyEnabled = true;
                    break;
                }
            }

            return anyEnabled;
        }

#endif

#endregion
    }
}