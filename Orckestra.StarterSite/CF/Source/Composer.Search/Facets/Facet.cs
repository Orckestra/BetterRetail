using System.Collections.Generic;

namespace Orckestra.Composer.Search.Facets
{
    public class Facet
    {
        /// <summary>
        /// Gets or sets the on demand facets.
        /// </summary>
        /// <value>
        /// The on demand facets.
        /// </value>
        //TODO: Maybe revisit this. It's polluting the FacetGroup. Try and offload the logic to the template or to the view model.
        public IList<FacetValue> OnDemandFacetValues { get; set; } 

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title { get; set; }

        /// <summary>
        /// Gets the total number of possibles values.
        /// </summary>
        /// <value>
        /// The quantity.
        /// </value>
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the type of the facet.
        /// </summary>
        /// <value>
        /// The type of the facet.
        /// </value>
        public FacetType FacetType { get; set; }
        
        /// <summary>
        /// Gets or sets the facet values.
        /// </summary>
        /// <value>
        /// The facets values.
        /// </value>
        public IList<FacetValue> FacetValues { get; set; }

        /// <summary>
        /// Gets or sets the lower bound of the range facet. 
        /// No value lower than this is included in the counts for this facet
        /// </summary>
        public string StartValue { get; set; }

        /// <summary>
        /// Gets or sets the upper bound of the range facet. 
        /// No value higher than this is included in the counts for this facet
        /// </summary>
        public string EndValue { get; set; }

        /// <summary>
        /// Gets or sets the size of each range.
        /// </summary>
        public string GapSize { get; set; }

        /// <summary>
        /// Gets or sets the name of the field.
        /// </summary>
        /// <value>
        /// The name of the field.
        /// </value>
        public string FieldName { get; set; }

        /// <summary>
        /// Sort Order Weight, lowest comes first
        /// </summary>
        public double SortWeight { get; set; }

        /// <summary>
        /// Gets or sets a value determining whether the facet is displayed or not in the search results
        /// </summary>
        public bool IsDisplayed { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Facet"/> class.
        /// </summary>
        public Facet()
        {
            FacetValues = new List<FacetValue>();
            OnDemandFacetValues = new List<FacetValue>();
        }
    }
}
