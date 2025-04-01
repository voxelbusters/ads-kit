using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelBusters.CoreLibrary;

namespace VoxelBusters.AdsKit
{
    /// <summary>
    /// Load ad request.
    /// </summary>
    [IncludeInDocs]
    public class LoadAdRequest : AsyncOperationHandle<LoadAdResult>
    {
        #region Properties

        internal new LoadAdOperation InternalOp => InternalOp;

        public string PlacementId => InternalOp.Placement;

        #endregion

        #region Constructors

        internal LoadAdRequest(LoadAdOperation internalOp)
            : base(internalOp)
        { }

        #endregion

        #region Public methods

        /// <summary>
        /// Returns ad content associated with the request.
        /// </summary>
        /// <returns>The ad content.</returns>
        public AdContent GetAdContent()
        {
            Assert.IsTrue(IsDone, "Request is not completed.");

            if (Error == null)
            {
                var     manager     = InternalOp.Manager;
                var     provider    = manager.GetAdNetworkWithId(InternalOp.Result.PreferredAdProvider);
                return manager.CreateAdContent(adType: InternalOp.AdType,
                                               placement: InternalOp.Placement,
                                               provider: provider);
            }
            return null;
        }

        #endregion
    }
}