using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Search.Facets;
using Orckestra.Composer.Search.Providers;
using Orckestra.Composer.Search.Providers.Facet;
using Orckestra.Composer.Utils;
using Orckestra.Overture;

namespace Orckestra.Composer.Search.Factory
{
    public class FacetFactory : ProviderFactory<IFacetProvider>, IFacetFactory
    {
        protected IFacetProviderRegistry FacetProviderRegistry { get; set; }

        public FacetFactory(IDependencyResolver dependencyResolver, IFacetProviderRegistry facetProviderRegistry)
            : base(dependencyResolver)
        {
            if (facetProviderRegistry == null) { throw new ArgumentNullException("facetProviderRegistry"); }
            FacetProviderRegistry = facetProviderRegistry;
        }

        /// <summary>
        /// Creates a new instance of <see cref="Facet"/> based on a <param name="facet"></param> object.
        /// </summary>
        public Facet CreateFacet(Overture.ServiceModel.Search.Facet facet, IReadOnlyList<SearchFilter> selectedFacets, CultureInfo cultureInfo)
        {
            if (facet == null) { throw new ArgumentNullException("facet"); }
            if (string.IsNullOrWhiteSpace(facet.FieldName)) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("FieldName"), "facet"); }

            var setting = SearchConfiguration.FacetSettings
                    .FirstOrDefault(s => s.FieldName.Equals(facet.FieldName, StringComparison.OrdinalIgnoreCase));

            if (setting == null)
            {
                return null;
            }

            Type factoryType = FacetProviderRegistry.ResolveProviderType(setting.FacetType.ToString());

            var instance = GetProviderInstance(factoryType);

            return instance.CreateFacet(facet, setting, selectedFacets, cultureInfo);
        }
    }
}