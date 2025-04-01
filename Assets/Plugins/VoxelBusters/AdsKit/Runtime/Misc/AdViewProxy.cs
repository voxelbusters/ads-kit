using System;
using UnityEngine;

namespace VoxelBusters.AdsKit
{
    public class AdViewProxy
    {
        public Vector2Int? Size { get; private set; }

        public AdViewProxy(Vector2Int? size = null)
        {
            Size = size;          
        }
    }
}