using Orckestra.Composer.Cart.Services;
using Orckestra.Composer.Cart.Services.Order;
using Orckestra.Composer.CompositeC1.Controllers;
using Orckestra.Composer.MyAccount.Services;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Repositories;
using Orckestra.Composer.Services;

namespace Orckestra.Composer.CompositeC1.Mvc.Controllers
{
    public class MyAccountController : MyAccountBaseController
    {
        public MyAccountController(
            ICustomerViewService customerViewService,
            ICustomerAddressViewService customerAddressViewService,
            IPaymentViewService paymentViewService,
            IComposerContext composerContext,
            IAddressRepository addressRepository,
            IMyAccountUrlProvider myAccountUrlProvider,
            IOrderUrlProvider orderUrlProvider,
            IMyAccountViewService myAccountViewService,
            IOrderHistoryViewService orderHistoryViewService,
            IWishListViewService wishListViewService) : base(
                customerViewService,
                customerAddressViewService,
                paymentViewService,
                composerContext, addressRepository,
                myAccountUrlProvider,
                orderUrlProvider,
                myAccountViewService,
                orderHistoryViewService,
                wishListViewService)
        {
        }
    }
}
