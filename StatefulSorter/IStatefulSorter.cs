using System.Collections.Generic;

namespace StatefulSorter
{
    public interface IStatefulSorter<T, U> where U : struct
    {
        IEnumerable<T> Sort(IEnumerable<T> items, U sortType);
        void Reset();
    }
}