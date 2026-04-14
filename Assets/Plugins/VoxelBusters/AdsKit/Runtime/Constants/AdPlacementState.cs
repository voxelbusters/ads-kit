using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelBusters.CoreLibrary;

namespace VoxelBusters.AdsKit
{
    [IncludeInDocs]
    public enum AdPlacementState
    {
        /// <summary> The placement state cannot be determined. </summary>
        Unknown = 0,

        /// <summary> The placement has no loaded ad. </summary>
        NoAdLoaded = 1,

        /// <summary> The placement is currently loading an ad. </summary>
        Loading = 2,

        /// <summary> The last ad load attempt failed. </summary>
        LoadFailed = 3,

        /// <summary> The placement has a loaded ad ready to show. </summary>
        ReadyToShow = 4,

        /// <summary> The placement is currently showing an ad. </summary>
        Showing = 5,

        /// <summary> Ads are disabled. </summary>
        Disabled = 6,
    }
}
