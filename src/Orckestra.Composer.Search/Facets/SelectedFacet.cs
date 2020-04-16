using System.Collections.Generic;

namespace Orckestra.Composer.Search.Facets
{
    public class SelectedFacets
    {
        /// <summary>
        /// Gets or sets the list of the selected Facets.
        /// </summary>
        public List<SelectedFacet> Facets { get; set; }

        /// <summary>
        /// Gets or sets the value indicating if all the selected facet can be removed at once
        /// </summary>
        public bool IsAllRemovable { get; set; }

        public SelectedFacets()
        {
            Facets = new List<SelectedFacet>();
        }
    }

    public class SelectedFacet
    {
        /// <summary>
        /// Gets or sets the name of the field.
        /// </summary>
        /// <value>
        /// The name of the field.
        /// </value>
        public string FieldName { get; set; }

        /// <summary>
        /// Gets or sets the displayable value.
        /// </summary>
        /// <value>
        /// The display name.
        /// </value>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the value of the selected facet
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the type of the facet.
        /// </summary>
        /// <value>
        /// The type of the facet.
        /// </value>
        public FacetType FacetType { get; set; }

        /// <summary>
        /// Gets or sets the value indicating if the selected facet can be removed.
        /// </summary>
        public bool IsRemovable { get; set; }

        /// <summary>
        /// Gets or sets the minimum value of the selected range facet
        /// </summary>
        public string MinimumValue { get; set; }

        /// <summary>
        /// Gets or sets the maximum value of the selected range facet
        /// </summary>
        public string MaximumValue { get; set; }
    }
}
