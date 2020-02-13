using System;
using System.Globalization;

namespace Orckestra.Composer.MyAccount.Parameters
{
    /// <summary>
    /// Builder params for creating the <see cref="Orckestra.Composer.MyAccount.ViewModels.AddressListViewModel"/>
    /// </summary>
    public class GetAddressListViewModelParam
    {
        /// <summary>
        /// (Mandatory)
        /// The culture to use for any displayed values.
        /// </summary>
        public CultureInfo CultureInfo { get; set; }

        /// <summary>
        /// (Mandatory)
        /// The scope to display values from.
        /// </summary>
        public string Scope { get; set; }


        /// <summary>
        /// (Mandatory) 
        /// The cutomer to whom belong the addresses
        /// </summary>
        public Guid CustomerId { get; set; }

        /// <summary>
        /// (Mandatory) 
        /// The add address Url.
        /// </summary>
        public string AddAddressUrl { get; set; }

        /// <summary>
        /// (Mandatory) 
        /// The base url for Edit Address.
        /// </summary>
        public string EditAddressBaseUrl { get; set; }

        /// <summary>
        /// (Mandatory) 
        /// The country where to get the regions
        /// </summary>
        public string CountryCode { get; set; }
    }
}
