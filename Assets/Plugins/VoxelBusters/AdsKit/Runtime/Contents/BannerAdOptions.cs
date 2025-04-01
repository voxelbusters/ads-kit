using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelBusters.CoreLibrary;

namespace VoxelBusters.AdsKit
{
    /// <summary>
    /// The base class for ad content options.
    /// </summary>
    [System.Serializable, IncludeInDocs]
    public class BannerAdOptions : AdContentOptions
    {
        #region Fields

        [SerializeField]
        private     AdPosition              m_position;

        [SerializeField]
        private     AdSize                  m_size;

        //TODO: Option for respecting safe area
        #endregion

        #region Properties

        public AdPosition Position
        {
            get => m_position;
            set => m_position   = value;
        }

        public AdSize Size
        {
            get => m_size;
            set => m_size   = value;
        }

        #endregion

        #region Constructors

        public BannerAdOptions(AdPosition position,
                               AdSize size)
            : base(targetType: AdType.Banner)
        {
            // Set properties
            Position            = position;
            Size                = size;
        }

        #endregion
    }
}