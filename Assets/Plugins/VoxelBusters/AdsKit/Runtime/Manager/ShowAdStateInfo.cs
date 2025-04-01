using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelBusters.CoreLibrary;

namespace VoxelBusters.AdsKit
{
    public enum ShowAdState
    {
        Started = 1,

        Clicked,

        Finished,

        Failed
    }

    public class ShowAdStateInfo
    {
        #region Properties

        public string AdUnitId { get; private set; }

        public AdType AdType { get; private set; }

        public string Placement { get; private set; }

        public string NetworkId { get; private set; }

        public ShowAdState State { get; internal set; } 

        public ShowAdResult Result { get; internal set; }

        public Error Error { get; internal set; }

        #endregion

        #region Constructors

        public ShowAdStateInfo(string adUnitId,
                               AdType adType,
                               string placement,
                               string networkId,
                               ShowAdState state,
                               ShowAdResult result = null,
                               Error error = null)
        {
            // Set properties
            AdUnitId        = adUnitId;
            AdType          = adType;
            Placement       = placement;
            NetworkId       = networkId;
            State           = state;
            Result          = result;
            Error           = error;
        }

        #endregion
    }
}