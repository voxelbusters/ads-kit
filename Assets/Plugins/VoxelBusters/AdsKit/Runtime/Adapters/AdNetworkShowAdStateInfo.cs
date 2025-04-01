using UnityEngine;
using System.Collections;
using VoxelBusters.CoreLibrary;

namespace VoxelBusters.AdsKit.Adapters
{
    public class AdNetworkShowAdStateInfo : AdNetworkAdBasicInfo
    {
        #region Properties

        public ShowAdState State { get; private set; }

        public bool Clicked { get; private set; }

        public ShowAdResultCode? ResultCode { get; private set; }

        public double? WatchDuration { get; private set; }

        public Error Error { get; private set; }

        #endregion

        #region Constructors

        public AdNetworkShowAdStateInfo(string adUnitId,
                                        AdType adType,
                                        string placement,
                                        string networkId,
                                        ShowAdState state,
                                        bool clicked = false,
                                        ShowAdResultCode? resultCode = null,
                                        double? watchDuration = null,
                                        Error error = null)
            : base(adUnitId: adUnitId,
                   adType: adType,
                   networkId: networkId,
                   placement: placement)
        {
            // Set properties
            State           = state;
            Clicked         = clicked;
            ResultCode      = resultCode;
            WatchDuration   = watchDuration;
            Error           = error;
        }

        #endregion
    }
}