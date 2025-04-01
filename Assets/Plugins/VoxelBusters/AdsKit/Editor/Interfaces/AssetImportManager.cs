using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using VoxelBusters.CoreLibrary;
using VoxelBusters.CoreLibrary.Editor;

namespace VoxelBusters.AdsKit.Editor
{
    public static class AssetImportManager
    {
        #region Constants

        private     const   string      kPackageIdentifierPrefix    = "Packages/";

        #endregion
        
        #region Find Methods

        public static AdNetworkAssetInfo[] FindAssets()
        {
            var     baseType        = typeof(AdNetworkAssetInfo);
            var     instanceList    = new List<AdNetworkAssetInfo>();
            foreach (var type in ReflectionUtility.FindAllTypes(predicate: (type) => type.IsClass && !type.IsAbstract && type.IsSubclassOf(baseType)))
            {
                var     instance    = ReflectionUtility.CreateInstance(type) as AdNetworkAssetInfo;
                if (instance == null) continue;

                instanceList.Add(instance);
            }
            instanceList.Sort((x,y)=> x.NetworkId.CompareTo(y.NetworkId));

            return instanceList.ToArray();
        }

        #endregion

        #region Asset methods

        public static bool IsInstalled(this AdNetworkAssetInfo asset)
        {
            foreach (var path in asset.InstallPaths)
            {
                if (IsPackageIdentifier(path, out string identifier))
                {
                    if(!AssetDatabase.IsValidFolder($"Packages/{identifier}"))
                        return false;
                }

                if(!AssetDatabase.IsValidFolder(path))
                    return false;
            }
            
            return true;
        }

        public static void Install(this AdNetworkAssetInfo asset)
        {
            AssetDatabase.StartAssetEditing();

            // Create operation instances
            var     operations  = new List<IAsyncOperation>();
            foreach (var path in asset.ImportPaths)
            {
                if (IsPackageIdentifier(path, out string identifier))
                {
                    operations.Add(new AddUpmPackageRequest(identifier));
                }
                else if (IOServices.FileExists(path) && path.EndsWith("unitypackage"))
                {
                    operations.Add(new ImportPackageRequest(path));
                }
                else if (IsWebUrl(path))
                {
                    if(AssetDatabase.IsValidFolder(asset.InstallPaths[0]) || IsPackageIdentifier(asset.InstallPaths[0], out string _)) //Ignore if the path to which it installed is a package path instead of a folder
                        continue;

                    
                    if (EditorUtility.DisplayDialog($"Enable {asset.Name}",
                                                $"To enable {asset.Name}, you need to download the {asset.Name}'s unity package. \n\nImport the unity package after downloading to complete integration.",
                                                $"Download {asset.Name} Package",
                                                null))
                    {
                        Application.OpenURL(path);
                    }
                }
                else
                {
                    DebugLogger.LogError(AdsKitSettings.Domain, $"Failed to install the {asset.Name} asset as import path:{path} is invalid.");
                }
            }

            // Start operation
            var     groupOperation      = new ChainedOperation(abortOnError: true,
                                                               operations: operations.ToArray());
            groupOperation.OnComplete  += (op) =>
            {
                if (op.Status == AsyncOperationStatus.Succeeded)
                {
                    asset.OnInstall();
                }
                AssetDatabase.StopAssetEditing();
            };
            groupOperation.Start();
        }

        public static void Uninstall(this AdNetworkAssetInfo asset, bool includeAdNetworkUninstallation)
        {
            AssetDatabase.StartAssetEditing();

            var     operations  = new List<IAsyncOperation>();
            foreach (var path in asset.InstallPaths)
            {
                //We will skip ad network installed folders and only delete our integrations if includeAdNetworkUninstallation is false
                if (!path.Contains("Generated") && !includeAdNetworkUninstallation)
                {
                    continue;
                }
                
                if (IsPackageIdentifier(path, out string identifier))
                {
                    operations.Add(new RemoveUpmPackageRequest(identifier));
                }
                else if (AssetDatabase.IsValidFolder(path))
                {
                    operations.Add(new DeleteAssetRequest(path));
                }
            }

            // Start operation
            var     groupOperation      = new ChainedOperation(abortOnError: true,
                                                               operations: operations.ToArray());
            groupOperation.OnComplete  += (op) =>
            {
                if (op.Status == AsyncOperationStatus.Succeeded)
                {
                    asset.OnUninstall();
                }
                AssetDatabase.StopAssetEditing();
            };
            groupOperation.Start();
        }

        #endregion

        #region Private methods

        private static bool IsPackageIdentifier(string path, out string identifier)
        {
            identifier      = null;

            if (path.StartsWith(kPackageIdentifierPrefix))
            {
                identifier  = path.Substring(kPackageIdentifierPrefix.Length);
                return true;
            }
            return false;
        }

        private static bool IsWebUrl(string path)
        {
            return path.StartsWith("https://");
        }

        #endregion
    }
}