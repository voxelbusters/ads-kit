using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelBusters.CoreLibrary;

namespace VoxelBusters.AdsKit
{
    /// <summary>
    /// The enumerated states of the user’s interaction with the ad.
    /// </summary>
    [IncludeInDocs]
    public enum ShowAdResultCode
    {
        Unknown = 0,

        /// <summary> Indicates that the user skipped the ad. </summary>
        Skipped,

        /// <summary> Indicates that the ad display is complete. </summary>
        Finished,

        /// <summary> Indicates that the ad failed to complete due to a Unity service error. </summary>
        Failed,
    }

    /// <summary>
    /// The object represents show ads result.
    /// </summary>
    [IncludeInDocs]
    public class ShowAdResult
    {
        #region Properties

        public string AdUnitId { get; private set; }

        public AdType AdType { get; private set; }

        public string Placement { get; private set; }

        public string NetworkId { get; private set; }

        public bool Clicked { get; private set; }

        public ShowAdResultCode ResultCode { get; private set; }

        public double? WatchDuration { get; private set; }

        #endregion

        #region Constructors

        public ShowAdResult(string adUnitId,
                            AdType adType,
                            string placement,
                            string networkId,
                            bool clicked,
                            ShowAdResultCode resultCode = ShowAdResultCode.Unknown,
                            double? watchDuration = null)
        {
            // Set properties
            AdUnitId        = adUnitId;
            AdType          = adType;
            Placement       = placement;
            NetworkId       = networkId;
            Clicked         = clicked;
            ResultCode      = resultCode;
            WatchDuration   = watchDuration;
        }

        #endregion
    }
    
}