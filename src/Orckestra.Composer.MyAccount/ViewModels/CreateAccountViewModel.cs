using System;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.MyAccount.ViewModels
{
    /// <summary>
    /// ViewModel to display the Create Account Form
    /// </summary>
    public sealed class CreateAccountViewModel : PasswordPatternViewModel
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

        /// <summary>
        /// The PhoneNumber for the User account
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// The PhoneNumber RegEx validation rule
        /// </summary>
        public string PhoneNumberRegEx { get; set; }

        /// <summary>
        /// The use Email as Username flag
        /// </summary>
        public bool UseEmailAsUsername { get; set; }
    }
}
