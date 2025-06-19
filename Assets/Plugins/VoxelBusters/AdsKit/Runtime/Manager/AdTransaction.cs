using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelBusters.CoreLibrary;

namespace VoxelBusters.AdsKit
{
    /// <summary>
    /// The transaction object.
    /// </summary>
    [IncludeInDocs]
    public class AdTransaction
    {
        #region Properties

        public string AdUnitId { get; private set; }

        public AdType AdType { get; private set; }

        public string Placement { get; private set; }

        public string NetworkId { get; private set; }

        public string CurrencyCode { get; private set; }

        public double Value { get; private set; }

        public PrecisionType Precision { get; private set; }

        #endregion

        #region Constructors

        public AdTransaction(string adUnitId,
                             AdType adType,
                             string placement,
                             string networkId,
                             string currencyCode,
                             double value,
                             PrecisionType precision)
        {
            // Set properties
            AdUnitId        = adUnitId;
            AdType          = adType;
            Placement       = placement;
            NetworkId       = networkId;
            CurrencyCode    = currencyCode;
            Value           = value;
            Precision       = precision;
        }

        #endregion

        #region Base class methods

        public override string ToString()
        {
            return $"(AdUnitId:{AdUnitId}, " +
                $"AdType:{AdType}, " +
                $"Placement:{Placement}, " +
                $"NetworkId:{NetworkId}, " +
                $"CurrencyCode:{CurrencyCode}, " +
                $"Value:{Value}, " +
                $"Precision: {Precision})";
        }

        #endregion

        #region Nested types

        public enum PrecisionType
        {
            Unknown = 0,

            Estimated,

            Precise,

            PublisherProvided,
        }

        #endregion
    }
}