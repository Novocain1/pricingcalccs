namespace PricingCalculator
{
    public struct PricingOperation
    {
        public EnumOperationType Type { get; }
        public EnumOperationParameter Parameter { get; }
        public EnumOperationPriority Priority { get; }

        public PricingOperation(EnumOperationType type, EnumOperationParameter parameter, EnumOperationPriority priority)
        {
            Type = type;
            Parameter = parameter;
            Priority = priority;
        }
    }
}