using System.Globalization;

namespace Orckestra.Composer.Country
{
    public class RetrieveRegionDisplayNameParam
    {
        /// <summary>
        /// The countryIsoCode (unique) that identifies the country of the region
        /// IsRequired
        /// </summary>
        public string IsoCode { get; set; }

        /// <summary>
        /// The Region IsoCode (unique) that identifies the region to retrieve
        /// IsRequired
        /// </summary>
        public string RegionCode { get; set; }

        /// <summary>
        /// The culture name in which language the data will be returned
        /// </summary>
        public CultureInfo CultureInfo { get; set; }
    }
}
