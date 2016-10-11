﻿using System;

namespace Xunit
{
    internal sealed class FactAttribute : Attribute
    {
    }

    internal static class Assert
    {
        public static void Equal(int expected, int actual)
        {
            if (expected != actual)
                throw new InvalidOperationException($"expected: {expected.ToString()} actual: {actual.ToString()}");
        }
        public static void Equal(String expected, String actual)
        {
            if (expected != actual)
                throw new InvalidOperationException($"expected: {expected} actual: {actual}");
        }
        public static void Equal<T>(T expected, T actual)
        {
            if (expected == null && actual == null)
                return;

            if (!expected.Equals(actual))
                throw new InvalidOperationException($"expected: {expected.ToString()} actual: {actual.ToString()}");
        }
    }
}

