namespace PricingCalculator
{
    public class PriceCalculation
    {
        public decimal RecommendedPrice { get; set; }
        public decimal MarginPercent { get; set; }
        public decimal PriceRelativeToMarket { get; set; }
        public EnumPricingStrategy StrategyUsed { get; set; }
    }
}