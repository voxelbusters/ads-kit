using System.Collections;
using System.Collections.Generic;

namespace VoxelBusters.AdsKit.Editor
{
    public abstract class AdNetworkAssetInfo
    {
        #region Properties

        public string NetworkId { get;  private set; }

        public string Name { get;  private set; }

        public string Description { get;  private set; }

        public string[] ImportPaths { get; private set; }

        public string[] InstallPaths { get; private set; }

        #endregion

        #region Constructors

        protected AdNetworkAssetInfo(string networkId,
                                     string name,
                                     string description,
                                     string[] importPaths,
                                     string[] installPaths)
        {
            // Set properties
            NetworkId       = networkId;
            Name            = name;
            Description     = description;
            ImportPaths     = importPaths;
            InstallPaths    = installPaths;
        }

        #endregion

        #region Callback methods

        public virtual void OnInstall()
        { }

        public virtual void OnUninstall()
        { }

        #endregion
    }
}