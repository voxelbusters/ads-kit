using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelBusters.AdsKit.Adapters;
using VoxelBusters.CoreLibrary;

namespace VoxelBusters.AdsKit
{
    /// <summary>
    /// The object represents initialise operation result.
    /// </summary>
    [IncludeInDocs]
    public class InitialiseResult
    {
        #region Properties

        public AdNetworkAdapter[] SubscribedAdNetworks { get; private set; }

        public string[] InvalidNetworks { get; private set; }

        #endregion

        #region Constructors

        public InitialiseResult(AdNetworkAdapter[] subscribedAdNetworks,
                                string[] invalidAdNetworks)
        {
            // Set properties
            SubscribedAdNetworks    = subscribedAdNetworks;
            InvalidNetworks         = invalidAdNetworks;
        }

        #endregion
    }
}