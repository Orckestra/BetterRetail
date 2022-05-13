using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.MyAccount.ViewModels
{
    /// <summary>
    /// ViewModel to display the Reset Password Form
    /// </summary>
    public sealed class ResetPasswordViewModel : PasswordPatternViewModel
    {
        /// <summary>
        ///  The FirstName for the User account
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        ///  The LastName for the User account
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Change password Status result (Success, InvalidToken, Failed)
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// ReturnUrl to be used on client side to redirect on success
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        /// Url to the Forgot Password page
        /// </summary>
        public string ForgotPasswordUrl { get; set; }
    }
}
