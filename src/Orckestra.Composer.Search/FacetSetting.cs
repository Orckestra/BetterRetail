using System;
using System.Collections.Generic;
using Orckestra.Composer.Search.Facets;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;

namespace Orckestra.Composer.Search
{
    public class FacetSetting
    {
        public FacetSetting(string fieldname)
        {
            if (string.IsNullOrEmpty(fieldname)) { throw new ArgumentException(GetMessageOfNullEmpty(), nameof(fieldname)); }

            FieldName = fieldname;
            FacetType = FacetType.SingleSelect;
            SortWeight = 0.0;
            MaxCollapsedValueCount = 5;
            MaxExpendedValueCount = int.MaxValue;
            DependsOn = new List<string>();
            StartValue = null;
            EndValue = null;
            GapSize = null;
            FacetValueType = typeof (string);
            IsDisplayed = true;
            PromotedValues = new List<PromotedFacetValueSetting>();
        }

        /// <summary>
        /// Solr field name (Mandatory)
        /// </summary>
        public string FieldName { get; private set; }

        /// <summary>
        /// Solr field type (Single, Multi, ...)
        /// </summary>
        public FacetType FacetType { get; set; }

        /// <summary>
        /// Sort Order Weight, lowest comes first
        /// </summary>
        public double SortWeight { get; set; }

        /// <summary>
        /// Maximum number of values to display when this facet is collapsed
        /// </summary>
        public int MaxCollapsedValueCount { get; set; }
        
        /// <summary>
        /// Maximum number of values to display when this facet is expended
        /// </summary>
        public int MaxExpendedValueCount { get; set; }

        /// <summary>
        /// List of Facet FieldNames this facet depends on. 
        /// This facet will only be queryed if one of the dependency has a selected value.
        /// An empty dependency list means no dependencies
        /// </summary>
        public IList<string> DependsOn { get; set; }

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
        /// Gets or sets the type of the values of the facet
        /// </summary>
        public Type FacetValueType { get; set; }

        /// <summary>
        /// Gets or sets a value determining whether the facet is displayed or not in the search results
        /// </summary>
        public bool IsDisplayed { get; set; }

        /// <summary>
        /// Get or sets a list of facet values which should be promoted in the list of facets in the search results page
        /// </summary>
        public IList<PromotedFacetValueSetting> PromotedValues { get; set; }

    }
}
