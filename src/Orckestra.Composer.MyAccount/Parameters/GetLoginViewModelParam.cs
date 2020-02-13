using System.Globalization;
using Orckestra.Overture.ServiceModel.Customers;

namespace Orckestra.Composer.MyAccount.Parameters
{
    /// <summary>
    /// Builder params for creating the <see cref="Orckestra.Composer.MyAccount.ViewModels.LoginViewModel"/>
    /// </summary>
    public class GetLoginViewModelParam
    {
        /// <summary>
        /// (Mandatory)
        /// The culture to use for any displayed values.
        /// </summary>
        public CultureInfo CultureInfo { get; set; }

        /// <summary>
        /// (Optional) 
        /// The authenticated Customer.
        /// </summary>
        public Customer Customer { get; set; }

        /// <summary>
        /// (Optional)
        /// The login results.
        /// </summary>
        public MyAccountStatus? Status { get; set; }

        /// <summary>
        /// (Optional) 
        /// ReturnUrl to be used on client side to redirect on success
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        /// (Optional) 
        /// The authenticated Customer username.
        /// </summary>
        public string Username { get; set; }


        public string LoginUrl { get; set; }
        public string CreateAccountUrl { get; set; }
        public string ForgotPasswordUrl { get; set; }
    }
}