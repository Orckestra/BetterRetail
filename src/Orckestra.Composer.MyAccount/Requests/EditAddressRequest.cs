using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Orckestra.Composer.MyAccount.Requests
{
    /// <summary>
    /// WebApi Request to Create or Update the information relative to one of the address for the currently logged in User
    /// </summary>
    public class EditAddressRequest
    {
        public EditAddressRequest()
        {
            AdditionnalProperties = new Dictionary<string, object>();
        }

        /// <summary>
        /// Human readable name set by the user to identify his addresses
        /// </summary>
        [Required]
        public string AddressName { get; set; }

        /// <summary>
        /// 2 letter country ISO code
        /// </summary>
        [Required]
        public string CountryCode { get; set; }

        /// <summary>
        /// 2 letter region ISO code
        /// </summary>
        [Required]
        public string RegionCode { get; set; }

        /// <summary>
        /// The region name
        /// </summary>
        public string RegionName { get; set; }

        /// <summary>
        /// Postal code of the address
        /// </summary>
        [Required]
        public string PostalCode { get; set; }

        /// <summary>
        /// First Name of the address
        /// </summary>
        [Required]
        public string FirstName { get; set; }

        /// <summary>
        /// Last Name of the address
        /// </summary>
        [Required]
        public string LastName { get; set; }

        /// <summary>
        /// Line1 of the address
        /// </summary>
        [Required]
        public string Line1 { get; set; }

        /// <summary>
        /// Line2 of the address
        /// </summary>
        public string Line2 { get; set; }

        /// <summary>
        /// City of the address
        /// </summary>
        [Required]
        public string City { get; set; }

        /// <summary>
        /// PhoneNumber of the address
        /// </summary>
        [Required]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Should this address be set as the default shipping address
        /// </summary>
        public bool IsPreferredShipping { get; set; }

        /// <summary>
        /// Should this address be set as the default billing address
        /// </summary>
        public bool IsPreferredBilling { get; set; }

        /// <summary>
        /// Additionnal custom properties to bind to the user
        /// this can be bound using &lt;input name=&quot;AdditionnalProperties[<em>somePropertyName</em>]&quot; /&gt;
        /// </summary>
        public Dictionary<string, object> AdditionnalProperties { get; set; }

        /// <summary>
        /// (Optional)
        /// ReturnUrl to be used on client side to redirect after creating the address
        /// </summary>
        public string ReturnUrl { get; set; }
    }
}