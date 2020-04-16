using System.Globalization;
using Orckestra.Overture.ServiceModel.Customers;

namespace Orckestra.Composer.MyAccount.Parameters
{
    /// <summary>
    /// Builder params for creating the <see cref="Orckestra.Composer.MyAccount.ViewModels.ResetPasswordViewModel"/>
    /// </summary>
    public class GetResetPasswordViewModelParam
    {
        /// <summary>
        /// (Mandatory)
        /// The culture to use for any displayed values.
        /// </summary>
        public CultureInfo CultureInfo { get; set; }

        /// <summary>
        /// (Optional) 
        /// The Customer targeted by the reset password ticket and results.
        /// </summary>
        public Customer Customer { get; set; }

        /// <summary>
        /// (Optional) 
        /// The reset password results
        /// </summary>
        public MyAccountStatus? Status { get; set; }

        /// <summary>
        /// (Optional) 
        /// ReturnUrl to be used on client side to redirect on success
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        /// (Mandatory)
        /// Encrypted password reset ticket send to the Customer using the
        /// SendResetPasswordInstruction method.
        /// </summary>
        public string Ticket { get; set; }

        public string ForgotPasswordUrl { get; set; }
    }
}
