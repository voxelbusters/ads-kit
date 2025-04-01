using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using VoxelBusters.CoreLibrary;

namespace VoxelBusters.AdsKit.Editor
{
    public abstract class AdNetworkSettingsSyncer  : IPreprocessBuildWithReport
    {
        public int callbackOrder => -int.MaxValue;

        public void OnPreprocessBuild(BuildReport report)
        {
            if(!AdsKitSettingsEditorUtility.SettingsExists)
                return;

            var settings = AdsKitSettingsEditorUtility.DefaultSettings;

            for(int i = 0; i < settings.NetworkSettingsArray.Length; i++)
            {
                var networkSettings = settings.NetworkSettingsArray[i];

                if (!networkSettings.IsEnabled)
                    continue;

                if (!networkSettings.NetworkId.Equals(GetNetworkId()))
                    continue;

                // Get existing keys
                networkSettings.GetApiKeysForPlatform(ApplicationServices.GetActiveOrSimulationPlatform(), settings.IsDebugBuild, out string apiKey, out string apiSecret);

                try
                {
                    Sync(apiKey, apiSecret);
                }
                catch (Exception e)
                {
                    DebugLogger.LogException("Contact AdsKit plugin support with your AdMob version. Exception: ", e);
                }
                break;
            }
        }

        protected abstract string GetNetworkId();
        protected abstract void Sync(string apiKey, string apiSecret);
    }
}