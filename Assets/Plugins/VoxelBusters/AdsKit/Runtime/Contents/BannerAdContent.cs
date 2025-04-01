using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using VoxelBusters.AdsKit.Adapters;
using VoxelBusters.CoreLibrary;

namespace VoxelBusters.AdsKit
{
    /// <summary>
    /// The banner ad content object.
    /// </summary>
    [IncludeInDocs]
    public class BannerAdContent : AdContent
    {
        #region Properties

        public AdPosition Position { get; private set; }

        public AdSize? RequestedSize { get; private set; }

        public Vector2Int? DisplaySize { get; private set; } //In Pixels

        #endregion

        #region Constructors

        internal BannerAdContent(string placement,
                                 AdsManagerImpl manager = null,
                                 AdNetworkAdapter provider = null,
                                 AdContentOptions options = null)
            : base(AdType.Banner, placement, manager, provider)
        { 
            BannerAdOptions bannerOptions = (BannerAdOptions) options;
            AdViewProxy viewProxy = provider.GetAdViewProxy(placement);

            if(bannerOptions != null) 
            { 
                SetPosition(bannerOptions.Position);
                SetRequestedSize(bannerOptions.Size);

                if(viewProxy != null)
                {
                    SetDisplaySize(viewProxy.Size);
                }
            }
        }

        #endregion

        #region Create methods

        public static BannerAdContent Create(string placement)
        {
            return new BannerAdContent(placement);
        }

        #endregion

        #region Base class methods

        protected override AdContentOptions BuildContentOptions()
        {
            var     defaultOptions  = Manager.GetDefaultAdContentOptions(AdType.Banner) as BannerAdOptions;
            return new BannerAdOptions(position: Position ?? defaultOptions.Position,
                                       size: RequestedSize ?? defaultOptions.Size);

        }

        #endregion

        #region Public methods

        public void HideAd(bool destroy = false)
        {
            if (!Provider)
            {
                Debug.LogError("Ad provider not available. This can be due to ad not getting loaded or no provider can handle this ad type.");
                return;
            }
            if (!IsShowing)
            {
                Debug.LogError("Ad content is not displayed.");
                return;
            }

            Provider.HideBanner(Placement, destroy);
            SetContentStatus(ContentStatus.Hidden);
        }

        #endregion

        #region Setter methods

        private void SetPosition(AdPosition position)
        {
            if (Position != null) return;

            Position    = position;
        }

        private void SetRequestedSize(AdSize size)
        {
            if (RequestedSize != null) return;

            RequestedSize        = size;
        }

        private void SetDisplaySize(Vector2Int? size)
        {
            if (DisplaySize != null) return;

            DisplaySize        = size;
        }

        #endregion
    }
}