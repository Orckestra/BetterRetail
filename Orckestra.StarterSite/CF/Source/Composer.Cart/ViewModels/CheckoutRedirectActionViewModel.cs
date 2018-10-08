using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Cart.ViewModels
{
    public sealed class CheckoutRedirectActionViewModel : BaseViewModel
    {

        /// <summary>
        /// The last checkout step completed by the user.
        /// </summary>
        public int LastCheckoutStep { get; set; }

        /// <summary>
        /// The redirect url.
        /// </summary>
        public string RedirectUrl { get; set; }
    }
}
