using System;
using System.Collections.Generic;
using System.Globalization;

namespace Orckestra.Composer.Recipes.Parameters
{
    public class GetCustomerRecipeFavoriteSearchResultsParam
    {
        public string SearchQuery { get; set; }
        public string SortBy { get; set; }
        public string SortDirection { get; set; }
        public string[] QueryKeys { get; set; }
        public CultureInfo Culture { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public List<string> FavoriteIds { get; set; } = new List<string>();
        public string Scope { get; set; }
        public Guid CustomerId { get; set; }
    }
}
