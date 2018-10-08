using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.ViewModels;

namespace Orckestra.Composer.Cart.Services
{
    public interface ICheckoutNavigationViewService
    {
        /// <summary>
        /// Get the view model for the checkout Navigation.
        /// </summary>
        /// <param name="param">param</param>
        /// <returns>The CheckoutNavigationViewModel</returns>
        CheckoutNavigationViewModel GetCheckoutNavigationViewModel(GetCheckoutNavigationParam param);
    }
}
