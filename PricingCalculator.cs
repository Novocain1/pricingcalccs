using System;
using System.Collections.Generic;
using System.Linq;

namespace PricingCalculator
{

    public class PricingCalculator
    {
        public decimal MinMarginPercent { get; set; } = 20;
        public EnumSeason CurrentSeason { get; set; }
        public bool DollarStore { get; set; }

        private Dictionary<EnumPricingFlag, List<PricingOperation>> _operationsCache = new Dictionary<EnumPricingFlag, List<PricingOperation>>();

        public ReasonableIncreaseConfig ReasonableConfig { get; set; } = new ReasonableIncreaseConfig
        {
            BaseIncrease = 0.02m,
            MaxIncrease = 0.10m,
            LowPriceThreshold = 10m,
            MidPriceThreshold = 50m,
            HighPriceThreshold = 100m,
            LowPriceAdditional = 0.06m,
            MidPriceAdditional = 0.03m,
            HighPriceAdditional = 0.01m,
            UseInterpolation = true
        };

        public PricingCalculator(EnumSeason season, bool dollarStore)
        {
            CurrentSeason = season;
            DollarStore = dollarStore;
        }

        public decimal CalculatePrice(PricingItem item, EnumPricingFlag flags)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            if (item.UnitCost <= 0 || item.RetailPrice <= 0)
                throw new ArgumentException("Unit cost and retail price must be greater than zero");

            try
            {
                // Get initial price
                decimal result = item.RetailPrice;

                // Get operations from cache or create and cache them
                List<PricingOperation> operations;
                if (!_operationsCache.TryGetValue(flags, out operations))
                {
                    operations = flags.GetOperations();
                    _operationsCache[flags] = operations;
                }

                // Process operations in order of priority
                foreach (var operation in operations.OrderBy(o => o.Priority))
                {
                    result = ApplyOperation(operation, item, result);
                }

                // Apply dollar store rounding if enabled (global flag)
                if (DollarStore)
                {
                    result = Math.Ceiling(result);
                }

                // Ensure price is non-negative
                return Math.Max(0, result);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in CalculatePrice: {ex.Message}");
                throw;
            }
        }

        private decimal ApplyOperation(PricingOperation operation, PricingItem item, decimal currentPrice)
        {
            decimal result = currentPrice;
            decimal minViable = item.UnitCost * (1 + MinMarginPercent / 100);
            decimal competitivePrice = item.RetailPrice * 0.97m;
            decimal wholePart = Math.Floor(result);
            decimal fraction = result - wholePart;

            switch (operation.Type)
            {
                case EnumOperationType.Multiply:
                    result *= GetMultiplierValue(operation.Parameter);
                    break;

                case EnumOperationType.Add:
                    result += GetAddValue(operation.Parameter);
                    break;

                case EnumOperationType.Max:
                    if (operation.Parameter == EnumOperationParameter.Param_UnitCost)
                    {
                        result = Math.Max(result, minViable);
                    }
                    break;

                case EnumOperationType.Min:
                    if (operation.Parameter == EnumOperationParameter.Param_RetailPrice)
                    {
                        result = Math.Min(result, competitivePrice);
                    }
                    break;

                case EnumOperationType.Floor:
                    result = Math.Floor(result);
                    break;

                case EnumOperationType.Ceiling:
                    result = Math.Ceiling(result);
                    break;

                case EnumOperationType.Round:
                    result = Math.Round(result, 2);
                    break;

                case EnumOperationType.Custom:
                    result = ApplyCustomOperation(operation.Parameter, item, result, wholePart, fraction);
                    break;
            }

            return result;
        }

        private decimal GetMultiplierValue(EnumOperationParameter parameter)
        {
            switch (parameter)
            {
                case EnumOperationParameter.Param_0_75: return 0.75m;
                case EnumOperationParameter.Param_0_80: return 0.80m;
                case EnumOperationParameter.Param_0_85: return 0.85m;
                case EnumOperationParameter.Param_0_90: return 0.90m;
                case EnumOperationParameter.Param_0_95: return 0.95m;
                case EnumOperationParameter.Param_0_97: return 0.97m;
                case EnumOperationParameter.Param_0_98: return 0.98m;
                case EnumOperationParameter.Param_0_99: return 0.99m;
                case EnumOperationParameter.Param_1_01: return 1.01m;
                case EnumOperationParameter.Param_1_02: return 1.02m;
                case EnumOperationParameter.Param_1_03: return 1.03m;
                case EnumOperationParameter.Param_1_05: return 1.05m;
                case EnumOperationParameter.Param_1_08: return 1.08m;
                case EnumOperationParameter.Param_1_10: return 1.10m;
                case EnumOperationParameter.Param_1_15: return 1.15m;
                case EnumOperationParameter.Param_1_20: return 1.20m;
                case EnumOperationParameter.Param_1_25: return 1.25m;
                default: return 1.0m;
            }
        }

        private decimal GetAddValue(EnumOperationParameter parameter)
        {
            switch (parameter)
            {
                case EnumOperationParameter.Param_Cents_25: return 0.25m;
                case EnumOperationParameter.Param_Cents_49: return 0.49m;
                case EnumOperationParameter.Param_Cents_50: return 0.50m;
                case EnumOperationParameter.Param_Cents_69: return 0.69m;
                case EnumOperationParameter.Param_Cents_75: return 0.75m;
                case EnumOperationParameter.Param_Cents_79: return 0.79m;
                case EnumOperationParameter.Param_Cents_89: return 0.89m;
                case EnumOperationParameter.Param_Cents_95: return 0.95m;
                case EnumOperationParameter.Param_Cents_99: return 0.99m;
                default: return 0.0m;
            }
        }

