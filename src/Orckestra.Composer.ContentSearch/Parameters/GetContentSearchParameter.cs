using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.ContentSearch.Parameters
{
    public class GetContentSearchParameter
    {
        public string BaseUrl { get; set; }
        public string SearchQuery { get; set; }
        public bool IsCorrectedSearchQuery { get; set; }
        public string CorrectedSearchQuery { get; set; }
        public string SortBy { get; set; }

        public string SortDirection { get; set; }
        public string[] QueryKeys { get; set; }
        public string[] Facets { get; set; }
        public CultureInfo Culture { get; set; }
        public int PageSize { get; set; }

        public int CurrentPage { get; set; }

        public string PathInfo { get; set; }

        public bool ProductsTabActive { get; set; }

        public bool CurrentSiteOnly { get; set; }
    }
}
