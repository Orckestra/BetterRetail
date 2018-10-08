using System.Globalization;

namespace Orckestra.Composer.Product.Parameters
{
    public class GetProductIdentifiersParam
    {
        public CultureInfo CultureInfo { get; set; }
        public string ProductId { get; set; }
        public string Scope { get; set; }
        public string[] MerchandiseTypes { get; set; }
        public bool FallbackOnSameCategoriesProduct { get; set; }
        public int MaxItems { get; set; }
    }
}