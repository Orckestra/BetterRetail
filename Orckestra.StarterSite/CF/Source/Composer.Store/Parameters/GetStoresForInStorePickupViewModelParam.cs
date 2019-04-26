using System.Globalization;

namespace Orckestra.Composer.Store.Parameters
{
    public class GetStoresForInStorePickupViewModelParam
    {
        public string Scope { get; set; }
        public CultureInfo CultureInfo { get; set; }
        public string BaseUrl { get; set; }
    }
}
