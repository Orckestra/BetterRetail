using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Search.Facets;

namespace Orckestra.Composer.Search.Providers.SelectedFacet
{
    public class MultiSelectSelectedFacetProvider : ISelectedFacetProvider
    {
        private IFacetLocalizationProvider FacetLocalizationProvider { get; set; }

        public MultiSelectSelectedFacetProvider(IFacetLocalizationProvider facetLocalizationProvider)
        {
            FacetLocalizationProvider = facetLocalizationProvider ?? throw new ArgumentNullException(nameof(facetLocalizationProvider));
        }
        /// <summary>
        ///     Gets the type of facet this provider is building.
        /// </summary>
        protected FacetType FacetType
        {
            get { return FacetType.MultiSelect; }
        }

        /// <summary>
        ///     Creates a new list of a <see cref="Facets.SelectedFacet" /> from a <see cref="filter" /> object.
        /// </summary>
        /// <param name="filter">Facet to create the facet predicate from.</param>
        /// <param name="setting">Settings of the facet</param>
        /// <param name="cultureInfo">Culture in which the display names will be returned in</param>
        public IEnumerable<Facets.SelectedFacet> CreateSelectedFacetList(SearchFilter filter, FacetSetting setting,
            CultureInfo cultureInfo)
        {
            if (filter == null) { throw new ArgumentNullException(nameof(filter)); }
            if (setting == null) { throw new ArgumentNullException(nameof(setting)); }

            if (!setting.FieldName.Equals(filter.Name, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException(
                    string.Format("The specified setting is for the facet '{0}', whereas the filter is for the facet '{1}'",
                    setting.FieldName, filter.Name), nameof(setting));
            }

            if (setting.FacetType != FacetType)
            {
                throw new ArgumentException(
                    string.Format("The facetResult is defined as '{0}' which does not match '{1}'", 
                    setting.FacetType, FacetType), nameof(setting));
            }

            return filter.Value
                .Split(SearchConfiguration.MultiFacetValueSplitter)
                .Select(multiSelectFilterValue => new Facets.SelectedFacet
                {
                    FieldName = filter.Name,
                    DisplayName = FacetLocalizationProvider.GetFormattedFacetValueTitle(filter.Name, multiSelectFilterValue, cultureInfo),
                    FacetType = FacetType,
                    IsRemovable = !filter.IsSystem,
                    Value = multiSelectFilterValue
                }).ToList();
        }
    }
}