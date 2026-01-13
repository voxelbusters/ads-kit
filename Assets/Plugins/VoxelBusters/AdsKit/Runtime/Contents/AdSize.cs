using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelBusters.CoreLibrary;

namespace VoxelBusters.AdsKit
{
    public static class AdSizeName
    {
        public static string Banner => "Banner";

        public static string LargeBanner => "LargeBanner";

        public static string MediumRectangle => "MediumRectangle";

        public static string FullBanner => "FullBanner";

        public static string Leaderboard => "Leaderboard";
    }

    /// <summary>
    /// Ad size object.
    /// </summary>
    [System.Serializable, IncludeInDocs]
    public struct AdSize
    {
        #region Fields

        [SerializeField]
        private     int                 m_width;

        [SerializeField]
        private     int                 m_height;

        [SerializeField]
        private     AdSizeType          m_type;

        [SerializeField]
        private     ScreenOrientation   m_anchorOrientation;

        [SerializeField]
        private     string              m_name;

        #endregion

        #region Properties

        public int Width => m_width;

        public int Height => m_height;

        public AdSizeType Type => m_type;

        public ScreenOrientation AnchorOrientation => m_anchorOrientation;

        public string Name => m_name;

        #endregion

        #region Constructors

        public AdSize(int width,
                      int height,
                      AdSizeType type,
                      ScreenOrientation anchorOrientation = 0,
                      string name = null)
        {
            // Set properties
            m_width             = width;
            m_height            = height;
            m_type              = type;
            m_anchorOrientation = anchorOrientation;
            m_name              = name;
        }

        #endregion

        #region Static methods

        public static AdSize Banner() => new AdSize(320, 50, AdSizeType.Fixed, name: AdSizeName.Banner);

        public static AdSize LargeBanner() => new AdSize(320, 100, AdSizeType.Fixed, name: AdSizeName.LargeBanner);

        public static AdSize MediumRectangle() => new AdSize(300, 250, AdSizeType.Fixed, name: AdSizeName.MediumRectangle);

        public static AdSize FullBanner() => new AdSize(468, 60, AdSizeType.Fixed, name: AdSizeName.FullBanner);

        public static AdSize Leaderboard() => new AdSize(728, 90, AdSizeType.Fixed, name: AdSizeName.Leaderboard);

        public static AdSize AnchoredAdaptive(ScreenOrientation orientation, int width) => new AdSize(width, -1, AdSizeType.AnchoredAdaptive, orientation);

        #endregion

        #region Nested types

        public enum AdSizeType
        {
            Fixed = 0,

            AnchoredAdaptive
        }

        #endregion

        #region Comparision methods

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            var other = (AdSize)obj;
            return other.m_width == m_width && other.m_height == m_height && other.m_type == m_type && other.m_anchorOrientation == m_anchorOrientation;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion
    }
}