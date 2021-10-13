using System.Collections.Generic;
using System.Globalization;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Search.Facets;
using Orckestra.Composer.Utils;

namespace Orckestra.Composer.Search.Factory
{
    public interface IFacetFactory
    {
        /// <summary>
        /// Creates a new instance of <see cref="Facet"/> based on a <param name="facet"></param> object.
        /// </summary>
        Facet CreateFacet(Overture.ServiceModel.Search.Facet facet, IReadOnlyList<SearchFilter> selectedFacets,
            CultureInfo cultureInfo);

        /// <summary>
        /// Convert category facet values to the tree view
        /// </summary>
        /// <param name="facets">All facets</param>
        /// <param name="selectedFacets">Currect selected facets</param>
        /// <param name="categoriesTree">Categories tree</param>
        /// <param name="culture">Culture</param>
        /// <param name="categoryCounts">Independent product counts for each category and total count for search results</param>
        /// <returns></returns>
        CategoryFacetValuesTree BuildCategoryFacetValuesTree(IList<Facet> facets,
            SelectedFacets selectedFacets,
            Tree<Overture.ServiceModel.Products.Category, string> categoriesTree,
            CategoryFacetCounts categoryCounts,
            CultureInfo culture);
    }
}