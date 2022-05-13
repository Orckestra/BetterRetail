using System;
using System.Text.RegularExpressions;

namespace Orckestra.Composer.Cart.Extensions
{
    internal static class StringExtensions
    {
        public static string RemoveWhitespaces(this string value)
        {
            if (value == null) { throw new ArgumentNullException(nameof(value)); }

            return Regex.Replace(value, @"\s+", string.Empty);
        }
    }
}