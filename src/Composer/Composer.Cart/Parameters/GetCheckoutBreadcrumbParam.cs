using System.Globalization;

namespace Orckestra.Composer.Cart.Parameters
{
    public class GetCheckoutBreadcrumbParam
    {
        public string HomeUrl { get; set; }

        public CultureInfo CultureInfo { get; set; }
    }
}
