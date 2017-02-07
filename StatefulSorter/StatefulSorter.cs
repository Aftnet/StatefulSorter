using System;
using System.Collections.Generic;
using System.Linq;

namespace StatefulSorter
{
    public class StatefulSorter<T, U> : IStatefulSorter<T, U> where U : struct
    {
        private readonly IReadOnlyDictionary<U, Func<T, object>> SortSelectorMap;

        private U? CurrentSort;
        private bool SortByDescending;

        public StatefulSorter(IReadOnlyDictionary<U, Func<T, object>> sortSelectorMap)
        {
            Reset();
            SortSelectorMap = sortSelectorMap;
        }

        public IEnumerable<T> Sort(IEnumerable<T> items, U sortType)
        {
            if (items == null)
            {
                return null;
            }

            if (CurrentSort.HasValue && CurrentSort.Value.Equals(sortType))
            {
                SortByDescending = !SortByDescending;
            }
            else
            {
                SortByDescending = false;
            }

            CurrentSort = sortType;

            var selector = SortSelectorMap[sortType];
            items = SortByDescending ? items.OrderByDescending(selector) : items.OrderBy(selector);
            return items;
        }

        public void Reset()
        {
            CurrentSort = null;
            SortByDescending = false;
        }
    }
}
