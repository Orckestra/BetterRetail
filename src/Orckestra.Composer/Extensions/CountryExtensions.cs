using System;
using System.Text.RegularExpressions;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;


namespace Orckestra.Composer.Extensions
{
    public static class CountryExtensions
    {
        /// <summary>
        /// Determines whether the format of the postal code is valid using on the postal code regex of that country
        /// </summary>
        public static void Validate(this Country.CountryViewModel country, string postalCode)
        {
            if (country == null) { throw new ArgumentNullException(nameof(country)); }
            if (string.IsNullOrWhiteSpace(postalCode)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(), nameof(postalCode)); }

            if (!string.IsNullOrWhiteSpace(country.PostalCodeRegex))
            {
                var regex = new Regex(country.PostalCodeRegex, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
                if (regex.IsMatch(postalCode))
                {
                    return;
                }
                else
                {
                    throw new InvalidOperationException(string.Format("The postal code {0} does not match the regex of {1}", postalCode, country.CountryName));
                }
            }
        }
    }
}