using System.Globalization;

namespace Orckestra.Composer.Country
{
    public class RetrieveCountryParam
    {
        /// <summary>
        /// The countryIsoCode (unique) that identifies the country to retrieve
        /// IsRequired
        /// </summary>
        public string IsoCode { get; set; }

        /// <summary>
        /// The culture name in which language the data will be returned
        /// </summary>
        public CultureInfo CultureInfo { get; set; }
    }
}
