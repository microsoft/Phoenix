// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestDataUtils.cs" company="Microsoft Corporation">
//   Microsoft Corporation. All rights reserved.
//   Information Contained Herein is Proprietary and Confidential.
// </copyright>
// <summary>
//   Utilities for generating test data for unit tests
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Phoenix.Test.Common
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    ///     Provides utility methods for generating test data.
    /// </summary>
    public static class TestDataUtils
    {
        private static Lazy<Random> rng = new Lazy<Random>(() => new Random());

        /// <summary>
        ///     Randomly generates either True or False
        /// </summary>
        /// <returns>A random boolean value</returns>
        public static bool GetRandomBoolean()
        {
            return rng.Value.Next(2) == 1;
        }

        /// <summary>
        ///     Randomly generates guid
        /// </summary>
        /// <returns>A random guid value</returns>
        public static Guid GetRandomGuid()
        {
            return Guid.NewGuid();
        }

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

            //return new string(GetRandomCharSequence(length).ToArray());
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

        /// <summary>
        ///     Generates a random integer.
        /// </summary>
        /// <returns>A random Int32.</returns>
        public static int GetRandomInt()
        {
            return rng.Value.Next();
        }

        /// <summary>
        /// Generates a random 16-bit integer.
        /// </summary>
        /// <returns>A random Int16.</returns>
        public static short GetRandomShort()
        {
            return (short)rng.Value.Next(short.MaxValue);
        }

        /// <summary>
        /// Generates a random decimal value.
        /// </summary>
        /// <returns>A random Decimal.</returns>
        public static decimal GetRandomDecimal()
        {
            return (decimal)rng.Value.Next();
        }

        /// <summary>
        ///     Generates a random email address.
        /// </summary>
        /// <returns>A random email address.</returns>
        public static string GetRandomEmailAddress(int length = 16)
        {
            const string Suffix = "@example.com";
            const int SuffixLength = 12;

            var stringBuilder = new StringBuilder();
            for (int i = SuffixLength; i < length; i++)
            {
                stringBuilder.Append(rng.Value.Next(10));
            }

            stringBuilder.Append(Suffix);
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Generates a random number between 0 and 1.
        /// </summary>
        /// <returns>A random number</returns>
        public static double GetRandomPercentage()
        {
            return rng.Value.NextDouble();
        }

        /// <summary>
        /// Gets a random date, +/- 365 days from now.
        /// </summary>
        /// <returns>A new DateTime</returns>
        public static DateTime GetRandomDate()
        {
            return DateTime.Now + TimeSpan.FromDays(rng.Value.Next(-365, 365));
        }

        /// <summary>
        /// Gets a random byte array
        /// </summary>
        /// <param name="length">The length of the byte array</param>
        /// <returns>A new byte array</returns>
        public static byte[] GetRandomByteArray(int length = 8)
        {
            var result = new byte[length];
            rng.Value.NextBytes(result);
            return result;
        }

    }
}
