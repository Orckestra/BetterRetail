using System.Globalization;

namespace Orckestra.Composer.Product.Parameters
{
    public class GetProductBreadcrumbParam
    {
        public string ProductName { get; set; }

        public string CategoryId { get; set; }

        public string HomeUrl { get; set; }

        public string Scope { get; set; }

        public CultureInfo CultureInfo { get; set; }

        public string BaseUrl { get; set; }
    }
}
