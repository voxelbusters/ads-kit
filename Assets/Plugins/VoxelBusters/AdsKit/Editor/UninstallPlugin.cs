using UnityEngine;
using System.Collections;
using UnityEditor;
using VoxelBusters.CoreLibrary;

namespace VoxelBusters.AdsKit.Editor
{
	public class UninstallPlugin
	{
		#region Constants
	
		private const	string	kAlertTitle				= "AdsKit";

		private const	string	kUninstallMessage		= "Backup before doing this step to preserve changes done with in this plugin. This deletes files only related to AdsKit plugin. Do you want to proceed?";
		
		#endregion	
	
		#region Methods
	
		public static void Uninstall()
		{
			bool	approved			= EditorUtility.DisplayDialog(kAlertTitle, kUninstallMessage, "Uninstall", "Cancel");
			if (approved)
			{
				var		kPluginFolders	=	new string[]
				{
					AdsKitSettings.Package.DefaultInstallPath,
					AdsKitSettings.Package.UpmInstallPath
				};
				foreach (string fileOrFolder in kPluginFolders)
				{
					IOServices.DeleteFileOrDirectory(fileOrFolder);
					IOServices.DeleteFileOrDirectory(fileOrFolder + ".meta");
				}
				AssetDatabase.Refresh();
                EditorUtility.DisplayDialog(
					title: kAlertTitle,
					message: "Uninstall successful!",
					ok: "Ok");
			}
		}
		
		#endregion
	}
}