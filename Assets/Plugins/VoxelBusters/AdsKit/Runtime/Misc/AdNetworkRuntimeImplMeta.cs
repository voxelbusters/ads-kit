using System;
using System.Reflection;

namespace VoxelBusters.AdsKit.Adapters
{
    internal class AdNetworkRuntimeImplMeta
    {
        #region Properties

        public string Name { get; private set; }

        public string NetworkId { get; private set; }

        public Type ImplementationType { get; private set; }

        public MethodInfo CreateMethodInfo { get; private set; }

        #endregion

        #region Constructors

        public AdNetworkRuntimeImplMeta(string name,
                                        string networkId,
                                        Type implementationType,
                                        MethodInfo createMethodInfo)
        {
            // Set properties
            Name                        = name;
            NetworkId                   = networkId;
            ImplementationType          = implementationType;
            CreateMethodInfo            = createMethodInfo;
        }

        #endregion
    }
}
