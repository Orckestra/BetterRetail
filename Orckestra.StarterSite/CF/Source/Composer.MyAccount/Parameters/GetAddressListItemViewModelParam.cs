using System.Globalization;
using Orckestra.Overture.ServiceModel;

namespace Orckestra.Composer.MyAccount.Parameters
{
    /// <summary>
    /// Builder params for creating the <see cref="Orckestra.Composer.MyAccount.ViewModels.AddressListViewModel"/>
    /// </summary>
    public class GetAddressListItemViewModelParam
    {
        /// <summary>
        /// (Mandatory)
        /// The culture to use for any displayed values.
        /// </summary>
        public CultureInfo CultureInfo { get; set; }

        /// <summary>
        /// (Mandatory) 
        /// The address to display
        /// </summary>
        public Address Address { get; set; }
    }
}
