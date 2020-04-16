using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Store.ViewModels
{
    public sealed class StoreAddressViewModel : BaseViewModel
    {
        public string City { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public string RegionCode { get; set; }
        public string RegionName { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string PostalCode { get; set; }
        public string PhoneNumber { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}
