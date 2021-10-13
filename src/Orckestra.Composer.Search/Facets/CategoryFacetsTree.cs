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
        public CategoryFacetValuesTreeItem(string title, string value, int quantity, FacetType facetType, string fieldName, bool isSelected, bool isRemovable)
        {
            Title = title;
            Value = value;
            Quantity = quantity;
            FacetType = facetType;
            FieldName = fieldName;
            IsSelected = isSelected;
            IsRemovable = isRemovable;
        }

        public string Title { get; set; }

        public string Value { get; set; }

        public int Quantity { get; set; }

        public string CategoryId { get; set; }

        public string CategoryUrl { get; set; }

        public FacetType FacetType { get; set; }

        public string FieldName { get; set; }

        public bool IsSelected { get; set; }

        public bool IsRemovable { get; set; }

        public List<CategoryFacetValuesTreeItem> Items { get; set; }

        public List<CategoryFacetValuesTreeItem> OnDemandItems { get; set; }
    }
}
