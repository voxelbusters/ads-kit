using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelBusters.CoreLibrary;

namespace VoxelBusters.AdsKit
{
    /// <summary>
    /// The base class for ad content options.
    /// </summary>
    [IncludeInDocs]
    public abstract class AdContentOptions
    {
        #region Properties

        internal AdType TargetType { get; private set; }

        #endregion

        #region Constructors

        protected AdContentOptions(AdType targetType)
        {
            // Set properties
            TargetType  = targetType;
        }

        #endregion

        #region Abstract methods

        internal bool IsCompatibleWithAdType(AdType adType) => (TargetType == adType);

        #endregion
    }
}