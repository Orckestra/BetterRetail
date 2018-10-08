using System.Globalization;
using Orckestra.Overture.ServiceModel.Customers;

namespace Orckestra.Composer.MyAccount.Parameters
{
    /// <summary>
    /// Builder params for creating the <see cref="Orckestra.Composer.MyAccount.ViewModels.CreateAccountViewModel"/>
    /// </summary>
    public class GetCreateAccountViewModelParam
    {
        /// <summary>
        /// (Mandatory)
        /// The culture to use for any displayed values
        /// </summary>
        public CultureInfo CultureInfo { get; set; }

        /// <summary>
        /// (Optional)
        /// The created Customer
        /// </summary>
        public Customer Customer { get; set; }

        /// <summary>
        /// (Optional) 
        /// The creation results
        /// </summary>
        public MyAccountStatus? Status { get; set; }

        /// <summary>
        /// (Optional) 
        /// ReturnUrl to be used on client side to redirect on success
        /// </summary>
        public string ReturnUrl { get; set; }

        public string TermsAndConditionsUrl { get; set; }
    }
}
