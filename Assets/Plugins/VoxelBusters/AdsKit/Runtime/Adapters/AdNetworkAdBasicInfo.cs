using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelBusters.AdsKit.Adapters
{
    public class AdNetworkAdBasicInfo
    {
        #region Properties

        public string AdUnitId { get; private set; }

        public AdType AdType { get; private set; }

        public string Placement { get; private set; }

        public string NetworkId { get; private set; }

        #endregion

        #region Constructors

        protected AdNetworkAdBasicInfo(string adUnitId,
                                       AdType adType,
                                       string placement,
                                       string networkId)
        {
            // Set properties
            AdUnitId        = adUnitId;
            AdType          = adType;
            Placement       = placement;
            NetworkId       = networkId;
        }

        #endregion

        public override string ToString()
        {
            return string.Format("[AdUnitId={0}, AdType={1}, Placement={2}, NetworkId={3}",
                                 AdUnitId,
                                 AdType,
                                 Placement,
                                 NetworkId);
        }
    }
}