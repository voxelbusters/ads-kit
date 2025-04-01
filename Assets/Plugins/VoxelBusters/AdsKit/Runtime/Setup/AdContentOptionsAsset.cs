using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelBusters.AdsKit
{
    public abstract class AdContentOptionsAsset : ScriptableObject
    {
        #region Abstract methods

        public abstract AdContentOptions GetRawData();

        #endregion
    }
}