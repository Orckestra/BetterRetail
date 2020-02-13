using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Cart.ViewModels
{
    public sealed class CheckoutSignInViewModel: BaseViewModel
    {
        public CheckoutSignInAsGuestViewModel GuestCustomer { get; set; }

        public CheckoutSignInAsReturningViewModel ReturningCustomer { get; set; }
    }
}
