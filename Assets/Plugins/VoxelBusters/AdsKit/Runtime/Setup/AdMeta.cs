using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using VoxelBusters.CoreLibrary;

namespace VoxelBusters.AdsKit
{
    [System.Serializable]
    public class AdMeta
    {
        #region Fields

        [SerializeField, AdPlacement]
        private     string                      m_placement;

        [SerializeField, FormerlySerializedAs("m_placementIdOverrides")]
        private     RuntimePlatformConstantSet  m_adUnitIds;

        #endregion

        #region Properties

        public string Placement
        {
            get => m_placement;
            private set => m_placement    = value;
        }

        #endregion

        #region Constructors

        public AdMeta(string placement,
                      RuntimePlatformConstantSet adUnitIds)
        {
            // Set properties
            Placement       = placement;
            m_adUnitIds     = adUnitIds;
        }

        #endregion

        #region Public methods

        public string GetAdUnitIdForPlatform(RuntimePlatform platform)
        {
            return m_adUnitIds.GetConstantForPlatform(platform: platform, defaultValue: null);
        }

        public string GetAdUnitIdForActiveOrSimulationPlatform()
        {
            return GetAdUnitIdForPlatform(platform: ApplicationServices.GetActiveOrSimulationPlatform());
        }

        #endregion
    }
}