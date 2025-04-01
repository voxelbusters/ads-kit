using UnityEngine;
using VoxelBusters.CoreLibrary;

namespace VoxelBusters.AdsKit
{
    /// <summary>
    /// Specify either a preset position or an absolute postion (from Top Left) with AdPosition
    /// </summary>
    [IncludeInDocs]
    public class AdPosition
    {

        public AdPositionPreset? Preset { get; private set; }

        public Vector2Int? Absolute {get; private set; } //From Top Left

        private AdPosition() {}

        public static AdPosition Create(AdPositionPreset preset)
        {
            return new AdPosition() {
                Preset = preset
            };
        }

        public static AdPosition Create(Vector2Int absolutePosition)
        {
            return new AdPosition() {
                Absolute = absolutePosition
            };
        }
    }
}