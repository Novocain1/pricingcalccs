using System.Collections.Generic;

namespace PricingCalculator
{
    public class SaveData
    {
        public List<PricingItem> Items { get; set; }
        public bool DollarStore { get; set; }
        public EnumPricingStrategy GlobalStrategy { get; set; }
        public EnumSeason CurrentSeason { get; set; }
        public ReasonableIncreaseConfig ReasonableConfig { get; set; }
    }
}