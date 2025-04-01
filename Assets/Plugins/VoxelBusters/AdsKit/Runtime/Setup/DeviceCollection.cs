using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelBusters.AdsKit
{
    [System.Serializable]
    public class DeviceCollection
    {
        #region Constants

        private     static readonly string[] kEmptyIdArray   = new string[0];

        #endregion

        #region Fields

        [SerializeField]
        private     string[]    m_iosIds;

        [SerializeField]
        private     string[]    m_androidIds;

        #endregion

        #region Properties

        public string[] IosIds
        {
            get => m_iosIds;
            set => m_iosIds = value;
        }

        public string[] AndroidIds
        {
            get => m_androidIds;
            set => m_androidIds    = value;
        }

        #endregion

        #region Constructors

        public DeviceCollection(string[] iosIds = null,
                                string[] androidIds = null)
        {
            // Set properties
            IosIds      = iosIds ?? kEmptyIdArray;
            AndroidIds  = androidIds ?? kEmptyIdArray;
        }

        #endregion

        #region Public methods

        public string[] GetDeviceIdsForPlatform(RuntimePlatform platform)
        {
            switch (platform)
            {
                case RuntimePlatform.IPhonePlayer:
                    return IosIds;

                case RuntimePlatform.Android:
                    return AndroidIds;

                default:
                    return kEmptyIdArray;
            }
        }

        #endregion
    }
}