using System;
using System.Collections.Generic;
using System.Text;

namespace Rnd.Lib.Extensions
{
    public static class FluentExtensions
    {
        public static T Modify<T>(this T value, Action<T> action)
        {
            action(value);
            return value;
        }

        public static IEnumerable<T> ModifyMany<T>(this IEnumerable<T> values, Action<T> action)
        {
            foreach (var value in values)
            {
                action(value);
                yield return value;
            }
        }
    }
}