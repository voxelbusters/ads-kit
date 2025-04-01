using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace VoxelBusters.AdsKit
{
    [System.Serializable, CreateAssetMenu(fileName= "BannerAdOptionsAsset", menuName = "Ads Kit/Banner Ad Options Asset", order = 100)]
    public class BannerAdOptionsAsset : AdContentOptionsAsset
    {
        #region Fields

        [SerializeField]
        private     AdPositionPreset        m_positionPreset;

        [SerializeField]
        private     Vector2Int              m_absolutePosition;

        [SerializeField]
        private     string                  m_sizeType;

        [SerializeField]
        private     AdSize                  m_customSize;

        #endregion

        #region Properties

        public AdPositionPreset PositionPreset
        {
            get => m_positionPreset;
            private set => m_positionPreset   = value;
        }

        public Vector2Int AbsolutePosition
        {
            get => m_absolutePosition;
            private set => m_absolutePosition = value;
        }

        public AdSize Size => GetSizeInternal();

        #endregion

        #region Static methods

        public static implicit operator BannerAdOptions(BannerAdOptionsAsset obj)
        {
            AdPosition position;

            if(obj.m_absolutePosition.x == 0 && obj.m_absolutePosition.y == 0)
            {
                position = AdPosition.Create(obj.m_positionPreset);
            }
            else
            {
                position = AdPosition.Create(obj.AbsolutePosition);
            }

            return new BannerAdOptions(position: position,
                                       size: obj.Size);
        }

        public static BannerAdOptionsAsset Create(AdPosition position,
                                                  string sizeType,
                                                  AdSize? customSize = null)
        {
            var     newInstance         = CreateInstance<BannerAdOptionsAsset>();
            newInstance.m_sizeType      = sizeType;
            newInstance.m_customSize    = customSize ?? new AdSize();
            newInstance.PositionPreset  = position.Preset ?? AdPositionPreset.TopLeft;
            newInstance.AbsolutePosition  = position.Absolute ?? Vector2Int.zero;
            return newInstance;
        }

        #endregion

        #region Base class methods

        public override AdContentOptions GetRawData() => (BannerAdOptions)this;

        #endregion

        #region Private methods

        private AdSize GetSizeInternal()
        {
            if (string.Equals(m_sizeType, AdSizeName.Banner))
            {
                return AdSize.Banner();
            }
            else if (string.Equals(m_sizeType, AdSizeName.LargeBanner))
            {
                return AdSize.LargeBanner();
            }
            else if (string.Equals(m_sizeType, AdSizeName.MediumRectangle))
            {
                return AdSize.MediumRectangle();
            }
            else if (string.Equals(m_sizeType, AdSizeName.FullBanner))
            {
                return AdSize.FullBanner();
            }
            else if (string.Equals(m_sizeType, AdSizeName.Leaderboard))
            {
                return AdSize.Leaderboard();
            }
            else
            {
                return m_customSize;
            }
        }

        #endregion
    }
}