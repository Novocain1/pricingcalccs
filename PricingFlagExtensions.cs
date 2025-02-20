using System.Collections.Generic;

namespace PricingCalculator
{
    public static class PricingFlagExtensions
    {
        public static List<PricingOperation> GetOperations(this EnumPricingFlag flags)
        {
            var operations = new List<PricingOperation>();

            // Step 1: Percentage Adjustments (Priority 1)
            if (flags.HasFlag(EnumPricingFlag.Discount25Percent))
                operations.Add(new PricingOperation(EnumOperationType.Multiply, EnumOperationParameter.Param_0_75, EnumOperationPriority.Priority_1));
            else if (flags.HasFlag(EnumPricingFlag.Discount20Percent))
                operations.Add(new PricingOperation(EnumOperationType.Multiply, EnumOperationParameter.Param_0_80, EnumOperationPriority.Priority_1));
            else if (flags.HasFlag(EnumPricingFlag.Discount15Percent))
                operations.Add(new PricingOperation(EnumOperationType.Multiply, EnumOperationParameter.Param_0_85, EnumOperationPriority.Priority_1));
            else if (flags.HasFlag(EnumPricingFlag.Discount10Percent))
                operations.Add(new PricingOperation(EnumOperationType.Multiply, EnumOperationParameter.Param_0_90, EnumOperationPriority.Priority_1));
            else if (flags.HasFlag(EnumPricingFlag.Discount5Percent))
                operations.Add(new PricingOperation(EnumOperationType.Multiply, EnumOperationParameter.Param_0_95, EnumOperationPriority.Priority_1));
            else if (flags.HasFlag(EnumPricingFlag.Discount2Percent))
                operations.Add(new PricingOperation(EnumOperationType.Multiply, EnumOperationParameter.Param_0_98, EnumOperationPriority.Priority_1));

            if (flags.HasFlag(EnumPricingFlag.Increase25Percent))
                operations.Add(new PricingOperation(EnumOperationType.Multiply, EnumOperationParameter.Param_1_25, EnumOperationPriority.Priority_1));
            else if (flags.HasFlag(EnumPricingFlag.Increase20Percent))
                operations.Add(new PricingOperation(EnumOperationType.Multiply, EnumOperationParameter.Param_1_20, EnumOperationPriority.Priority_1));
            else if (flags.HasFlag(EnumPricingFlag.Increase15Percent))
                operations.Add(new PricingOperation(EnumOperationType.Multiply, EnumOperationParameter.Param_1_15, EnumOperationPriority.Priority_1));
            else if (flags.HasFlag(EnumPricingFlag.Increase10Percent))
                operations.Add(new PricingOperation(EnumOperationType.Multiply, EnumOperationParameter.Param_1_10, EnumOperationPriority.Priority_1));
            else if (flags.HasFlag(EnumPricingFlag.Increase5Percent))
                operations.Add(new PricingOperation(EnumOperationType.Multiply, EnumOperationParameter.Param_1_05, EnumOperationPriority.Priority_1));
            else if (flags.HasFlag(EnumPricingFlag.Increase1Percent))
                operations.Add(new PricingOperation(EnumOperationType.Multiply, EnumOperationParameter.Param_1_01, EnumOperationPriority.Priority_1));
            else if (flags.HasFlag(EnumPricingFlag.ReasonableIncrease))
                operations.Add(new PricingOperation(EnumOperationType.Custom, EnumOperationParameter.Param_Reasonable, EnumOperationPriority.Priority_1));

            // Step 2: Minimum Viable Price (Priority 2)
            if (flags.HasFlag(EnumPricingFlag.UseMinimumViable))
                operations.Add(new PricingOperation(EnumOperationType.Max, EnumOperationParameter.Param_UnitCost, EnumOperationPriority.Priority_2));

            // Step 3: Margin Multipliers (Priority 3)
            if (flags.HasFlag(EnumPricingFlag.DoubleMargin))
                operations.Add(new PricingOperation(EnumOperationType.Custom, EnumOperationParameter.None, EnumOperationPriority.Priority_3));
            else if (flags.HasFlag(EnumPricingFlag.TripleMargin))
                operations.Add(new PricingOperation(EnumOperationType.Custom, EnumOperationParameter.None, EnumOperationPriority.Priority_3));

            // Step 4: Competitive Pricing (Priority 4)
            if (flags.HasFlag(EnumPricingFlag.CompetitiveMatch))
                operations.Add(new PricingOperation(EnumOperationType.Min, EnumOperationParameter.Param_RetailPrice, EnumOperationPriority.Priority_4));

            if (flags.HasFlag(EnumPricingFlag.MaxPenetrationPrice))
                operations.Add(new PricingOperation(EnumOperationType.Custom, EnumOperationParameter.None, EnumOperationPriority.Priority_4));

            // Step 5: Premium Image Offset (Priority 5)
            if (flags.HasFlag(EnumPricingFlag.PremiumImageOffset))
                operations.Add(new PricingOperation(EnumOperationType.Multiply, EnumOperationParameter.Param_1_08, EnumOperationPriority.Priority_5));

            // Step 6: Seasonal Adjustments (Priority 6)
            if (flags.HasFlag(EnumPricingFlag.SeasonalAdjustment))
                operations.Add(new PricingOperation(EnumOperationType.Custom, EnumOperationParameter.Param_Seasonal, EnumOperationPriority.Priority_6));

            // Step 7: Bundle Pricing (Priority 7)
            if (flags.HasFlag(EnumPricingFlag.BundlePricing))
                operations.Add(new PricingOperation(EnumOperationType.Multiply, EnumOperationParameter.Param_0_97, EnumOperationPriority.Priority_7));

            // Step 8: Key Price Point Targeting (Priority 8)
            if (flags.HasFlag(EnumPricingFlag.KeyPricePoint))
                operations.Add(new PricingOperation(EnumOperationType.Custom, EnumOperationParameter.Param_KeyPricePoint, EnumOperationPriority.Priority_8));

            // Step 9: Floor Price (Priority 9)
            if (flags.HasFlag(EnumPricingFlag.FloorPrice))
                operations.Add(new PricingOperation(EnumOperationType.Floor, EnumOperationParameter.None, EnumOperationPriority.Priority_9));

            // Step 10: Cent Adjustments (Priority 10)
            if (flags.HasFlag(EnumPricingFlag.Add99Cents))
                operations.Add(new PricingOperation(EnumOperationType.Custom, EnumOperationParameter.Param_Cents_99, EnumOperationPriority.Priority_10));
            else if (flags.HasFlag(EnumPricingFlag.Add50Cents))
                operations.Add(new PricingOperation(EnumOperationType.Custom, EnumOperationParameter.Param_Cents_50, EnumOperationPriority.Priority_10));
            else if (flags.HasFlag(EnumPricingFlag.RoundTo99Cents))
                operations.Add(new PricingOperation(EnumOperationType.Custom, EnumOperationParameter.Param_Cents_99, EnumOperationPriority.Priority_10));
            else if (flags.HasFlag(EnumPricingFlag.RoundTo95Cents))
                operations.Add(new PricingOperation(EnumOperationType.Custom, EnumOperationParameter.Param_Cents_95, EnumOperationPriority.Priority_10));
            else if (flags.HasFlag(EnumPricingFlag.RoundTo75Cents))
                operations.Add(new PricingOperation(EnumOperationType.Custom, EnumOperationParameter.Param_Cents_75, EnumOperationPriority.Priority_10));
            else if (flags.HasFlag(EnumPricingFlag.RoundTo49Cents))
                operations.Add(new PricingOperation(EnumOperationType.Custom, EnumOperationParameter.Param_Cents_49, EnumOperationPriority.Priority_10));
            else if (flags.HasFlag(EnumPricingFlag.RoundTo25Cents))
                operations.Add(new PricingOperation(EnumOperationType.Custom, EnumOperationParameter.Param_Cents_25, EnumOperationPriority.Priority_10));

            if (flags.HasFlag(EnumPricingFlag.PsychologicalPricing))
                operations.Add(new PricingOperation(EnumOperationType.Custom, EnumOperationParameter.Param_Psychological, EnumOperationPriority.Priority_10));

            // Step 11: Ceiling Price (Priority 11)
            if (flags.HasFlag(EnumPricingFlag.CeilingPrice))
                operations.Add(new PricingOperation(EnumOperationType.Ceiling, EnumOperationParameter.None, EnumOperationPriority.Priority_11));

            return operations;
        }
    }
}