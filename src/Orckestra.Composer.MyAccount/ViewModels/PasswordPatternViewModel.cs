using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.MyAccount.ViewModels
{
    /// <summary>
    /// ViewModel to display the PasswordPattern
    /// </summary>
    public class PasswordPatternViewModel : BaseViewModel
    {
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
    }
}
