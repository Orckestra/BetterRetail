using System;

namespace Orckestra.Composer.Search.Facets
{
    public class PromotedFacetValueSetting
    {
        public PromotedFacetValueSetting(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentNullException("title");
            }
            Title = title;
        }

        /// <summary>
        /// Gets the title of the value
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Sort Order Weight, lowest comes first
        /// </summary>
        public double SortWeight { get; set; }
    }
}