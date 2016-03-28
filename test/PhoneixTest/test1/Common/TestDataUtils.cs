// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestDataUtils.cs" company="Microsoft Corporation">
//   Microsoft Corporation. All rights reserved.
//   Information Contained Herein is Proprietary and Confidential.
// </copyright>
// <summary>
//   Utilities for generating test data.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Phoenix.Test.Common
{
    using System;
    using System.Linq;
    using System.Text;

    /// <summary>
    ///     Provides utility methods for generating test data.
    /// </summary>
    public static class TestDataUtils
    {
        private static Lazy<Random> rng = new Lazy<Random>(() => new Random());

        /// <summary>
        ///     Generates a string of random characters
        /// </summary>
        /// <param name="length">The length of the string to generate</param>
        /// <returns>A string of random characters</returns>
        public static string GetRandomString(int length = 16)
        {
            var stringBuilder = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                stringBuilder.Append(rng.Value.Next(10));
            }

            return stringBuilder.ToString();

        }

        /// <summary>
        /// Generates a sequence of random characters.
        /// </summary>
        /// <param name="length">The length of the sequence to generate</param>
        /// <returns>A sequence of random characters</returns>
        public static string GetRandomCharString(int length = 16)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
