using VoxelBusters.AdsKit.Adapters;
using VoxelBusters.CoreLibrary;

namespace VoxelBusters.AdsKit
{
    /// <summary>
    /// The rewarded interstitial ad content object.
    /// </summary>
    [IncludeInDocs]
    public class RewardedInterstitialAdContent : AdContent
    {
        #region Constructors

        internal RewardedInterstitialAdContent(string placement,
                                        AdsManagerImpl manager = null,
                                        AdNetworkAdapter provider = null)
            : base(AdType.RewardedInterstitial, placement, manager, provider)
        { }

        #endregion

        #region Create methods

        public static RewardedInterstitialAdContent Create(string placement)
        {
            return new RewardedInterstitialAdContent(placement);
        }

        #endregion
    }
}