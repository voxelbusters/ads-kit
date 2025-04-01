using UnityEngine;
using System.Collections;
using System;

namespace VoxelBusters.AdsKit
{
    [Serializable]
    public class AdPlacementMeta
    {
        #region Fields

        [SerializeField]
        [Tooltip("A name to identify this ad")]
        private     string                      m_name;

        [SerializeField]
        [Tooltip("Ad type of this ad")]
        private     AdType                      m_adType;

        [SerializeField]
        [Tooltip("Option to enable auto load")]
        private     bool                        m_autoLoad;

        [SerializeField]
        [Tooltip("Optional options asset")]
        private     AdContentOptionsAsset       m_optionsObject;

        #endregion

        #region Properties

        public string Name
        {
            get => m_name;
            private set => m_name   = value;
        }

        public AdType AdType
        {
            get => m_adType;
            private set => m_adType = value;
        }

        public bool AutoLoad
        {
            get => m_autoLoad;
            private set => m_autoLoad   = value;
        }

        public AdContentOptions ContentOptions => m_optionsObject?.GetRawData();

        #endregion

        #region Constructors

        public AdPlacementMeta(string name,
                               AdType adType,
                               bool autoLoad,
                               AdContentOptionsAsset optionsObject = null)
        {
            // Set properties
            Name            = name;
            AdType          = adType;
            AutoLoad        = autoLoad;
            m_optionsObject = optionsObject;
        }

        #endregion
    }
}