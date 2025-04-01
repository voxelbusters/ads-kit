using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using VoxelBusters.AdsKit.Adapters;
using VoxelBusters.CoreLibrary;

namespace VoxelBusters.AdsKit
{
    internal static class Helpers
    {
        #region Static fields

        private     static      AdNetworkRuntimeImplMeta[]      s_cachedRuntimeImplMetaArray;

        #endregion

        #region Find methods

        public static AdNetworkAdapter FindAdNetwork(AdNetworkAdapter[] array, string networkId)
        {
            return Array.Find(array, (item) => string.Equals(item.NetworkId, networkId));
        }

        public static bool TryFindRuntimeImpl(AdNetworkRuntimeImplMeta[] array, string networkId, out AdNetworkRuntimeImplMeta meta)
        {
            meta  = Array.Find(array, (item) => string.Equals(item.NetworkId, networkId));
            return (meta != null);
        }

        public static AdNetworkRuntimeImplMeta[] FindAllRuntimeImplMeta()
        {
            // Extract required data using reflection
            if (s_cachedRuntimeImplMetaArray == null)
            {
                var     attributeCollection     = ReflectionUtility.FindTypesWithAttribute(typeof(AdNetworkAttribute));
                var     bindingAttr             = BindingFlags.Public | BindingFlags.Static;
                var     metaList                = new List<AdNetworkRuntimeImplMeta>();
                foreach (var entry in attributeCollection)
                {
                    var     networkAttr         = entry.Value[0] as AdNetworkAttribute;
                    var     networkName         = networkAttr.Name;
                    var     networkId           = networkAttr.NetworkId;
                    var     implType            = entry.Key;
                    var     createMethodInfo    = default(MethodInfo);

                    foreach (var memberInfo in implType.FindMembers(MemberTypes.Method, bindingAttr, null, null))
                    {
                        var     methodInfo      = memberInfo as MethodInfo;
                        if (methodInfo.TryGetCustomAttriute(attribute: out AdNetworkCreateMethodAttribute createAttr))
                        {
                            createMethodInfo    = methodInfo;
                        }
                    }

                    // Add new entry
                    var     newEntry            = new AdNetworkRuntimeImplMeta(name: networkName,
                                                                               networkId: networkId,
                                                                               implementationType: implType,
                                                                               createMethodInfo: createMethodInfo);
                    metaList.Add(newEntry);
                }
                s_cachedRuntimeImplMetaArray    = metaList.ToArray();
            }
            return s_cachedRuntimeImplMetaArray;
        }

        #endregion
    }
}