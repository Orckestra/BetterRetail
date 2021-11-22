using System.Collections.Generic;
using System.Globalization;
using Orckestra.Composer.Parameters;

namespace Orckestra.Composer.Search.Parameters
{
    public class GetCategoryBrowsingViewModelParam
    {
        public string CategoryId { get; set; }

        public string CategoryName { get; set; }

        public int Page { get; set; }

        public string SortBy { get; set; }

        public string SortDirection { get; set; }

        public int NumberOfItemsPerPage { get; set; }

        public List<SearchFilter> SelectedFacets { get; set; }

        public bool IsAllProducts { get; set; }

        public string BaseUrl { get; set; }

        public CultureInfo CultureInfo { get; set; }

        public List<string> InventoryLocationIds { get; set; }

        public GetCategoryBrowsingViewModelParam()
        {
            SelectedFacets = new List<SearchFilter>();
            InventoryLocationIds = new List<string>();
        }
    }
}
