using System;
using System.Globalization;

namespace Orckestra.Composer.Store.Parameters
{
    public class GetStoreUrlParam
    {
        public string BaseUrl { get; set; }

        public string StoreName { get; set; }

        public string StoreNumber { get; set; }

        public CultureInfo CultureInfo { get; set; }

        public Guid WebsiteId { get; set; }
    }
}
