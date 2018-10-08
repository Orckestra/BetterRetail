using System.Collections.Generic;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Store.ViewModels
{
    public sealed class StoreStructuredDataViewModel : BaseViewModel
    {
        public string Name { get; set; }

        public string Telephone { get; set; }

        public string StreetAddress { get; set; }

        public string AddressLocality { get; set; }

        public string AddressRegion { get; set; }

        public string PostalCode { get; set; }

        public string AddressCountry { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public string Url { get; set; }
        public List<StructuredDataOpeningHoursSpecificationViewModel> OpeningHoursSpecifications { get; set; }

    }
}
