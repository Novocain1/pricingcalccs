using System;

namespace PricingCalculator
{
    [Flags]
    public enum EnumOperationParameter
    {
        None = 0,
                                 // Discount parameters
        Param_0_99 = 1 << 0,           // 0.99
        Param_0_98 = 1 << 1,           // 0.98
        Param_0_97 = 1 << 2,           // 0.97
        Param_0_95 = 1 << 3,           // 0.95
        Param_0_90 = 1 << 4,           // 0.90
        Param_0_85 = 1 << 5,           // 0.85
        Param_0_80 = 1 << 6,           // 0.80
        Param_0_75 = 1 << 7,           // 0.75
                                                                
                                 // Increase parameters
        Param_1_01 = 1 << 8,           // 1.01
        Param_1_02 = 1 << 9,           // 1.02
        Param_1_03 = 1 << 10,          // 1.03
        Param_1_05 = 1 << 11,          // 1.05
        Param_1_08 = 1 << 12,          // 1.08
        Param_1_10 = 1 << 13,          // 1.10
        Param_1_15 = 1 << 14,          // 1.15
        Param_1_20 = 1 << 15,          // 1.20
        Param_1_25 = 1 << 16,          // 1.25
                                                        
                                 // Cents parameters
        Param_Cents_25 = 1 << 17,      // 0.25
        Param_Cents_49 = 1 << 18,      // 0.49
        Param_Cents_50 = 1 << 19,      // 0.50
        Param_Cents_69 = 1 << 20,      // 0.69
        Param_Cents_75 = 1 << 21,      // 0.75
        Param_Cents_79 = 1 << 22,      // 0.79
        Param_Cents_89 = 1 << 23,      // 0.89
        Param_Cents_95 = 1 << 24,      // 0.95
        Param_Cents_99 = 1 << 25,      // 0.99
                                                                    
                                 // Special parameters
        Param_UnitCost = 1 << 26,      // For minimum viable calculations
        Param_RetailPrice = 1 << 27,   // For competitive calculations
        Param_Reasonable = 1 << 28,    // For reasonable increase
        Param_Seasonal = 1 << 29,      // For seasonal adjustments
        Param_KeyPricePoint = 1 << 30, // For key price point targeting
        Param_Psychological = 1 << 31  // For psychological pricing
    }
}