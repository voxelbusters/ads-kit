using UnityEngine;
using System.Collections;
using VoxelBusters.CoreLibrary;

namespace VoxelBusters.AdsKit.Adapters
{
    public class AdNetworkLoadAdStateInfo : AdNetworkAdBasicInfo
    {
        #region Properties

        public AdPlacementState PlacementState { get; internal set; }

        public Error Error { get; internal set; }

        #endregion

        #region Constructors

        public AdNetworkLoadAdStateInfo(string adUnitId,
                                        AdType adType,
                                        string networkId,
                                        string placement,
                                        AdPlacementState placementState = AdPlacementState.Unknown,
                                        Error error = null)
            : base(adUnitId: adUnitId,
                   adType: adType,
                   networkId: networkId,
                   placement: placement)
        {
            // Set properties
            PlacementState  = placementState;
            Error           = error;
        }

        #endregion
    }
}