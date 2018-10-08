using System;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.MyAccount.ViewModels
{
    /// <summary>
    /// ViewModel to display the Login Form
    /// </summary>
    public sealed class LoginViewModel : BaseViewModel
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
        /// Login attempt Status result (Success, InvalidPassword, Failed, ...)
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// ReturnUrl to be used on client side to redirect on success
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        /// Url to the login page
        /// </summary>
        public string LoginUrl { get; set; }

        /// <summary>
        /// Url to the Create Account page
        /// </summary>
        public string CreateAccountUrl { get; set; }

        /// <summary>
        /// Url to the Forgot Password page
        /// </summary>
        public string ForgotPasswordUrl { get; set; }

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


    }
}
