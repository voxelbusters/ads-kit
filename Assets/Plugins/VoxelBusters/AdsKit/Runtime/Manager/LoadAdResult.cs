using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelBusters.AdsKit.Adapters;
using VoxelBusters.CoreLibrary;

namespace VoxelBusters.AdsKit
{
    /// <summary>
    /// The object represents load ads result.
    /// </summary>
    [IncludeInDocs]
    public class LoadAdResult
    {
        #region Properties

        public string AdUnitId { get; private set; }

        public AdType AdType { get; private set; }

        public string Placement { get; private set; }

        public AdPlacementState PlacementState { get; private set; }

        public AdNetworkLoadAdStateInfo[] LoadStateArray { get; private set; }

        public string PreferredAdProvider { get; private set; }

        #endregion

        #region Constructors

        public LoadAdResult(string adUnitId,
                            AdType adType,
                            string placement,
                            AdPlacementState placementState,
                            AdNetworkLoadAdStateInfo[] loadStateArray,
                            string preferredAdProvider = null)
        {
            // Set properties
            AdUnitId            = adUnitId;
            AdType              = adType;
            Placement           = placement;
            PlacementState      = placementState;
            LoadStateArray      = loadStateArray;
            PreferredAdProvider = preferredAdProvider;
        }

        #endregion
    }
}