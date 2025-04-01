namespace VoxelBusters.AdsKit
{
    public class AdReward
    {
        public string AdUnitId { get; private set; }

        public AdType AdType { get; private set; }

        public string Placement { get; private set; }

        public string NetworkId { get; private set; }

        public double Amount { get; private set; }

        public string Type { get; private set; }


        #region Constructors

        public AdReward(string adUnitId,
                        AdType adType,
                        string placement,
                        string networkId,
                        double amount,
                        string type)
        {
            // Set properties
            AdUnitId    = adUnitId;
            AdType      = adType;
            Placement   = placement;
            NetworkId   = networkId;
            Amount      = amount;
            Type        = type;
        }

        #endregion

        public bool IsRewardAmountAvailable()
        {
            return Amount != -1;
        }

        public override string ToString()
        {
            return $"[AdUnitId: {AdUnitId}, AdType: {AdType}, Placement: {Placement}, NetworkId: {NetworkId}, Amount: {Amount}, Type: {Type}]";
        }
    }
}