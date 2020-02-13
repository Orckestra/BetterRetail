using System;
using System.Globalization;
using System.Linq;
using FizzWare.NBuilder.Generators;
using NUnit.Framework;

namespace Orckestra.ForTests
{
    /// <summary>
    /// Randomizing extensions for Unit Test purposes
    /// </summary>
    public static class TestingExtensions
    {
        /// <summary>
        /// Get a random CultureInfo
        /// </summary>
        /// <returns></returns>
        public static CultureInfo GetRandomCulture()
        {
            var allCultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
            var length = allCultures.Length - 1;
            var randomCulture = allCultures[GetRandom.Int(1, length)];

            Assert.That(randomCulture != null);
            Assert.That(!string.IsNullOrEmpty(randomCulture.Name));

            return allCultures[GetRandom.Int(1, length)];
        }

        /// <summary>
        /// Get a random value for the Given Enum type
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <returns></returns>
        public static TEnum GetRandomEnum<TEnum>()
        {
            var values = Enum.GetValues(typeof(TEnum));
            var rndIndex = GetRandom.Int(0, values.Length - 1);

            return values.OfType<TEnum>().ElementAt(rndIndex);
        }
    }
}
