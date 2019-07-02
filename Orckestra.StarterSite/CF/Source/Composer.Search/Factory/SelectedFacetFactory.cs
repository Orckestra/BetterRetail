using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Search.Facets;
using Orckestra.Composer.Search.Providers.SelectedFacet;
using Orckestra.Composer.Utils;
using Orckestra.Overture;

namespace Orckestra.Composer.Search.Factory
{
    public class SelectedFacetFactory : ProviderFactory<ISelectedFacetProvider>, ISelectedFacetFactory
    {
         private ISelectedFacetProviderRegistry SelectedFacetProviderRegistry { get; set; }

         public SelectedFacetFactory(IDependencyResolver dependencyResolver, ISelectedFacetProviderRegistry selectedFacetProviderRegistry)
            : base(dependencyResolver)
        {
            if (selectedFacetProviderRegistry == null) { throw new ArgumentNullException("selectedFacetProviderRegistry"); }
            SelectedFacetProviderRegistry = selectedFacetProviderRegistry;
        }

        /// <summary>
        /// Creates a new list of <see cref="SelectedFacet"/> based on a <param name="filter"></param> object.
        /// </summary>
         public virtual IEnumerable<SelectedFacet> CreateSelectedFacet(SearchFilter filter, CultureInfo cultureInfo)
        {
            if (filter == null) { throw new ArgumentNullException("filter"); }
            if (string.IsNullOrWhiteSpace(filter.Name)) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("Name"), "filter"); }

            var setting = SearchConfiguration.FacetSettings
                .FirstOrDefault(s => s.FieldName.Equals(filter.Name, StringComparison.OrdinalIgnoreCase));

            if (setting == null)
            {
                return Enumerable.Empty<SelectedFacet>();
            }

            Type factoryType = SelectedFacetProviderRegistry.ResolveProviderType(setting.FacetType.ToString());

            var instance = GetProviderInstance(factoryType);

            return instance.CreateSelectedFacetList(filter, setting, cultureInfo);
        }
    }
}