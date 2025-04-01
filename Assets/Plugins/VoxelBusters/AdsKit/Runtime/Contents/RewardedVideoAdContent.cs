using UnityEngine;
using System.Collections;
using VoxelBusters.AdsKit.Adapters;
using VoxelBusters.CoreLibrary;

namespace VoxelBusters.AdsKit
{
    /// <summary>
    /// The rewarded video ad content object.
    /// </summary>
    [IncludeInDocs]
    public class RewardedVideoAdContent : AdContent
    {
        #region Constructors

        internal RewardedVideoAdContent(string placement,
                                        AdsManagerImpl manager = null,
                                        AdNetworkAdapter provider = null)
            : base(AdType.RewardedVideo, placement, manager, provider)
        { }

        #endregion

        #region Create methods

        public static RewardedVideoAdContent Create(string placement)
        {
            return new RewardedVideoAdContent(placement);
        }

        #endregion
    }
}