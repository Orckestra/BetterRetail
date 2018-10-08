using System;
using System.Collections.Generic;
using System.Globalization;

namespace Orckestra.Composer.Providers.Localization
{
    /// <summary>
    /// Tree structure for storying Localized resource for a given culture and a given category
    /// </summary>
    public class LocalizationTree
    {
        public string                                  CultureName         { get; private set; }
        public Dictionary<string,LocalizationCategory> LocalizedCategories { get; private set; }

        public LocalizationTree(CultureInfo culture)
        {
            CultureName         = culture.Name;
            LocalizedCategories = new Dictionary<string, LocalizationCategory>(StringComparer.InvariantCulture);
        }
    }
}
