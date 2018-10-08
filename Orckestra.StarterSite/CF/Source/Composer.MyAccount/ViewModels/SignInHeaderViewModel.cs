using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.MyAccount.ViewModels
{
    /// <summary>
    /// ViewModel to display the SignIn Header
    /// </summary>
    public sealed class SignInHeaderViewModel : BaseViewModel
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
        /// Localized Url for log in page (if not logged in) or to My Account page (if logged in)
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Indicates if customer is logged in or not
        /// </summary>
        public bool IsLoggedIn { get; set; }

        /// <summary>
        /// The encrypted CustomerId
        /// </summary>
        public string EncryptedCustomerId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is loading.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is loading; otherwise, <c>false</c>.
        /// </value>
        public bool IsLoading { get; set; }
    }
}