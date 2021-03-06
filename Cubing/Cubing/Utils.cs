﻿using System;

namespace Cubing
{
    /// <summary>
    /// Contains a function for calculating the binomial coefficient.
    /// </summary>
    public static class Utils
    {
        //IMPR doc
        /// <summary>
        /// Calculate the binomial coefficient (<paramref name="n"/> choose
        /// <paramref name="k"/>). Return 0 if
        /// <c><paramref name="k"/> &lt; 0</c> or if
        /// <c><paramref name="k"/> &lt; 0</c> or if
        /// <c><paramref name="k"/> &gt; <paramref name="n"/></c>.
        /// </summary>
        /// <param name="n">The number of possibilities.</param>
        /// <param name="k">The number of items to choose.</param>
        /// <returns>
        /// The binomial coefficient (<paramref name="n"/> choose
        /// <paramref name="k"/>).
        /// </returns>
        public static int BinomialCoefficient(int n, int k) {
            if (k < 0 || k > n)
                return 0;
            k = Math.Min(k, n - k); //symmetry
            int c = 1;
            for (int i = 0; i < k; i++)
                c = c * (n - i) / (i + 1);
            return c;
        }
    }
}