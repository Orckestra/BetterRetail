using System;
using System.Collections.Generic;

namespace Orckestra.Composer.Providers.Localization
{
    /// <summary>
    /// Tree structure for storying Localized resource for a given culture and a given category
    /// </summary>
    public class LocalizationCategory
    {
        public string                     CategoryName    { get; private set; }
        public Dictionary<string,string>  LocalizedValues { get; private set; }

        public LocalizationCategory(string categoryName)
        {
            CategoryName    = categoryName;
            LocalizedValues = new Dictionary<string, string>(StringComparer.InvariantCulture);
        }
    }
}
