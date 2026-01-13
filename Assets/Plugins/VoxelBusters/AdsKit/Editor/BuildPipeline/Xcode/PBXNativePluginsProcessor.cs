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

            if (Settings.HasAdNetworkEnabled(AdNetworkServiceId.kLevelPlay))
            {
                var     rootDict    = doc.root;
                
                // For arbitrary network loads
                AllowArbitraryLoads(rootDict);
                
                // Add SKAdNetworkIdentifier entries
                AddSkAdNetworkItem(rootDict);
            }
        }
        private void AllowArbitraryLoads(PlistElementDict rootDict)
        {
            rootDict.CreateDict(InfoPlistKey.kNSAppTransportSecurity).SetBoolean(InfoPlistKey.kNSAllowsArbitraryLoads, true);
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

#region Private methods

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