using System;
using VoxelBusters.CoreLibrary;

namespace VoxelBusters.AdsKit.Adapters
{
    public class AdNetworkInitialiseStateInfo
    {
        #region Properties

        public string NetworkId { get; private set; }

        public AdNetworkInitialiseStatus Status { get; internal set; }

        public Error Error { get; internal set; }

        #endregion

        #region Constructors

        public AdNetworkInitialiseStateInfo(string networkId,
                                            AdNetworkInitialiseStatus status = 0,
                                            Error error = null)
        {
            // Set properties
            NetworkId           = networkId;
            Status              = status;
            Error               = error;
        }

        #endregion
    }
}