        private decimal ApplyCustomOperation(EnumOperationParameter parameter, PricingItem item, decimal currentPrice, decimal wholePart, decimal fraction)
        {
            decimal result = currentPrice;

            switch (parameter)
            {
                case EnumOperationParameter.Param_Reasonable:
                    // Use the cached ReasonableConfig for calculations
                    decimal pricePoint = item.RetailPrice;
                    decimal additionalIncrease = 0;

                    if (ReasonableConfig.UseInterpolation)
                    {
                        // Smoother interpolation between price tiers
                        if (pricePoint <= ReasonableConfig.LowPriceThreshold)
                        {
                            // For very low prices, use the full low price additional increase
                            additionalIncrease = ReasonableConfig.LowPriceAdditional;
                        }
                        else if (pricePoint <= ReasonableConfig.MidPriceThreshold)
                        {
                            // Interpolate between low and mid price additional
                            decimal ratio = (pricePoint - ReasonableConfig.LowPriceThreshold) /
                                           (ReasonableConfig.MidPriceThreshold - ReasonableConfig.LowPriceThreshold);
                            additionalIncrease = ReasonableConfig.LowPriceAdditional -
                                                ratio * (ReasonableConfig.LowPriceAdditional - ReasonableConfig.MidPriceAdditional);
                        }
                        else if (pricePoint <= ReasonableConfig.HighPriceThreshold)
                        {
                            // Interpolate between mid and high price additional
                            decimal ratio = (pricePoint - ReasonableConfig.MidPriceThreshold) /
                                           (ReasonableConfig.HighPriceThreshold - ReasonableConfig.MidPriceThreshold);
                            additionalIncrease = ReasonableConfig.MidPriceAdditional -
                                                ratio * (ReasonableConfig.MidPriceAdditional - ReasonableConfig.HighPriceAdditional);
                        }
                        else
                        {
                            // For very high prices, use the high price additional increase
                            additionalIncrease = ReasonableConfig.HighPriceAdditional;
                        }
                    }
                    else
                    {
                        // Discrete tier approach (no interpolation)
                        if (pricePoint <= ReasonableConfig.LowPriceThreshold)
                            additionalIncrease = ReasonableConfig.LowPriceAdditional;
                        else if (pricePoint <= ReasonableConfig.MidPriceThreshold)
                            additionalIncrease = ReasonableConfig.MidPriceAdditional;
                        else if (pricePoint <= ReasonableConfig.HighPriceThreshold)
                            additionalIncrease = ReasonableConfig.HighPriceAdditional;
                        // For prices above HighPriceThreshold, only the base increase applies
                    }

                    decimal totalIncrease = Math.Min(ReasonableConfig.BaseIncrease + additionalIncrease, ReasonableConfig.MaxIncrease);
                    result *= (1 + totalIncrease);
                    break;

                case EnumOperationParameter.None:
                    // Handle margin multipliers
                    decimal currentMargin = result - item.UnitCost;

                    // Fix: Check triple margin first, then double margin
                    if (result > item.UnitCost * 3)
                    {
                        // Triple margin
                        result = item.UnitCost + (currentMargin * 3);
                    }
                    else if (result > item.UnitCost * 2)
                    {
                        // Double margin
                        result = item.UnitCost + (currentMargin * 2);
                    }

                    // Handle max penetration price
                    decimal minViable = item.UnitCost * (1 + MinMarginPercent / 100);
                    result = Math.Max(minViable, result * 0.93m);
                    break;

                case EnumOperationParameter.Param_Seasonal:
                    // Apply seasonal adjustments
                    switch (CurrentSeason)
                    {
                        case EnumSeason.Winter:
                            result *= 0.95m;
                            break;
                        case EnumSeason.Spring:
                            result *= 1.03m;
                            break;
                        case EnumSeason.Summer:
                            result *= 1.05m;
                            break;
                        case EnumSeason.Fall:
                            result *= 1.02m;
                            break;
                        case EnumSeason.Holiday:
                            result *= 1.08m;
                            break;
                        default:
                            result *= 1.01m;
                            break;
                    }
                    break;

                case EnumOperationParameter.Param_KeyPricePoint:
                    // Target common psychological price points
                    if (result <= 10)
                        result = Math.Floor(result) - 0.01m;
                    else if (result <= 20)
                        result = Math.Floor(result) + 0.99m;
                    else if (result <= 50)
                        result = Math.Floor(result / 5) * 5 - 0.01m;
                    else
                        result = Math.Floor(result / 10) * 10 - 0.01m;
                    break;

                case EnumOperationParameter.Param_Psychological:
                    // Apply psychological pricing
                    if (fraction < 0.25m)
                        result = wholePart + 0.25m;
                    else if (fraction < 0.49m)
                        result = wholePart + 0.49m;
                    else if (fraction < 0.69m)
                        result = wholePart + 0.69m;
                    else if (fraction < 0.79m)
                        result = wholePart + 0.79m;
                    else if (fraction < 0.89m)
                        result = wholePart + 0.89m;
                    else if (fraction < 0.95m)
                        result = wholePart + 0.95m;
                    else if (fraction < 0.99m)
                        result = wholePart + 0.99m;
                    else
                        result = wholePart + 1.00m;
                    break;

                case EnumOperationParameter.Param_Cents_25:
                case EnumOperationParameter.Param_Cents_49:
                case EnumOperationParameter.Param_Cents_50:
                case EnumOperationParameter.Param_Cents_75:
                case EnumOperationParameter.Param_Cents_95:
                case EnumOperationParameter.Param_Cents_99:
                    // Apply cent adjustments
                    result = Math.Floor(result) + GetAddValue(parameter);
                    break;
            }

            return result;
        }
    }
}