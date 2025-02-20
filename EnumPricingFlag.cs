using System;

namespace PricingCalculator
{
    [Flags]
    public enum EnumPricingFlag : long
    {
        None = 0,

        // Basic discount operations
        Discount2Percent = 1L << 0,
        Discount5Percent = 1L << 1,
        Discount10Percent = 1L << 2,
        Discount15Percent = 1L << 3,
        Discount20Percent = 1L << 4,
        Discount25Percent = 1L << 5,

        // Basic increase operations
        Increase1Percent = 1L << 6,
        Increase5Percent = 1L << 7,
        Increase10Percent = 1L << 8,
        Increase15Percent = 1L << 9,
        Increase20Percent = 1L << 10,
        Increase25Percent = 1L << 11,

        // Minimum viable price
        UseMinimumViable = 1L << 12,

        // Margin operations
        DoubleMargin = 1L << 13,
        TripleMargin = 1L << 14,

        // Competitive pricing
        CompetitiveMatch = 1L << 15,
        MaxPenetrationPrice = 1L << 16,

        // Premium image
        PremiumImageOffset = 1L << 17,

        // Seasonal adjustments
        SeasonalAdjustment = 1L << 18,

        // Bundle pricing
        BundlePricing = 1L << 19,

        // Key price points
        KeyPricePoint = 1L << 20,

        // Rounding operations
        FloorPrice = 1L << 21,
        CeilingPrice = 1L << 22,

        // Cent adjustments
        Add50Cents = 1L << 23,
        Add99Cents = 1L << 24,
        RoundTo25Cents = 1L << 25,
        RoundTo49Cents = 1L << 26,
        RoundTo75Cents = 1L << 27,
        RoundTo95Cents = 1L << 28,
        RoundTo99Cents = 1L << 29,

        // Special pricing
        PsychologicalPricing = 1L << 30,
        ReasonableIncrease = 1L << 31
    }
}