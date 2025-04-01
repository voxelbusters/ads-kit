using UnityEngine;
using System.Collections;
using VoxelBusters.AdsKit.Adapters;
using VoxelBusters.CoreLibrary;

namespace VoxelBusters.AdsKit
{
    /// <summary>
    /// The interstitial ad content object.
    /// </summary>
    [IncludeInDocs]
    public class InterstitialAdContent : AdContent
    {
        #region Constructors

        internal InterstitialAdContent(string placement,
                                       AdsManagerImpl manager = null,
                                       AdNetworkAdapter provider = null)
            : base(AdType.Interstitial, placement, manager, provider)
        { }

        #endregion

        #region Create methods

        public static InterstitialAdContent Create(string placement)
        {
            return new InterstitialAdContent(placement);
        }

        #endregion
    }
}