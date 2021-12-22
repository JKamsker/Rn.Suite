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

#nullable enable

        public static IEnumerable<string> NotNullOrEmpty(this IEnumerable<string?> enumerable)
        {
            foreach (var item in enumerable)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    yield return item;
                }
            }
        }

#nullable disable
    }
}