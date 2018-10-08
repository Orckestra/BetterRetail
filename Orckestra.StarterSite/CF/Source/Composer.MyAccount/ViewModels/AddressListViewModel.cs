using System.Collections.Generic;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.MyAccount.ViewModels
{
    /// <summary>
    /// ViewModel to display the Address list in the MyAccount setion
    /// </summary>
    public sealed class AddressListViewModel :  BaseViewModel
    {
        public AddressListViewModel()
        {
            Addresses = new List<AddressListItemViewModel>();
        }

        /// <summary>
        ///  The FirstName for the User account
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        ///  The LastName for the User account
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Url to add a new address to the customer's profile
        /// </summary>
        public string AddAddressUrl { get; set; }

        /// <summary>
        /// List of address items
        /// </summary>
        public List<AddressListItemViewModel> Addresses { get; set; }
    }
}
