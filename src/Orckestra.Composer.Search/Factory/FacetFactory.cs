using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Search.Context;
using Orckestra.Composer.Search.Facets;
using Orckestra.Composer.Search.Providers.Facet;
using Orckestra.Overture;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;

namespace Orckestra.Composer.Search.Factory
{
    public class FacetFactory : ProviderFactory<IFacetProvider>, IFacetFactory
    {
        protected IFacetProviderRegistry FacetProviderRegistry { get; }
        protected IFacetConfigurationContext FacetConfigContext { get; }

        public FacetFactory(IDependencyResolver dependencyResolver, IFacetProviderRegistry facetProviderRegistry, IFacetConfigurationContext facetConfigContext)
            : base(dependencyResolver)
        {
            FacetProviderRegistry = facetProviderRegistry ?? throw new ArgumentNullException(nameof(facetProviderRegistry));
            FacetConfigContext = facetConfigContext;
        }

        /// <summary>
        /// Creates a new instance of <see cref="Facet"/> based on a <param name="facet"></param> object.
        /// </summary>
        public Facet CreateFacet(Overture.ServiceModel.Search.Facet facet, IReadOnlyList<SearchFilter> selectedFacets, CultureInfo cultureInfo)
        {
            if (facet == null) { throw new ArgumentNullException(nameof(facet)); }
            if (string.IsNullOrWhiteSpace(facet.FieldName)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(facet.FieldName)), nameof(facet)); }

            var setting = FacetConfigContext.GetFacetSettings()
                    .Find(s => s.FieldName.Equals(facet.FieldName, StringComparison.OrdinalIgnoreCase));

            if (setting == null) { return null; }

            Type factoryType = FacetProviderRegistry.ResolveProviderType(setting.FacetType.ToString());

            var instance = GetProviderInstance(factoryType);

            return instance.CreateFacet(facet, setting, selectedFacets, cultureInfo);
        }
    }
}