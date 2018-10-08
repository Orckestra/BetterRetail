using System.Globalization;

namespace Orckestra.Composer.MyAccount.Parameters
{
    /// <summary>
    /// Service call params to send instruction when a Customer forgot it's password
    /// </summary>
    public class ForgotPasswordParam
    {
        /// <summary>
        /// (Mandatory)
        /// The unique Email address of the Customer who needs instructions
        /// </summary>
        public string Email { get; set; }

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

    }
}
