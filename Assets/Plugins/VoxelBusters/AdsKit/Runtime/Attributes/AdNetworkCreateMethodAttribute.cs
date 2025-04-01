using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelBusters.AdsKit
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AdNetworkCreateMethodAttribute : Attribute
    {
        #region Constructors

        public AdNetworkCreateMethodAttribute()
        { }

        #endregion
    }
}
