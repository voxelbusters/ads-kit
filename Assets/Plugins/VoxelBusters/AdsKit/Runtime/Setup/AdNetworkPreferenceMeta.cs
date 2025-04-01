using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelBusters.CoreLibrary;

namespace VoxelBusters.AdsKit
{
    [System.Serializable]
    public class AdNetworkPreferenceMeta
    {
        #region Constants

        private     static  readonly    string[]    s_emptyArray    = new string[0];    

        #endregion

        #region Fields

        [SerializeField, AdNetworkId]
        private     string[]        m_banner;

        [SerializeField, AdNetworkId]
        private     string[]        m_interstitial;
        
        [SerializeField, AdNetworkId]
        private     string[]        m_rewardedInterstitial;

        [SerializeField, AdNetworkId]
        private     string[]        m_video;

        [SerializeField, AdNetworkId]
        private     string[]        m_rewardedVideo;

        [SerializeField, AdNetworkId, HideInInspector]
        private     string[]        m_augmentedReality;

        [SerializeField, AdNetworkId, HideInInspector]
        private     string[]        m_playable;

        [SerializeField, AdNetworkId, HideInInspector]
        private     string[]        m_iapPromo;

        #endregion

        #region Properties

        public string[] Banner => m_banner;

        public string[] Interstitial => m_interstitial;
        
        public string[] RewardedInterstitial => m_rewardedInterstitial;
        public string[] Video => m_video;
        public string[] RewardedVideo => m_rewardedVideo;

        public string[] AugmentedReality => m_augmentedReality;

        public string[] Playable => m_playable;

        public string[] IapPromo => m_iapPromo;

        #endregion

        #region Constructors

        public AdNetworkPreferenceMeta(string[] banner = null, 
            string[] interstitial = null, string[] rewardedInterstitial = null,
            string[] video = null, string[] rewardedVideo = null,
            string[] augmentedReality = null, string[] playable = null,
            string[] iapPromo = null)
        {
            // Set properties
            m_banner                    = banner ?? s_emptyArray;
            m_interstitial              = interstitial ?? s_emptyArray;
            m_rewardedInterstitial      = rewardedInterstitial ?? s_emptyArray;
            m_video                     = video ?? s_emptyArray;
            m_rewardedVideo             = rewardedVideo ?? s_emptyArray;
            m_augmentedReality          = augmentedReality ?? s_emptyArray;
            m_playable                  = playable ?? s_emptyArray;
            m_iapPromo                  = iapPromo ?? s_emptyArray;
        }

        #endregion

#region Helpers

        public bool HasAnyPreferredNetworksConfigured(AdType adType)
        {
            switch(adType)
            {
                case AdType.Banner:
                    return !IsEmpty(m_banner);
                case AdType.Interstitial:
                    return !IsEmpty(m_interstitial);
                case AdType.RewardedInterstitial:
                    return !IsEmpty(m_rewardedInterstitial);
                case AdType.RewardedVideo:
                    return !IsEmpty(m_rewardedVideo);
                case AdType.Video:
                    return !IsEmpty(m_video);
                default:
                    throw new System.Exception($"Ad type not configured {adType}");
            }

            bool IsEmpty(string[] array)
            {
                return array.Length == 0 || string.Join("", array).Length == 0;
            }
        }

#endregion
    }
}