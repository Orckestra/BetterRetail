using System.Globalization;

namespace Orckestra.Composer.Parameters
{
    public class GetProductParam
    {
        public string Scope { get; set; }
        public CultureInfo CultureInfo { get; set; }
        public string  ProductId { get; set; }
        public string VariantId { get; set; }
        public string BaseUrl { get; set; }
        public bool ReturnInactive { get; set; }
    }
}
