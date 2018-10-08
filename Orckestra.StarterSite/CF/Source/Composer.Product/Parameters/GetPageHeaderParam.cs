using System.Globalization;

namespace Orckestra.Composer.Product.Parameters
{
    public class GetPageHeaderParam
    {
        public string Scope { get; set; }

        public CultureInfo CultureInfo { get; set; }

        public string  ProductId { get; set; }

        public string BaseUrl { get; set; }
    }
}
