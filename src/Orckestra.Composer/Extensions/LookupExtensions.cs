using Orckestra.Overture.ServiceModel;
using Orckestra.Overture.ServiceModel.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Extensions
{
    public static class LookupExtensions
    {
        /// <summary>
        /// Returns the display names of a given lookup value from a Lookup.
        /// Null is returned if no display names are found.
        /// </summary>
        /// <param name="lookup">The Lookup containing all the lookup values</param>
        /// <param name="value">The value to find the display name for within the Lookup</param>
        /// <returns></returns>
        public static LocalizedString GetDisplayNames(this Lookup lookup, string value)
        {
            value = value?.Trim();
            if (lookup == null || lookup.Values == null || lookup.Values.Count == 0 || string.IsNullOrEmpty(value)) return null;

            var foundLookup = lookup.Values
                .FirstOrDefault(lookupValue => string.Equals(lookupValue.Value?.Trim(), value, StringComparison.InvariantCultureIgnoreCase));

            return foundLookup?.DisplayName;
        }

        /// <summary>
        /// Returns the display name of a given lookup value for a given culture from a Lookup.
        /// Null is returned if no display names are found or if no name is found for the received culture.
        /// </summary>
        /// <param name="lookup">The Lookup containing all the lookup values</param>
        /// <param name="value">The value to find the display name for within the Lookup</param>
        /// <param name="cultureName">The name of the culture to use to return the display name</param>
        /// <returns></returns>
        public static string GetDisplayName(this Lookup lookup, string value, string cultureName)
        {
            value = value?.Trim();
            cultureName = cultureName?.Trim();
            if (string.IsNullOrEmpty(value) || string.IsNullOrEmpty(cultureName)) return null;

            var displayNames = lookup?.GetDisplayNames(value);
            return displayNames?.GetLocalizedValue(cultureName);
        }
    }
}
