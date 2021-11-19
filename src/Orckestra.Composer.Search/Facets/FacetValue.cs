namespace Orckestra.Composer.Search.Facets
{
    public class FacetValue
    {
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the internal value.
        /// </summary>
        public string Value { get; set; }

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
        /// Gets or sets a value indicating whether this instance should be promoted in the list of facet
        /// </summary>
        public bool IsPromoted { get; set; }

        /// <summary>
        /// Gets or sets the minimum value of the range facet value
        /// </summary>
        public string MinimumValue { get; set; }

        /// <summary>
        /// Gets or sets the maximum value of the range facet value
        /// </summary>
        public string MaximumValue { get; set; }

        /// <summary>
        /// Sort Order Weight, lowest comes first
        /// </summary>
        public double PromotionSortWeight { get; set; }

        /// <summary>
        /// Gets or sets the value indicating if the facet value can be removed. Used for Query pages
        /// </summary>
        public bool IsRemovable { get; set; } = true;
    }
}
