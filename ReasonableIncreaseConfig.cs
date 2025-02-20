namespace PricingCalculator
{
    public class ReasonableIncreaseConfig
    {
        public decimal BaseIncrease { get; set; }
        public decimal MaxIncrease { get; set; }

        public decimal LowPriceThreshold { get; set; }
        public decimal MidPriceThreshold { get; set; }
        public decimal HighPriceThreshold { get; set; }

        public decimal LowPriceAdditional { get; set; }
        public decimal MidPriceAdditional { get; set; }
        public decimal HighPriceAdditional { get; set; }

        public bool UseInterpolation { get; set; }
    }
}