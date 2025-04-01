using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VoxelBusters.CoreLibrary;
using VoxelBusters.CoreLibrary.NativePlugins;
using VoxelBusters.CoreLibrary.Editor;

namespace VoxelBusters.AdsKit.Editor
{
    public static class AdsKitEditorUtility
    {
        #region Constants

		// URL
        private     const   string      kProductUrl                     = "https://u3d.as/37du";

		private 	const   string      kSupportUrl			            = "https://discord.gg/voxel-busters-672868273779507223";

		private		const   string      kTutorialUrl		            = "https://adskit.voxelbusters.com/docs";		

		private		const   string	    kSubscribePageUrl	            = "https://www.voxelbusters.com";

        #endregion

        #region Resource methods

        public static void OpenTutorialsPage()
        {
            Application.OpenURL(kTutorialUrl);
        }

        public static void OpenSupportPage()
        {
            Application.OpenURL(kSupportUrl);
        }

        public static void OpenSubscribePage()
        {
            Application.OpenURL(kSubscribePageUrl);
        }

        public static void OpenProductPage()
        {
            Application.OpenURL(kProductUrl);
        }

        #endregion
    }
}