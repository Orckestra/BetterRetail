using System.Globalization;
using Orckestra.Composer.MyAccount.ViewModels;
using Orckestra.Overture.ServiceModel;

namespace Orckestra.Composer.MyAccount.Parameters
{
    /// <summary>
    /// Builder params for creating the <see cref="EditAddressViewModel"/>
    /// </summary>
    public class GetEditAddressViewModelParam
    {
        /// <summary>
        /// True if view model will be used for editing an existing address;
        /// false if view model will be used to create a new address
        /// </summary>
        public bool IsUpdating { get; set; }

        /// <summary>
        /// (Mandatory)
        /// The culture to use for any displayed values.
        /// </summary>
        public CultureInfo CultureInfo { get; set; }

        /// <summary>
        /// (Optional) 
        /// The address to edit (if any) or null to create a new one
        /// </summary>
        public Address Address { get; set; }

        /// <summary>
        /// (Optional) 
        /// The update address form results.
        /// </summary>
        public MyAccountStatus? Status { get; set; }

        /// <summary>
        /// (Optional) 
        /// ReturnUrl to be used on client side to redirect on success
        /// </summary>
        public string ReturnUrl { get; set; }
    }
}
