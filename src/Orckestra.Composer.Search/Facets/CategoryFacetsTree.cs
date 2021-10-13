using System.Collections.Generic;

namespace Orckestra.Composer.Search.Facets
{
    public class CategoryFacetValuesTree
    {
        public long RootTotalCount { get; set; }
        public List<CategoryFacetValuesTreeItem> Items { get; set; }
    }

    public class CategoryFacetValuesTreeItem
    {
        public CategoryFacetValuesTreeItem(FacetValue facetValue, FacetType facetType, string fieldName)
        {
            FacetValue = facetValue;
            FacetType = facetType;
            FieldName = fieldName;
            IsSelected = facetValue.IsSelected;
            IsRemovable = true;
        }

        public CategoryFacetValuesTreeItem(string selectedValue, int? selectedCount, bool isRemovable, FacetType facetType, string fieldName)
        {
            SelectedValue = selectedValue;
            SelectedTotalCount = selectedCount;
            IsRemovable = isRemovable;
            FacetType = facetType;
            FieldName = fieldName;
            IsSelected = true;
        }

        public string CategoryId { get; set; }
        public string CategoryUrl { get; set; }
        public FacetType FacetType { get; set; }

        public string FieldName { get; set; }

        public bool IsSelected { get; set; }
        public bool IsRemovable { get; set; }
        public string SelectedValue { get; set; }
        public int? SelectedTotalCount { get; set; }

        public FacetValue FacetValue { get; set; }
        public List<CategoryFacetValuesTreeItem> Items { get; set; }

        public List<CategoryFacetValuesTreeItem> OnDemandItems { get; set; }
    }
}
