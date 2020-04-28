using System;
using System.ComponentModel.DataAnnotations;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Cart.ViewModels
{
    public sealed class AddressViewModel : BaseViewModel
    {
        //The unique identifier of the address in Overture
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the Id of the address in the customer's address book
        /// </summary>
        public Guid? AddressBookId { get; set; }

        /// <summary>
        /// Human readable name set by the user to identify his addresses
        /// </summary>
        public string AddressName { get; set; }

        /// <summary>
        /// 2 letter country ISO code
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string CountryCode { get; set; }

        /// <summary>
        /// 2 letter region ISO code
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string RegionCode { get; set; }

        /// <summary>
        /// The region name
        /// </summary>
        public string RegionName { get; set; }

        /// <summary>
        /// Postal code of the address
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string PostalCode { get; set; }

        /// <summary>
        /// The regular expression for the postal/zip code
        /// </summary>
        public string PostalCodeRegexPattern { get; set; }

        /// <summary>
        /// First Name of the address
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string FirstName { get; set; }

        /// <summary>
        /// Last Name of the address
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string LastName { get; set; }

        /// <summary>
        /// Line1 of the address
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string Line1 { get; set; }

        /// <summary>
        /// Line2 of the address
        /// </summary>
        public string Line2 { get; set; }

        /// <summary>
        /// City of the address
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string City { get; set; }

        /// <summary>
        /// PhoneNumber of the address
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string PhoneNumber { get; set; }

        public string PhoneNumberFormated { get; set; }

        /// <summary>
        /// Phone Number regular expression to validate format
        /// </summary>
        public string PhoneRegex { get; set; }
    }
}
