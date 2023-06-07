using System;
using System.Collections.Generic;
using Xunit;

namespace DG.Common.Tests.XUnitHelpers
{
    public static class MoreAsserts
    {
        public static void NoExceptions(Action testCode)
        {
            var exception = Record.Exception(testCode);
            Assert.Null(exception);
        }

        public static void IsInMargin(TimeSpan actual, TimeSpan expected, TimeSpan margin)
        {
            Assert.InRange(actual, expected - margin, expected + margin);
        }

        public static void IsInMargin(double actual, double expected, double margin)
        {
            Assert.InRange(actual, expected - margin, expected + margin);
        }

        public static void IsOrdered<T>(IReadOnlyList<T> list) where T : IComparable<T>
        {
            if (list.Count > 1)
            {
                for (int i = 1; i < list.Count; i++)
                {
                    Assert.True(list[i - 1].CompareTo(list[i]) <= 0, $"Item at index {i - 1} ({list[i - 1]}) is not less than {list[i]}");
                }
            }
        }

        public static void IsOrderedDescending<T>(IReadOnlyList<T> list) where T : IComparable<T>
        {
            if (list.Count > 1)
            {
                for (int i = 1; i < list.Count; i++)
                {
                    Assert.True(list[i - 1].CompareTo(list[i]) >= 0, $"Item at index {i - 1} ({list[i - 1]}) is not greater than {list[i]}");
                }
            }
        }
    }
}
