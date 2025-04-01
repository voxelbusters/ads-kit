using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using VoxelBusters.CoreLibrary;
using VoxelBusters.CoreLibrary.NativePlugins;
using VoxelBusters.CoreLibrary.Editor;

namespace VoxelBusters.AdsKit.Editor
{
    [InitializeOnLoad]
    public static class AdsKitSettingsEditorUtility
    {
        #region Constants

        private     const       string              kLocalPathInProjectSettings     = "Project/Voxel Busters/Ads Kit";

        #endregion

        #region Static fields

        private     static      AdsKitSettings      s_defaultSettings;

        #endregion

        #region Static properties

        public static AdsKitSettings DefaultSettings
        {
            get
            {
                if (s_defaultSettings == null)
                {
                    var     instance    = LoadDefaultSettingsObject(throwError: false);
                    if (null == instance)
                    {
                        instance        = CreateDefaultSettingsObject();
                    }
                    s_defaultSettings   = instance;
                }
                return s_defaultSettings;
            }
            set
            {
                Assert.IsPropertyNotNull(value, nameof(value));

                // set new value
                s_defaultSettings       = value;
            }
        }

        public static bool SettingsExists
        {
            get
            {
                if (s_defaultSettings == null)
                {
                    s_defaultSettings   = LoadDefaultSettingsObject(throwError: false);
                }
                return (s_defaultSettings != null);
            }
        }

        #endregion

         #region Constructors

        static AdsKitSettingsEditorUtility()
        {
            AddGlobalDefines();
        }

        #endregion

        #region Static methods

        public static void ShowSettingsNotFoundErrorDialog()
        {
            EditorUtility.DisplayDialog(
                title: "Error",
                message: "AdsKit plugin is not configured. Please select plugin settings file from menu and configure it according to your preference.",
                ok: "Ok");
        }

        public static bool TryGetDefaultSettings(out AdsKitSettings defaultSettings)
        {
            // Set default value
            defaultSettings     = null;

            // Set reference if the object exists
            if (SettingsExists)
            {
                defaultSettings = DefaultSettings;
                return true;
            }
            return false;
        }

        public static void AddGlobalDefines()
        {
            ScriptingDefinesManager.AddDefine("ENABLE_VOXELBUSTERS_ADS_KIT");
        }
        
        public static void RemoveGlobalDefines()
        {
            ScriptingDefinesManager.RemoveDefine("ENABLE_VOXELBUSTERS_ADS_KIT");
        }

        public static void OpenInProjectSettings()
        {
            if (!SettingsExists)
            {
                CreateDefaultSettingsObject();
            }
            Selection.activeObject  = null;
            SettingsService.OpenProjectSettings(kLocalPathInProjectSettings);
        }

        [SettingsProvider]
        private static SettingsProvider CreateSettingsProvider()
        {
            return SettingsProviderZ.Create(settingsObject: DefaultSettings,
                                            path: kLocalPathInProjectSettings,
                                            scopes: SettingsScope.Project);
        }

        #endregion

        #region Private static methods

        private static AdsKitSettings CreateDefaultSettingsObject()
        {
            var contentDefaultSettings  =   GetDefaultAdContentSettings();
            var settings                =   AssetDatabaseUtility.CreateScriptableObject(assetPath: AdsKitSettings.DefaultSettingsAssetPath,
                                                               createFunc: () => AdsKitSettings.Create(adContentDefaultSettings: contentDefaultSettings));
            return settings;
        }

        private static AdsKitSettings LoadDefaultSettingsObject(bool throwError = true)
        {
            var     throwErrorFunc      = throwError ? () => Diagnostics.PluginNotConfiguredException() : (System.Func<System.Exception>)null;
            return AssetDatabaseUtility.LoadScriptableObject<AdsKitSettings>(assetPath: AdsKitSettings.DefaultSettingsAssetPath,
                                                                             throwErrorFunc: throwErrorFunc);
        }

        private static AdContentSettings GetDefaultAdContentSettings()
        {
            BannerAdOptionsAsset bannerAdOptions = GetOrCreateDefaultBannerAdOptionsAsset(AdsKitSettings.DefaultBannerAdOptionsAssetPath);

            return new AdContentSettings(bannerAdOptions);
        }

        private static BannerAdOptionsAsset GetOrCreateDefaultBannerAdOptionsAsset(string path)
        {
            BannerAdOptionsAsset bannerAdOptionsAsset = UnityEditor.AssetDatabase.LoadAssetAtPath<BannerAdOptionsAsset>(path);
            if (bannerAdOptionsAsset == null)
            {
                bannerAdOptionsAsset = BannerAdOptionsAsset.Create(AdPosition.Create(AdPositionPreset.BottomCenter), AdSizeName.FullBanner);
                AssetDatabaseUtility.CreateAssetAtPath(bannerAdOptionsAsset, path);
                UnityEditor.AssetDatabase.Refresh();
            }

            return bannerAdOptionsAsset;
        }

        #endregion

        #region Internal methods

        internal static void Rebuild()
        {
            // Actions
            WriteAndroidManifestFile();

            // Refresh Database
            AssetDatabase.Refresh();
        }

        public static void WriteAndroidManifestFile()
        {
            /*
            string manifestFolderPath = Constants.kAndroidPluginsReplayKitPath;
            if (AssetDatabaseUtils.FolderExists(manifestFolderPath))
            {
                var     generator       = new voxelbusters.adskit();
#if UNITY_2018_4_OR_NEWER
                generator.SaveManifest("com.voxelbusters.adskit", manifestFolderPath + "/AndroidManifest.xml");
#else
				generator.SaveManifest("com.voxelbusters.adskit", manifestFolderPath + "/AndroidManifest.xml");
#endif
            }
            */
        }

        #endregion
    }
}