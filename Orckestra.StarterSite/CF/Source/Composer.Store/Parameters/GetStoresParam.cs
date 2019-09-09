using System;
using System.Globalization;

namespace Orckestra.Composer.Store.Parameters
{
    public class GetStoresParam
    {
        public string Scope { get; set; }
        public CultureInfo CultureInfo { get; set; }

        public Guid WebsiteId { get; set; }
        public string BaseUrl { get; set; }
        /// <summary>
        /// Gets or sets the number of search results to display per paged search result.
        /// </summary>
        public int PageSize { get; set; }

        public int PageNumber { get; set; }

        public bool IncludeExtraInfo { get; set; } = false;
    }
}
