#if UNITY_EDITOR
using VoxelBusters.CoreLibrary;

namespace VoxelBusters.AdsKit.Editor.Clients
{
    public class LevelPlayAsset : AdNetworkAssetInfo
    {
       #region Constructors

       public LevelPlayAsset()
            : base(networkId: AdNetworkServiceId.kLevelPlay,
                   name: "Level Play",
                   description: "Created by the leading mobile game engine Unity and Iron Source.",
                   importPaths: new string[]
                   {
                       "https://github.com/ironsource-mobile/Unity-sdk/releases/latest", // 9.2.0 from 8.7.0
                       $"{AdsKitSettings.Package.GetEditorResourcesPath()}/Packages/LevelPlayAdapter.unitypackage",
                   },
                   installPaths: new string[]
                   {
                       "Packages/com.unity.services.levelplay",
                       "Assets/LevelPlay",
                       $"{AdsKitSettings.Package.GetGeneratedPath()}/LevelPlay",
                   })
        { }

        #endregion
    }
}
#endif