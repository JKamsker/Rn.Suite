using System;
using System.Collections.Generic;
using System.Text;

namespace Rnd.Lib.Utils
{
    public class FuncEqualityComparer<T> : IEqualityComparer<T>
    {
        Func<T, T, bool> _comparer;
        Func<T, int> _hash;

        public FuncEqualityComparer(Func<T, T, bool> comparer)
        {
            _comparer = comparer;
            _hash = t => t.GetHashCode();
        }
        public FuncEqualityComparer(Func<T, T, bool> comparer, Func<T, int> hash)
        {
            _comparer = comparer;
            _hash = hash;
        }

        public bool Equals(T x, T y)
        {
            return _comparer(x, y);
        }
        public int GetHashCode(T obj)
        {
            return _hash(obj);
        }
    }
}
