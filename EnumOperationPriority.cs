using System;

namespace PricingCalculator
{
    [Flags]
    public enum EnumOperationPriority
    {
        None = 0,
        Priority_1 = 1 << 0,  // First operations (base price adjustments)
        Priority_2 = 1 << 1,  // Second operations (min viable price)
        Priority_3 = 1 << 2,  // Third operations (margin multipliers)
        Priority_4 = 1 << 3,  // Fourth operations (competitive pricing)
        Priority_5 = 1 << 4,  // Fifth operations (premium image)
        Priority_6 = 1 << 5,  // Sixth operations (seasonal adjustments)
        Priority_7 = 1 << 6,  // Seventh operations (bundle pricing)
        Priority_8 = 1 << 7,  // Eighth operations (key price points)
        Priority_9 = 1 << 8,  // Ninth operations (floor price)
        Priority_10 = 1 << 9, // Tenth operations (cent adjustments)
        Priority_11 = 1 << 10 // Final operations (ceiling price)
    }
}