using System.ComponentModel;
using System;
using System.Collections.Generic;

namespace PricingCalculator
{
    public class SortableBindingList<T> : BindingList<T>
    {
        private bool _isSorted;
        private PropertyDescriptor _sortProperty;
        private ListSortDirection _sortDirection;

        protected override bool SupportsSortingCore => true;

        protected override bool IsSortedCore => _isSorted;

        protected override PropertyDescriptor SortPropertyCore => _sortProperty;

        protected override ListSortDirection SortDirectionCore => _sortDirection;

        protected override void ApplySortCore(PropertyDescriptor prop, ListSortDirection direction)
        {
            _sortProperty = prop;
            _sortDirection = direction;

            List<T> itemsList = (List<T>)Items;
            itemsList.Sort(Compare);

            _isSorted = true;
            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        private int Compare(T x, T y)
        {
            var result = CompareValues(
                _sortProperty.GetValue(x),
                _sortProperty.GetValue(y));

            return _sortDirection == ListSortDirection.Ascending ? result : -result;
        }

        private int CompareValues(object x, object y)
        {
            if (x == null) return y == null ? 0 : -1;
            if (y == null) return 1;
            return x is IComparable c ? c.CompareTo(y) : x.Equals(y) ? 0 : x.GetHashCode().CompareTo(y.GetHashCode());
        }

        protected override void RemoveSortCore()
        {
            _isSorted = false;
            _sortProperty = null;
        }
    }
}