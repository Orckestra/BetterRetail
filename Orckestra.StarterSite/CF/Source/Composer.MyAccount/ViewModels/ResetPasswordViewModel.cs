using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.MyAccount.ViewModels
{
    /// <summary>
    /// ViewModel to display the Reset Password Form
    /// </summary>
    public sealed class ResetPasswordViewModel : BaseViewModel
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
        /// The Minimum Required Password Length for the New Password
        /// </summary>
        public int MinRequiredPasswordLength { get; set; }

        /// <summary>
        /// The Minimum Required Non Alphanumeric Characets for the New Password
        /// The accepted non alphanumeric characeters are: !@#$%^&amp;*()_-+=[{]};:&gt;&lt;|./?
        /// </summary>
        public int MinRequiredNonAlphanumericCharacters { get; set; }

        /// <summary>
        /// A regex for validating the accepted password pattern
        /// </summary>
        public string PasswordRegexPattern { get; set; }

        /// <summary>
        /// Url to the Forgot Password page
        /// </summary>
        public string ForgotPasswordUrl { get; set; }
    }
}
