using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Search.Context;
using Orckestra.Composer.Search.Facets;
using Orckestra.Composer.Search.Providers.SelectedFacet;
using Orckestra.Overture;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;

namespace Orckestra.Composer.Search.Factory
{
    public class SelectedFacetFactory : ProviderFactory<ISelectedFacetProvider>, ISelectedFacetFactory
    {
         private ISelectedFacetProviderRegistry SelectedFacetProviderRegistry { get; }
         protected IFacetConfigurationContext FacetConfigContext { get; }

         public SelectedFacetFactory(IDependencyResolver dependencyResolver, ISelectedFacetProviderRegistry selectedFacetProviderRegistry, IFacetConfigurationContext facetConfigContext)
            : base(dependencyResolver)
        {
            SelectedFacetProviderRegistry = selectedFacetProviderRegistry ?? throw new ArgumentNullException(nameof(selectedFacetProviderRegistry));
            FacetConfigContext = facetConfigContext;
        }

        /// <summary>
        /// Creates a new list of <see cref="SelectedFacet"/> based on a <param name="filter"></param> object.
        /// </summary>
         public virtual IEnumerable<SelectedFacet> CreateSelectedFacet(SearchFilter filter, CultureInfo cultureInfo)
        {
            if (filter == null) { throw new ArgumentNullException(nameof(filter)); }
            if (string.IsNullOrWhiteSpace(filter.Name)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(filter.Name)), nameof(filter)); }

            var setting = FacetConfigContext.GetFacetSettings()
                .Find(s => s.FieldName.Equals(filter.Name, StringComparison.OrdinalIgnoreCase));

            if (setting == null) { return Enumerable.Empty<SelectedFacet>(); }

            Type factoryType = SelectedFacetProviderRegistry.ResolveProviderType(setting.FacetType.ToString());

            var instance = GetProviderInstance(factoryType);

            return instance.CreateSelectedFacetList(filter, setting, cultureInfo);
        }
    }
}