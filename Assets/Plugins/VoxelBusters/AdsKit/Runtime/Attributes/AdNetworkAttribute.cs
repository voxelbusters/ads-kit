using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelBusters.AdsKit
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AdNetworkAttribute : Attribute
    {
        #region Properties

        public string Name { get; private set; }

        public string NetworkId { get; private set; }

        #endregion

        #region Constructors

        public AdNetworkAttribute(string name)
            : this(name, name)
        { }

        public AdNetworkAttribute(string name, string networkId)
        {
            // Set properties
            Name        = name;
            NetworkId   = networkId;
        }

        #endregion
    }
}
