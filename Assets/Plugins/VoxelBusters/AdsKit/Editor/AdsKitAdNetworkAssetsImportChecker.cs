using System;
using UnityEditor;
using UnityEngine;
using VoxelBusters.CoreLibrary;

namespace VoxelBusters.AdsKit.Editor
{
    public static class AdsKitAdNetworkAssetsImportChecker
    {        
        [InitializeOnLoadMethod]
        private static void Init()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }

            if(AdsKitSettingsEditorUtility.TryGetDefaultSettings(out var Settings))
            {
                var adNetworkAssets   = AssetImportManager.FindAssets();
                foreach (var asset in adNetworkAssets)
                {
                    bool isEnabled = Array.Exists(Settings.NetworkSettingsArray, (settingsItem) => string.Equals(asset.NetworkId, settingsItem.NetworkId) && settingsItem.IsEnabled);
                    if (isEnabled)
                    {
                        InstallIfRequired(asset);
                    }
                }                                                 
            }
        }

        private static void InstallIfRequired(AdNetworkAssetInfo asset)
        {
            if (!asset.IsInstalled())
            {
                Debug.Log($"Installing missing {asset.NetworkId} adapters. This can happen if you upgrade or reinstall the plugin...");
                EditorApplication.delayCall += () => asset.Install();
            }
        }
    }
}