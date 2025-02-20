namespace PricingCalculator
{
    public enum EnumPricingStrategy : long
    {
        None = 0,
        // Original strategies
        Balanced = EnumPricingFlag.Discount2Percent | EnumPricingFlag.UseMinimumViable,
        Aggressive = EnumPricingFlag.Discount5Percent | EnumPricingFlag.UseMinimumViable,
        Premium = EnumPricingFlag.Increase10Percent | EnumPricingFlag.UseMinimumViable,
        DollarStore = EnumPricingFlag.CeilingPrice | EnumPricingFlag.UseMinimumViable,
        Psychological50 = EnumPricingFlag.Discount2Percent | EnumPricingFlag.UseMinimumViable | EnumPricingFlag.FloorPrice | EnumPricingFlag.Add50Cents,
        Psychological99 = EnumPricingFlag.Discount2Percent | EnumPricingFlag.UseMinimumViable | EnumPricingFlag.FloorPrice | EnumPricingFlag.Add99Cents,
        Budget = EnumPricingFlag.Discount5Percent | EnumPricingFlag.UseMinimumViable | EnumPricingFlag.FloorPrice,
        ValuePlus = EnumPricingFlag.Discount2Percent | EnumPricingFlag.UseMinimumViable | EnumPricingFlag.FloorPrice | EnumPricingFlag.Add50Cents,
        PremiumPlus = EnumPricingFlag.Increase10Percent | EnumPricingFlag.UseMinimumViable | EnumPricingFlag.CeilingPrice,
        Clearance = EnumPricingFlag.Discount5Percent | EnumPricingFlag.FloorPrice,
        CompetitiveEdge = EnumPricingFlag.Discount2Percent | EnumPricingFlag.UseMinimumViable | EnumPricingFlag.FloorPrice,
        LuxuryTier = EnumPricingFlag.Increase10Percent | EnumPricingFlag.UseMinimumViable | EnumPricingFlag.FloorPrice | EnumPricingFlag.Add99Cents,
        MarketLeader = EnumPricingFlag.Increase10Percent | EnumPricingFlag.UseMinimumViable | EnumPricingFlag.Add50Cents,
        EndOfLine = EnumPricingFlag.Discount5Percent | EnumPricingFlag.UseMinimumViable | EnumPricingFlag.FloorPrice | EnumPricingFlag.Add99Cents,
        BulkPricing = EnumPricingFlag.Discount2Percent | EnumPricingFlag.UseMinimumViable | EnumPricingFlag.CeilingPrice,
        SeasonalPromo = EnumPricingFlag.Discount5Percent | EnumPricingFlag.UseMinimumViable | EnumPricingFlag.FloorPrice | EnumPricingFlag.Add50Cents,
        NewProduct = EnumPricingFlag.Increase10Percent | EnumPricingFlag.UseMinimumViable | EnumPricingFlag.FloorPrice | EnumPricingFlag.Add99Cents,
        QuickSale = EnumPricingFlag.Discount5Percent | EnumPricingFlag.UseMinimumViable | EnumPricingFlag.FloorPrice | EnumPricingFlag.Add50Cents,

        // New strategies - Phase 1
        DeepDiscount = EnumPricingFlag.Discount10Percent | EnumPricingFlag.UseMinimumViable | EnumPricingFlag.RoundTo95Cents,
        UltraPremium = EnumPricingFlag.Increase20Percent | EnumPricingFlag.UseMinimumViable | EnumPricingFlag.CeilingPrice | EnumPricingFlag.Add99Cents,
        EconomyPlus = EnumPricingFlag.Discount5Percent | EnumPricingFlag.CompetitiveMatch | EnumPricingFlag.RoundTo49Cents,
        MidtierValue = EnumPricingFlag.Increase5Percent | EnumPricingFlag.UseMinimumViable | EnumPricingFlag.RoundTo95Cents,
        BlackFriday = EnumPricingFlag.Discount10Percent | EnumPricingFlag.FloorPrice | EnumPricingFlag.Add99Cents,
        HighMargin = EnumPricingFlag.Increase15Percent | EnumPricingFlag.DoubleMargin | EnumPricingFlag.CeilingPrice,
        BundleDeal = EnumPricingFlag.Discount5Percent | EnumPricingFlag.BundlePricing | EnumPricingFlag.RoundTo95Cents,
        CompetitiveDestroyer = EnumPricingFlag.Discount10Percent | EnumPricingFlag.CompetitiveMatch | EnumPricingFlag.FloorPrice,
        PremiumExperience = EnumPricingFlag.Increase15Percent | EnumPricingFlag.UseMinimumViable | EnumPricingFlag.Add99Cents,
        DynamicMarket = EnumPricingFlag.CompetitiveMatch | EnumPricingFlag.UseMinimumViable | EnumPricingFlag.RoundTo49Cents,

        // New strategies - Phase 2
        CyberMonday = EnumPricingFlag.Discount15Percent | EnumPricingFlag.UseMinimumViable | EnumPricingFlag.KeyPricePoint,
        LastChance = EnumPricingFlag.Discount20Percent | EnumPricingFlag.FloorPrice | EnumPricingFlag.Add99Cents,
        MembershipTier = EnumPricingFlag.Discount10Percent | EnumPricingFlag.RoundTo95Cents | EnumPricingFlag.UseMinimumViable,
        EliteStatus = EnumPricingFlag.Increase15Percent | EnumPricingFlag.PremiumImageOffset | EnumPricingFlag.Add99Cents,
        BusinessClass = EnumPricingFlag.Increase20Percent | EnumPricingFlag.DoubleMargin | EnumPricingFlag.RoundTo95Cents,
        FirstClass = EnumPricingFlag.Increase25Percent | EnumPricingFlag.TripleMargin | EnumPricingFlag.KeyPricePoint,
        FlashSale = EnumPricingFlag.Discount15Percent | EnumPricingFlag.RoundTo49Cents | EnumPricingFlag.UseMinimumViable,
        WarehouseClearance = EnumPricingFlag.Discount25Percent | EnumPricingFlag.MaxPenetrationPrice | EnumPricingFlag.RoundTo99Cents,
        HolidaySpecial = EnumPricingFlag.Discount10Percent | EnumPricingFlag.SeasonalAdjustment | EnumPricingFlag.KeyPricePoint,
        SummerSale = EnumPricingFlag.Discount15Percent | EnumPricingFlag.SeasonalAdjustment | EnumPricingFlag.RoundTo49Cents,
        WinterPromo = EnumPricingFlag.Discount5Percent | EnumPricingFlag.SeasonalAdjustment | EnumPricingFlag.RoundTo95Cents,
        SpringCollection = EnumPricingFlag.Increase10Percent | EnumPricingFlag.SeasonalAdjustment | EnumPricingFlag.PremiumImageOffset,
        FallLineup = EnumPricingFlag.Increase5Percent | EnumPricingFlag.SeasonalAdjustment | EnumPricingFlag.Add99Cents,
        VIPCustomer = EnumPricingFlag.Discount5Percent | EnumPricingFlag.PremiumImageOffset | EnumPricingFlag.RoundTo95Cents,
        MarketEntry = EnumPricingFlag.Discount10Percent | EnumPricingFlag.MaxPenetrationPrice | EnumPricingFlag.KeyPricePoint,
        ExclusiveItem = EnumPricingFlag.Increase20Percent | EnumPricingFlag.PremiumImageOffset | EnumPricingFlag.RoundTo95Cents,
        SmallPremium = EnumPricingFlag.Increase5Percent,
        VerySmallPremium = EnumPricingFlag.Increase1Percent,
        MarketRegulator = EnumPricingFlag.ReasonableIncrease,
        MarketRegulatorPsychological = EnumPricingFlag.ReasonableIncrease | EnumPricingFlag.PsychologicalPricing,
        
    }
}