using UnityEngine;
using System.Collections;

namespace VoxelBusters.AdsKit
{
    public class UserSettings
    {
        #region Properties

        public bool? Muted { get; private set; }

        public float? Volume { get; private set; }

        #endregion

        #region Constructors

        public UserSettings(bool? muted = null, float? volume = null)
        {
            // Set properties
            Muted       = muted;
            Volume      = volume;
        }

        #endregion
    }
}