using System.Globalization;

namespace Orckestra.Composer.Search.Parameters
{
    public class GetSearchBreadcrumbParam
    {
        public string Keywords { get; set; }

        public string HomeUrl { get; set; }

        public CultureInfo CultureInfo { get; set; }
    }
}
