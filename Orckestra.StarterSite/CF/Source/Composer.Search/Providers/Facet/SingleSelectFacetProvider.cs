using System.Collections.Generic;
using System.Linq;
using Orckestra.Composer.Search.Facets;

namespace Orckestra.Composer.Search.Providers.Facet
{
    public class SingleSelectFacetProvider : BaseFacetProvider, IFacetProvider
    {
        public SingleSelectFacetProvider(IFacetLocalizationProvider facetLocalizationProvider)
            : base(facetLocalizationProvider)
        {
        }

        /// <summary>
        ///     Gets the type of facet this provider is building.
        /// </summary>
        protected override FacetType FacetType
        {
            get { return FacetType.SingleSelect; }
        }

        protected override IEnumerable<string> TransformFacetSelectedValue(Overture.ServiceModel.Search.Facet facetResult, string facetSelectedValue)
        {
            var criteriaFacetValues = new[] {facetSelectedValue};

            //Keep only the criteria matching the result
            var selectedFacetValues = criteriaFacetValues
                .Join(facetResult.Values,
                    criteriaValue => criteriaValue,
                    result => result.Value,
                    (criteriaValue, result) => result.Value);

            return selectedFacetValues;
        }
    }
}