using UnityEngine;
using System.Collections;

namespace VoxelBusters.AdsKit.Adapters
{
    public class AdNetworkAdImpressionInfo : AdNetworkAdBasicInfo
    {
        #region Constructors

        public AdNetworkAdImpressionInfo(string adUnitId,
                                                      AdType adType,
                                                      string placement,
                                                      string networkId)
            : base(adUnitId: adUnitId,
                   adType: adType,
                   networkId: networkId,
                   placement: placement)
        { }

        #endregion
    }
}