using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelBusters.CoreLibrary;

namespace VoxelBusters.AdsKit
{
    public class AdNetworkIdAttribute : StringPopupAttribute
    {
        #region Static fields

        public static string[] s_options = new string[0];

        #endregion

        #region Static methods

        public static void SetOptions(string[] options)
        {
            Assert.IsArgNotNull(options, nameof(options));

            s_options   = options;
        }

        #endregion

        #region Base class method implementation

        protected override string[] GetDynamicOptions() => s_options;

        #endregion
    }
}