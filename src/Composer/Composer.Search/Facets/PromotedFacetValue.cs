using System;

namespace Orckestra.Composer.Search.Facets
{
    public class PromotedFacetValue
    {
        public PromotedFacetValue(string fieldName, FacetType facetType, string value)
        {
            if (string.IsNullOrWhiteSpace(fieldName))
            {
                throw new ArgumentNullException("fieldName");
            }
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException("value");
            }

            FieldName = fieldName;
            FacetType = facetType;
            Value = value;
            Title = value;
        }

        /// <summary>
        /// Gets the name of the facet associated to the value
        /// </summary>
        public string FieldName { get; private set; }

        /// <summary>
        /// Gets the value of a facet to be promoted in the facet list
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the internal value.
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// Gets or sets the quantity.
        /// </summary>
        /// <value>
        /// The quantity.
        /// </value>
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is selected.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is selected; otherwise, <c>false</c>.
        /// </value>
        public bool IsSelected { get; set; }

        /// <summary>
        /// Gets the type of facet
        /// </summary>
        public FacetType FacetType { get; private set; }
    }
}