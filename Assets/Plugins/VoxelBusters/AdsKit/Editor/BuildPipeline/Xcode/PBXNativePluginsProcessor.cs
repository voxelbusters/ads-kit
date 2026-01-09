#if UNITY_IOS || UNITY_TVOS
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.iOS.Xcode;
using VoxelBusters.AdsKit;
using VoxelBusters.CoreLibrary;
using VoxelBusters.CoreLibrary.Editor.NativePlugins.Build;
using VoxelBusters.CoreLibrary.Editor.NativePlugins.Build.Xcode;

namespace VoxelBusters.AdsKit.Editor.Build.Xcode
{
    public class PBXNativePluginsProcessor : CoreLibrary.Editor.NativePlugins.Build.Xcode.PBXNativePluginsProcessor
    {
#region Properties
        private AdsKitSettings Settings { get; set; }

        #endregion

        #region Base class methods
        public override void OnUpdateInfoPlist(PlistDocument doc)
        {
            // Check whether plugin is configured
            if (!EnsureInitialised()) return;

            // Add entries in info.plist
            // Add usage permissions
            var rootDict = doc.root;
            var permissions = GetUsagePermissions();
            foreach (string key in permissions.Keys)
            {
                rootDict.SetString(key, permissions[key]);
            }

            if (Settings.HasAdNetworkEnabled(AdNetworkServiceId.kLevelPlay))
            {
                // For arbitrary network loads
                AllowArbitraryLoads(rootDict);

                // Add SKAdNetworkIdentifier entries
                AddSkAdNetworkItem(rootDict);
            }
        }

        #region Private methods

        private Dictionary<string, string> GetUsagePermissions()
        {
            var requiredPermissionsDict = new Dictionary<string, string>(4);
            var trackingUsageDesription = Settings.UserTrackingUsageDescription;

            if (Settings.HasAnyAdNetworksEnabled())
            {
                requiredPermissionsDict["NSUserTrackingUsageDescription"] = trackingUsageDesription; //Replace with InfoPlistKey.kNSUserTrackingUsage
            }

            return requiredPermissionsDict;
        }

        private void AllowArbitraryLoads(PlistElementDict rootDict)
        {
            rootDict.CreateDict(InfoPlistKey.kNSAppTransportSecurity).SetBoolean(InfoPlistKey.kNSAllowsArbitraryLoads, false);
        }

        private void AddSkAdNetworkItem(PlistElementDict rootDict)
        {

            var skAdNetworkItemsKey = "SKAdNetworkItems";
            if(!rootDict.TryGetElement(skAdNetworkItemsKey, out PlistElementArray itemsArray))
            {
                itemsArray = rootDict.CreateArray(skAdNetworkItemsKey);
            }
                
            var     skAdNetworkItem    = itemsArray.AddDict();
            skAdNetworkItem.SetString("SKAdNetworkIdentifier", "su67r6k2v3.skadnetwork");
        }

        #endregion

        private bool EnsureInitialised()
        {
            if (Settings != null) return true;

            if (AdsKitSettingsEditorUtility.TryGetDefaultSettings(out AdsKitSettings settings))
            {
                Settings = settings;
                return true;
            }
            else
            {
                AdsKitSettingsEditorUtility.ShowSettingsNotFoundErrorDialog();
                return false;
            }
        }

        #endregion
    }
}
#endif