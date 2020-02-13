using System;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.MyAccount.ViewModels
{
    /// <summary>
    /// ViewModel to display the Create Account Form
    /// </summary>
    public sealed class CreateAccountViewModel : BaseViewModel
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
        /// Creation Status result (Success, DuplicateEmail, DuplicateUserName, UserRejected, Failed, ...)
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// ReturnUrl to be used on client side to redirect on success
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        /// The Minimum Required Password Length for the Password
        /// </summary>
        public int MinRequiredPasswordLength { get; set; }

        /// <summary>
        /// The Minimum Required Non Alphanumeric Characets for the Password
        /// The accepted non alphanumeric characeters are: !@#$%^&amp;*()_-+=[{]};:&gt;&lt;|./?
        /// </summary>
        public int MinRequiredNonAlphanumericCharacters { get; set; }

        /// <summary>
        /// A regex for validating the accepted password pattern
        /// </summary>
        public string PasswordRegexPattern { get; set; }

        /// <summary>
        /// Url to the terms and conditions page
        /// </summary>
        public string TermsAndConditionsUrl { get; set; }

        /// <summary>
        /// The Customer Username
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// The Customer Id
        /// </summary>
        public Guid CustomerId { get; set; }

        /// <summary>
        /// Information if the login is a success
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// the date the customer was created
        /// </summary>
        public DateTime Created { get; set; }
    }
}
