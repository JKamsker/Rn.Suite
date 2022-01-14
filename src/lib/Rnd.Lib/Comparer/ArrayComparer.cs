using System;
using System.Collections.Generic;

namespace Rnd.Lib.Utils
{
    public class ArrayComparer<T> : IComparer<T[]>
        where T : IComparable<T>
    {
        public static ArrayComparer<T> Instance = new ArrayComparer<T>();

        //
        // Summary:
        //     Compares two objects and returns a value indicating whether one is less than,
        //     equal to, or greater than the other.
        //
        // Parameters:
        //   x:
        //     The first object to compare.
        //
        //   y:
        //     The second object to compare.
        //
        // Returns:
        //     A signed integer that indicates the relative values of x and y, as shown in the
        //     following table.
        //     Value – Meaning
        //     Less than zero – x is less than y.
        //     Zero – x equals y.
        //     Greater than zero – x is greater than y.
        public int Compare(T[] x, T[] y)
        {
            if (x is null && y is null)
            {
                return ComparisonResult.XIsEqualToY;
            }

            if (x is null)
            {
                return ComparisonResult.XIsLessThanY;
            }

            if (y is null)
            {
                return ComparisonResult.XIsGreaterThanY;
            }


            var maxLen = Math.Max(x.Length, y.Length);

            for (int i = 0; i < maxLen; i++)
            {
                var xVal = i < x.Length ? x[i] : default;
                var yVal = i < y.Length ? y[i] : default;

                var compareResult = xVal.CompareTo(yVal);
                if (compareResult != ComparisonResult.XIsEqualToY)
                {
                    return compareResult;
                }
            }

            return ComparisonResult.XIsEqualToY;
        }
    }
}
