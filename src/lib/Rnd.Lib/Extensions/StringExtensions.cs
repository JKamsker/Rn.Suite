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

        public static string TrimStart
        (
            this string target,
            string trimString,
            StringComparison stringComparison = StringComparison.OrdinalIgnoreCase
        )
        {
            if (string.IsNullOrEmpty(trimString)) return target;

            string result = target;
            while (result.StartsWith(trimString, stringComparison))
            {
                result = result.Substring(trimString.Length);
            }

            return result;
        }

        public static string TrimEnd(this string inputText, string value, StringComparison comparisonType = StringComparison.CurrentCultureIgnoreCase)
        {
            if (!string.IsNullOrEmpty(value))
            {
                while (!string.IsNullOrEmpty(inputText) && inputText.EndsWith(value, comparisonType))
                {
                    inputText = inputText.Substring(0, (inputText.Length - value.Length));
                }
            }

            return inputText;
        }


        public static string ConcatString(this IEnumerable<string> enumerable) => string.Concat(enumerable);

        public static string ConcatString<T>(this IEnumerable<T> enumerable, Func<T, string> selector) => string.Concat(enumerable.Select(selector));

        public static string JoinString(this IEnumerable<string> enumerable, char seperator) => string.Join(seperator, enumerable);

        public static string JoinString(this IEnumerable<string> enumerable, string seperator) => string.Join(seperator, enumerable);

        public static string JoinString<T>(this IEnumerable<T> enumerable, string seperator, Func<T, string> selector) => string.Join(seperator, enumerable.Select(x => selector(x)));

        public static string JoinString<T>(this IEnumerable<T> enumerable, char seperator, Func<T, string> selector) => string.Join(seperator, enumerable.Select(x => selector(x)));

    }
}