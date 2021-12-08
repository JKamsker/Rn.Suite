using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rnd.Lib.Extensions
{
    public static class StringExtensions
    {
        public static string JoinStrings(this IEnumerable<string> input, string separator)
        {
            return string.Join(separator, input);
        }

        public static string LimitLength(this string input, int length)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            var result = input;
            if (result.Length > length)
            {
                result = result.Substring(0, length);
            }

            return result;
        }

        public static bool EndsWith(this string input, params string[] endings)
        {
            return input.EndsWith(StringComparison.OrdinalIgnoreCase, endings);
        }

        public static bool EndsWith(this string input, StringComparison stringComparison, params string[] endings)
        {
            return endings.Any(x => input.EndsWith(x, stringComparison));
        }
    }
}