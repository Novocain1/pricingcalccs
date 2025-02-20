using System.ComponentModel;

namespace PricingCalculator
{
    public class PricingItem : INotifyPropertyChanged
    {
        private string name = "";
        private decimal unitCost;
        private decimal retailPrice;
        private EnumPricingStrategy strategy = EnumPricingStrategy.None;

        // New properties for calculated values
        private decimal recommendedPrice;
        private decimal marginPercent;
        private decimal priceRelativeToMarket;

        public string Name
        {
            get => name;
            set
            {
                if (name != value)
                {
                    name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public decimal UnitCost
        {
            get => unitCost;
            set
            {
                if (unitCost != value)
                {
                    unitCost = value;
                    OnPropertyChanged(nameof(UnitCost));
                }
            }
        }

        public decimal RetailPrice
        {
            get => retailPrice;
            set
            {
                if (retailPrice != value)
                {
                    retailPrice = value;
                    OnPropertyChanged(nameof(RetailPrice));
                }
            }
        }

        public EnumPricingStrategy Strategy
        {
            get => strategy;
            set
            {
                if (strategy != value)
                {
                    strategy = value;
                    OnPropertyChanged(nameof(Strategy));
                }
            }
        }

        // New property definitions
        public decimal RecommendedPrice
        {
            get => recommendedPrice;
            set
            {
                if (recommendedPrice != value)
                {
                    recommendedPrice = value;
                    OnPropertyChanged(nameof(RecommendedPrice));
                }
            }
        }

        public decimal MarginPercent
        {
            get => marginPercent;
            set
            {
                if (marginPercent != value)
                {
                    marginPercent = value;
                    OnPropertyChanged(nameof(MarginPercent));
                }
            }
        }

        public decimal PriceRelativeToMarket
        {
            get => priceRelativeToMarket;
            set
            {
                if (priceRelativeToMarket != value)
                {
                    priceRelativeToMarket = value;
                    OnPropertyChanged(nameof(PriceRelativeToMarket));
                }
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}