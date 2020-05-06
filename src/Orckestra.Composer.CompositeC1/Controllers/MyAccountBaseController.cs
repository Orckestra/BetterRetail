using System;
using System.Net;
using System.Web.Mvc;
using Composite.Core.Xml;
using Orckestra.Composer.Cart;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Parameters.Order;
using Orckestra.Composer.Cart.Services;
using Orckestra.Composer.Cart.Services.Order;
using Orckestra.Composer.Cart.ViewModels;
using Orckestra.Composer.Cart.ViewModels.Order;
using Orckestra.Composer.Configuration;
using Orckestra.Composer.MvcFilters;
using Orckestra.Composer.MyAccount.Parameters;
using Orckestra.Composer.MyAccount.Services;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Repositories;
using Orckestra.Composer.Services;
using Orckestra.Composer.Utils;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.CompositeC1.Controllers
{
    [ValidateReturnUrl]
    public abstract class MyAccountBaseController : Controller
    {
        protected ICustomerViewService CustomerViewService { get; private set; }
        protected ICustomerAddressViewService CustomerAddressViewService { get; private set; }
        protected IComposerContext ComposerContext { get; private set; }
        protected IAddressRepository AddressRepository { get; private set; }
        protected IMyAccountUrlProvider MyAccountUrlProvider { get; private set; }
        protected IOrderHistoryViewService OrderHistoryViewService { get; private set; }
        protected IWishListViewService WishListViewService { get; private set; }
        protected IRecurringOrderTemplatesViewService RecurringOrderTemplatesViewService { get; private set; }
        protected IRecurringOrdersSettings RecurringOrdersSettings { get; private set; }

        protected MyAccountBaseController(
            ICustomerViewService customerViewService,
            ICustomerAddressViewService customerAddressViewService,
            IComposerContext composerContext,
            IAddressRepository addressRepository,
            IMyAccountUrlProvider myAccountUrlProvider,
            IOrderHistoryViewService orderHistoryViewService,
            IWishListViewService wishListViewService,
            IRecurringOrderTemplatesViewService recurringOrderTemplatesViewService,
            IRecurringOrdersSettings recurringOrdersSettings)
        {
            CustomerViewService = customerViewService ?? throw new ArgumentNullException(nameof(customerViewService));
            CustomerAddressViewService = customerAddressViewService ?? throw new ArgumentNullException(nameof(customerAddressViewService));
            ComposerContext = composerContext ?? throw new ArgumentNullException(nameof(composerContext));
            AddressRepository = addressRepository ?? throw new ArgumentNullException(nameof(addressRepository));
            MyAccountUrlProvider = myAccountUrlProvider ?? throw new ArgumentNullException(nameof(myAccountUrlProvider));
            OrderHistoryViewService = orderHistoryViewService ?? throw new ArgumentNullException(nameof(orderHistoryViewService));
            WishListViewService = wishListViewService ?? throw new ArgumentNullException(nameof(wishListViewService));
            RecurringOrderTemplatesViewService = recurringOrderTemplatesViewService ?? throw new ArgumentNullException(nameof(recurringOrderTemplatesViewService));
            RecurringOrdersSettings = recurringOrdersSettings ?? throw new ArgumentNullException(nameof(recurringOrdersSettings));
        }

        [AuthorizeAndRedirect]
        public virtual ActionResult AccountHeader()
        {
            var viewModel = CustomerViewService.GetAccountHeaderViewModelAsync(new GetAccountHeaderViewModelParam
            {
                Scope = ComposerContext.Scope,
                CultureInfo = ComposerContext.CultureInfo,
                CustomerId = ComposerContext.CustomerId
            }).Result;

            return View("AccountHeaderBlade", viewModel);
        }

        [AuthorizeAndRedirect]
        public virtual ActionResult UpdateAccount()
        {
            var urlParam = new BaseUrlParameter
            {
                CultureInfo = ComposerContext.CultureInfo
            };
            var changePasswordUrl = MyAccountUrlProvider.GetChangePasswordUrl(urlParam);
            var addressListUrl = MyAccountUrlProvider.GetAddressListUrl(urlParam);

            var param = new GetUpdateAccountViewModelParam
            {
                Scope = ComposerContext.Scope,
                CultureInfo = ComposerContext.CultureInfo,
                CustomerId = ComposerContext.CustomerId
            };

            var viewModel = CustomerViewService.GetUpdateAccountViewModelAsync(param).Result;
            viewModel.ChangePasswordUrl = changePasswordUrl;
            viewModel.AddressListUrl = addressListUrl;

            return View("UpdateAccountBlade", viewModel);
        }

        [AuthorizeAndRedirect]
        public virtual ActionResult CreateAddress()
        {
            var viewModel = CustomerAddressViewService.GetCreateAddressViewModelAsync(new GetCreateAddressViewModelAsyncParam
            {
                CustomerId = ComposerContext.CustomerId,
                CultureInfo = ComposerContext.CultureInfo,
                Scope = ComposerContext.Scope,
                CountryCode = ComposerContext.CountryCode
            }).Result;

            return View("EditAddressBlade", viewModel);
        }

        [AuthorizeAndRedirect]
        public virtual ActionResult EditAddress(Guid? addressId)
        {
            if (!addressId.HasValue)
            {
                return UnexpectedAddressForCustomer();
            }

            var vm = CustomerAddressViewService.GetEditAddressViewModelAsync(new GetEditAddressViewModelAsyncParam
            {
                AddressId = addressId.Value,
                CustomerId = ComposerContext.CustomerId,
                CultureInfo = ComposerContext.CultureInfo,
                Scope = ComposerContext.Scope,
            }).Result;
            if (vm == null)
            {
                return UnexpectedAddressForCustomer();
            }

            return View("EditAddressBlade", vm);
        }

        [AuthorizeAndRedirect]
        public virtual ActionResult CurrentOrders()
        {
            return View("CurrentOrdersContainer", GetOrderHistoryViewModel());
        }

        [AuthorizeAndRedirect]
        public virtual ActionResult PastOrders()
        {
            return View("PastOrdersContainer", GetOrderHistoryViewModel());
        }

        [AuthorizeAndRedirect]
        public virtual ActionResult OrderDetails(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            var vm = OrderHistoryViewService.GetOrderDetailViewModelAsync(new GetCustomerOrderParam
            {
                OrderNumber = id,
                CustomerId = ComposerContext.CustomerId,
                Scope = ComposerContext.Scope,
                CultureInfo = ComposerContext.CultureInfo,
                CountryCode = ComposerContext.CountryCode,
                BaseUrl = RequestUtils.GetBaseUrl(Request).ToString()
            }).Result;

            if (vm == null)
            {
                return new HttpUnauthorizedResult();
            }
            return View("OrderDetailsContainer", vm);
        }

        [AuthorizeAndRedirect]
        public virtual ActionResult WishList(XhtmlDocument emptyWishListContent)
        {
            var vm = WishListViewService.GetWishListViewModelAsync(new GetCartParam
            {
                Scope = ComposerContext.Scope,
                CultureInfo = ComposerContext.CultureInfo,
                CustomerId = ComposerContext.CustomerId,
                CartName = CartConfiguration.WishlistCartName,
                ExecuteWorkflow = CartConfiguration.WishListExecuteWorkflow,
                WorkflowToExecute = CartConfiguration.WishListWorkflowToExecute,
                BaseUrl = RequestUtils.GetBaseUrl(Request).ToString()
            }).Result;

            if (vm != null && vm.TotalQuantity == 0 && emptyWishListContent != null)
            {
                return View("WishListContainer", new {TotalQuantity = 0, EmptyContent = emptyWishListContent.Body});
            }

            return View("WishListContainer", vm);
        }

        [AuthorizeAndRedirect]
        public virtual ActionResult RecurringSchedule()
        {
            var vm = RecurringOrderTemplatesViewService.GetRecurringOrderTemplatesViewModelAsync(new GetRecurringOrderTemplatesParam {
                Scope = ComposerContext.Scope,
                CustomerId = ComposerContext.CustomerId,
                CultureInfo = ComposerContext.CultureInfo,
                BaseUrl = RequestUtils.GetBaseUrl(Request).ToString()
                }).Result;

            if (vm != null && vm.RecurringOrderTemplateViewModelList.Count == 0)
            {
                return View("RecurringScheduleContainer", new { TotalQuantity = 0 });
            }

            return View("RecurringScheduleContainer", vm);            
        }

        [AuthorizeAndRedirect]
        public virtual ActionResult RecurringScheduleDetails(string id)
        {
            return View("RecurringScheduleDetailsContainer", GetEmptyTemplateViewModel());
        }

        [AuthorizeAndRedirect]
        public virtual ActionResult UpcomingOrders()
        {
			if (!RecurringOrdersSettings.Enabled)
                return new EmptyResult();
			
            return View("RecurringCartsContainer", GetUpcomingOrdersViewModel());
        }

        [AuthorizeAndRedirect]
        public virtual ActionResult RecurringCartDetails(string name)
        {
            var vm = new CartViewModel
            {
                IsLoading = true
            };

            return View("RecurringCartDetailsContainer", vm);
        }        

        protected virtual OrderHistoryViewModel GetOrderHistoryViewModel()
        {
            return new OrderHistoryViewModel
            {
                IsLoading = true
            };
        }


        protected virtual ActionResult UnexpectedAddressForCustomer()
        {
            return new HttpUnauthorizedResult();
        }

        protected virtual LightRecurringOrderCartsViewModel GetUpcomingOrdersViewModel()
        {
            return new LightRecurringOrderCartsViewModel
            {
                IsLoading = true
            };
        }
        protected virtual RecurringOrderTemplatesViewModel GetEmptyTemplateViewModel()
        {
            return new RecurringOrderTemplatesViewModel
            {
                IsLoading = true
            };
        }
    }
}
