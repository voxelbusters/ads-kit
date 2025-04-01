using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelBusters.CoreLibrary;

namespace VoxelBusters.AdsKit
{
    [IncludeInDocs]
    public enum AdPlacementState
    {
        /// <summary> The Placement state is not known. </summary>
        Unknown = 0,

        /// <summary> The Placement is ready to show ads. </summary>
        Ready,
           
        /// <summary> The Placement is not available. </summary>
        NotAvailable,

        /// <summary> The Placement has been disabled. </summary>
        Disabled,

        /// <summary> The Placement is waiting to be ready. </summary>
        Waiting,

        /// <summary> The Placement has no advertisements to show. </summary>
        NoFill,
    }
}