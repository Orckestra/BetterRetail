using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.MyAccount.ViewModels
{
    /// <summary>
    /// ViewModel to display the User Information
    /// </summary>
    public sealed class UserMetadataViewModel : BaseViewModel
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
        ///  Url for Register page
        /// </summary>
        public string RegisterUrl { get; set; }

        /// <summary>
        /// Indicates if customer is logged in or not
        /// </summary>
        public bool IsAuthenticated { get; set; }

        /// <summary>
        /// The encrypted CustomerId
        /// </summary>
        public string EncryptedCustomerId { get; set; }

        /// <summary>
        /// The use Email as Username flag
        /// </summary>
        public bool UseEmailAsUsername { get; set; }

    }
}