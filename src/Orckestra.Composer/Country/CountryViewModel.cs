using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Country
{
    public sealed class CountryViewModel : BaseViewModel
    {

        /// <summary>
        /// The official political name given for the Country
        /// </summary>
        public string CountryName { get; set; }

        /// <summary>
        /// Two letter code that identifies Country uniquely
        /// </summary>
        public string IsoCode { get; set; }

        /// <summary>
        /// The indicator specifying if the Country is supported by the merchant
        /// </summary>
        public bool IsSupported { get; set; }

        /// <summary>
        /// The Country's postal code validation regular expression
        /// </summary>
        public string PostalCodeRegex { get; set; }

        /// <summary>
        /// The country's phone number validation regular expression
        /// </summary>
        public string PhoneRegex { get; set; }
    }
}
