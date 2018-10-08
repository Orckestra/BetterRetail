using System.Globalization;

namespace Orckestra.Composer.MyAccount.Parameters
{
    /// <summary>
    /// Builder params for creating the <see cref="Orckestra.Composer.MyAccount.ViewModels.ForgotPasswordViewModel"/>
    /// </summary>
    public class GetForgotPasswordViewModelParam
    {
        /// <summary>
        /// (Mandatory)
        /// The culture to use for any displayed values.
        /// </summary>
        public CultureInfo CultureInfo { get; set; }

        /// <summary>
        /// (Optional) 
        /// On success, Email where the forgot password instructions were sent.
        /// </summary>
        public string EmailSentTo { get; set; }

        /// <summary>
        /// (Optional) 
        /// The forgot password results.
        /// </summary>
        public MyAccountStatus? Status { get; set; }
    }
}
