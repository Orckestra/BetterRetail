using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.MyAccount.ViewModels
{
    /// <summary>
    /// ViewModel to display the Forgot Password Form
    /// </summary>
    public sealed class ForgotPasswordViewModel : BaseViewModel
    {
        /// <summary>
        /// Sending instruction Status result (Success, Failed, ...)
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// The unique Email address of the Customer where the instructions were sent
        /// </summary>
        public string EmailSentTo { get; set; }
    }
}
