using System;

namespace PricingCalculator
{
    [Flags]
    public enum EnumOperationType
    {
        None = 0,
        Multiply = 1 << 0,
        Add = 1 << 1,
        Max = 1 << 2,
        Min = 1 << 3,
        Floor = 1 << 4,
        Ceiling = 1 << 5,
        Round = 1 << 6,
        Custom = 1 << 7  // For operations that require custom logic
    }
}