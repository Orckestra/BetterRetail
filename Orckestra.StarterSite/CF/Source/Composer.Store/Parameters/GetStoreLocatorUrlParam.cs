using Orckestra.Composer.Parameters;
using System.Globalization;

namespace Orckestra.Composer.Store.Parameters
{
    public class GetStoreLocatorUrlParam: BaseUrlParameter
    {
        public string BaseUrl { get; set; }

        public int Page { get; set; }
    }
}
