using System.Collections.Generic;
using System.Linq;

namespace Rnd.Lib.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> OrEmpty<T>(this IEnumerable<T> enumerable)
        {
            return enumerable ?? Enumerable.Empty<T>();
        }
    }
}