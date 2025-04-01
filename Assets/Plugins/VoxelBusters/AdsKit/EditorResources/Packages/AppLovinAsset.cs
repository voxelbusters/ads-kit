#if UNITY_EDITOR
using VoxelBusters.CoreLibrary;

namespace VoxelBusters.AdsKit.Editor.Clients
{
    public class AppLovinAsset : AdNetworkAssetInfo
    {
       #region Constructors

       public AppLovinAsset()
            : base(networkId: AdNetworkServiceId.kAppLovin,
                   name: AdNetworkServiceId.kAppLovin,
                   description: "Empowers data-driven marketing, and ensures profitable growth.",
                   importPaths: new string[]
                   {
                       "https://github.com/AppLovin/AppLovin-MAX-Unity-Plugin/releases", //6.1.1
                       $"{AdsKitSettings.Package.GetEditorResourcesPath()}/Packages/AppLovinAdapter.unitypackage",
                   },
                   installPaths: new string[]
                   {
                       "Assets/MaxSdk",
                       $"{AdsKitSettings.Package.GetGeneratedPath()}/AppLovin",
                   })
        { }

        #endregion
    }
}
#endif