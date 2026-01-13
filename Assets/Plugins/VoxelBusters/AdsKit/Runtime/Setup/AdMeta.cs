using System;
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
        private RuntimePlatformConstantSet      m_adUnitIds;

        [SerializeField]
        //[HideInInspector]
        private     RuntimePlatformConstantSet  m_testAdUnitIds; //Fill these ids based on the network id type and knowing placement meta details (banner options) at build time.

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

        public string GetAdUnitIdForPlatform(RuntimePlatform platform, bool isTestMode)
        {
            string adUnitId = null;

            if (isTestMode)
            {
                adUnitId = m_testAdUnitIds.GetConstantForPlatform(platform: platform, defaultValue: null);
            }

            if (adUnitId == null)
            {
                adUnitId = m_adUnitIds.GetConstantForPlatform(platform: platform, defaultValue: null);
            }

            return adUnitId;
        }

        public string GetAdUnitIdForActiveOrSimulationPlatform(bool isTestMode)
        {
            return GetAdUnitIdForPlatform(platform: ApplicationServices.GetActiveOrSimulationPlatform(), isTestMode: isTestMode);
        }

        public void SetTestAdUnitId(RuntimePlatformConstantSet testAdUnitIdSet)
        {
            m_testAdUnitIds = testAdUnitIdSet;
        }

        #endregion
    }
}