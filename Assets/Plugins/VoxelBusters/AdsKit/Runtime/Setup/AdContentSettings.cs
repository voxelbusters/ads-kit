using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelBusters.AdsKit
{
    [System.Serializable]
    public class AdContentSettings
    {
        #region Fields

        [SerializeField]
        private     BannerAdOptionsAsset   m_banner;

        #endregion

        #region Properties

        public BannerAdOptions Banner => m_banner;

        #endregion

        #region Constructors

        public AdContentSettings(BannerAdOptionsAsset banner = null)
        {
            // Set properties
            m_banner  = banner;
        }

        #endregion
    }
}