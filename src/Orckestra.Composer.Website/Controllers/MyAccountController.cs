using Orckestra.Composer.Cart.Services;
using Orckestra.Composer.Cart.Services.Order;
using Orckestra.Composer.CompositeC1.Controllers;
using Orckestra.Composer.MyAccount.Services;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Repositories;
using Orckestra.Composer.Services;
using Orckestra.Composer.Configuration;

namespace Orckestra.Composer.Website.Controllers
{
    public class MyAccountController : MyAccountBaseController
    {
        public MyAccountController(
            ICustomerViewService customerViewService,
            ICustomerAddressViewService customerAddressViewService,
            IComposerContext composerContext,
            IAddressRepository addressRepository,
            IMyAccountUrlProvider myAccountUrlProvider,
            IOrderHistoryViewService orderHistoryViewService,
            IWishListViewService wishListViewService,
            IRecurringOrderTemplatesViewService recurringOrderTemplatesViewService,
            IRecurringOrdersSettings recurringOrdersSettings) : base(
                customerViewService,
                customerAddressViewService,
                composerContext, addressRepository,
                myAccountUrlProvider,
                orderHistoryViewService,
                wishListViewService,
                recurringOrderTemplatesViewService,
                recurringOrdersSettings)
        {
        }
    }
}
