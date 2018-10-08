using System.Globalization;

namespace Orckestra.Composer.MyAccount.Parameters
{
    /// <summary>
    /// Service call params to reset the password for a given Customer
    /// </summary>
    public class ResetPasswordParam
    {
        /// <summary>
        /// (Mandatory)
        /// Encrypted password reset ticket send to the Customer using the
        /// SendResetPasswordInstruction method.
        /// </summary>
        public string Ticket { get; set; }

        /// <summary>
        /// (Mandatory)
        /// The culture to use for any displayed values
        /// </summary>
        public CultureInfo CultureInfo { get; set; }

        /// <summary>
        /// (Mandatory)
        /// The scope responsible for this request, to which the Customer must belong
        /// </summary>
        public string Scope { get; set; }

        /// <summary>
        /// (Mandatory)
        /// The new password to set
        /// </summary>
        public string NewPassword { get; set; }

        /// <summary>
        /// (Optional)
        /// The answer to the password question
        /// </summary>
        public string PasswordAnswer { get; set; }

        /// <summary>
        /// (Optional) 
        /// ReturnUrl to be used on client side to redirect on success
        /// </summary>
        public string ReturnUrl { get; set; }
    }
}
