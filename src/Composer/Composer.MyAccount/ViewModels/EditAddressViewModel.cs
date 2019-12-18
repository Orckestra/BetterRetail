using System;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.MyAccount.ViewModels
{
    /// <summary>
    /// ViewModel to display the MyAddress Creation/Edition Form or Form result
    /// </summary>
    public sealed class EditAddressViewModel : BaseViewModel
    {
        /// <summary>
        /// Unique Id for this address
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Human readable name set by the user to identify his addresses
        /// </summary>
        public string AddressName { get; set; }

        /// <summary>
        /// 2 letter country ISO code
        /// </summary>
        public string CountryCode { get; set; }

        /// <summary>
        /// 2 letter region ISO code
        /// </summary>
        public string RegionCode { get; set; }

        /// <summary>
        /// The region name
        /// </summary>
        public string RegionName { get; set; }

        /// <summary>
        /// Postal code of the address
        /// </summary>
        public string PostalCode { get; set; }

        /// <summary>
        /// Postal Code Regex to check format
        /// </summary>
        public string PostalCodeRegex { get; set; }

        /// <summary>
        /// First Name of the address
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Last Name of the address
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Line1 of the address
        /// </summary>
        public string Line1 { get; set; }

        /// <summary>
        /// Line2 of the address
        /// </summary>
        public string Line2 { get; set; }

        /// <summary>
        /// City of the address
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// PhoneNumber of the address
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Phone Number regular expression to validate format
        /// </summary>
        public string PhoneRegex { get; set; }

        /// <summary>
        /// Is this address entry the prefered billing address
        /// </summary>
        public bool IsPreferredBilling { get; set; }

        /// <summary>
        /// Is this address entry the prefered shipping address
        /// </summary>
        public bool IsPreferredShipping { get; set; }

        /// <summary>
        /// True if view model will be used for editing an existing address;
        /// false if view model will be used to create a new address
        /// </summary>
        public bool IsUpdating { get; set; }

        /// <summary>
        /// Update address Status result (Success, InvalidPostalCode, Failed, ...)
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// ReturnUrl to be used on client side to redirect on success
        /// </summary>
        public string ReturnUrl { get; set; }
    }
}
