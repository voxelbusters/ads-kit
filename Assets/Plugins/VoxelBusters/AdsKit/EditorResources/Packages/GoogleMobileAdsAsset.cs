#if UNITY_EDITOR
using VoxelBusters.CoreLibrary;

namespace VoxelBusters.AdsKit.Editor.Clients
{
    public class GoogleMobileAds : AdNetworkAssetInfo
    {
       #region Constructors

       public GoogleMobileAds()
            : base(networkId: AdNetworkServiceId.kGoogleMobileAds,
                   name: "Ad Mob",
                   description: "AdMob is a mobile advertising subsidiary of Google.",
                   importPaths: new string[]
                   {
                       $"https://github.com/googleads/googleads-mobile-unity/releases",//9.1.0
                       $"{AdsKitSettings.Package.GetEditorResourcesPath()}/Packages/GoogleMobileAdsAdapter.unitypackage",
                   },
                   installPaths: new string[]
                   {
                       "Assets/GoogleMobileAds",
                       $"{AdsKitSettings.Package.GetGeneratedPath()}/AdMob",
                   })
        { }

        #endregion
    }
}
#endif