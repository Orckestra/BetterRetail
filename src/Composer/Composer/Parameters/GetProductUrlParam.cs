using System.Globalization;

namespace Orckestra.Composer.Parameters
{
    public class GetProductUrlParam
    {
        public string SKU { get; set; }
        public string ProductId { get; set; }

        public string ProductName { get; set; }

        public CultureInfo CultureInfo { get; set; }

        public string VariantId { get; set; }

        public string BaseUrl { get; set; }
    }
}
