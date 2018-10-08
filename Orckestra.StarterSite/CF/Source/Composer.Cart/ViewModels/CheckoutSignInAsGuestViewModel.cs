using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Cart.ViewModels
{
    public sealed class CheckoutSignInAsGuestViewModel : BaseViewModel
    {
        /// <summary>
        /// The Checkout URL to target
        /// </summary>
        public string CheckoutUrlTarget { get; set; }

        /// <summary>
        /// The Register Page Url.
        /// </summary>
        public string RegisterUrl { get; set; }
    }
}
