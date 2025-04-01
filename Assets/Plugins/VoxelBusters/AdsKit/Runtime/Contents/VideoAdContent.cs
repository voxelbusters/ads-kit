using UnityEngine;
using System.Collections;
using VoxelBusters.AdsKit.Adapters;
using VoxelBusters.CoreLibrary;

namespace VoxelBusters.AdsKit
{
    /// <summary>
    /// The video ad content object.
    /// </summary>
    [IncludeInDocs]
    public class VideoAdContent : AdContent
    {
        #region Constructors

        internal VideoAdContent(string placement,
                                AdsManagerImpl manager = null,
                                AdNetworkAdapter provider = null)
            : base(AdType.Video, placement, manager, provider)
        { }

        #endregion

        #region Create methods

        public static VideoAdContent Create(string placement)
        {
            return new VideoAdContent(placement);
        }

        #endregion
    }
}