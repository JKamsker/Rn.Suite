using System;
using System.Collections.Generic;
using System.Text;

namespace Rnd.Lib.Extensions
{
    public static class SpanExtensions
    {
        public static bool Contains(this string[] array, string value, StringComparison comparison = StringComparison.Ordinal)
            => array.AsSpan().Contains(value, comparison);

        public static bool Contains(this Span<string> array, string value, StringComparison comparison = StringComparison.Ordinal)
            => ((ReadOnlySpan<string>)array).Contains(value, comparison);

        public static bool Contains(this ReadOnlySpan<string> array, string value, StringComparison comparison = StringComparison.Ordinal)
        {
            foreach (var item in array)
            {
                if (string.Equals(value, item, comparison))
                {
                    return true;
                }
            }
            return false;
        }
    }
}