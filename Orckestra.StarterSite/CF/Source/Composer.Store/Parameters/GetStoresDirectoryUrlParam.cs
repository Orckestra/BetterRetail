using System.Globalization;

namespace Orckestra.Composer.Store.Parameters
{
    public class GetStoresDirectoryUrlParam
    {
        public string BaseUrl { get; set; }
        public CultureInfo CultureInfo { get; set; }
        public int Page { get; set; }
    }
}
