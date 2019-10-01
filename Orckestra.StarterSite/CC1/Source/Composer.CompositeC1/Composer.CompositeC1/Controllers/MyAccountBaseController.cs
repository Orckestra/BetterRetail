using System;
using System.Net;
using System.Web.Mvc;
using Composite.Core.Xml;
using Composite.Data;
using Orckestra.Composer.Cart;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Parameters.Order;
using Orckestra.Composer.Cart.Services;
using Orckestra.Composer.Cart.Services.Order;
using Orckestra.Composer.Cart.ViewModels;
using Orckestra.Composer.Cart.ViewModels.Order;
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
        protected IOrderUrlProvider OrderUrlProvider { get; private set; }
        protected IMyAccountViewService MyAccountViewService { get; private set; }
        protected IOrderHistoryViewService OrderHistoryViewService { get; private set; }
        protected IWishListViewService WishListViewService { get; private set; }
        protected IRecurringOrderTemplatesViewService RecurringOrderTemplatesViewService { get; private set; }
        protected IRecurringOrderCartsViewService RecurringOrderCartsViewService { get; private set; }

        protected MyAccountBaseController(
            ICustomerViewService customerViewService,
            ICustomerAddressViewService customerAddressViewService,
            IComposerContext composerContext,
            IAddressRepository addressRepository,
            IMyAccountUrlProvider myAccountUrlProvider,
            IOrderUrlProvider orderUrlProvider,
            IMyAccountViewService myAccountViewService,
            IOrderHistoryViewService orderHistoryViewService,
            IWishListViewService wishListViewService,
            IRecurringOrderTemplatesViewService recurringOrderTemplatesViewService,
            IRecurringOrderCartsViewService recurringOrderCartsViewService)
        {
            if (customerViewService == null) throw new ArgumentNullException("customerViewService");
            if (customerAddressViewService == null) throw new ArgumentNullException("customerAddressViewService");
            if (composerContext == null) throw new ArgumentNullException("composerContext");
            if (addressRepository == null) throw new ArgumentNullException("addressRepository");
            if (myAccountUrlProvider == null) throw new ArgumentNullException("myAccountUrlProvider");
            if (orderUrlProvider == null) throw new ArgumentNullException("orderUrlProvider");
            if (myAccountViewService == null) throw new ArgumentNullException("myAccountViewService");
            if (orderHistoryViewService == null) throw new ArgumentNullException("orderHistoryViewService");
            if (wishListViewService == null) throw new ArgumentNullException("wishListViewService");
            if (recurringOrderTemplatesViewService == null) throw new ArgumentNullException("recurringOrderTemplatesViewService");
            if (recurringOrderCartsViewService == null) throw new ArgumentNullException("recurringOrderCartsViewService");

            CustomerViewService = customerViewService;
            CustomerAddressViewService = customerAddressViewService;
            ComposerContext = composerContext;
            AddressRepository = addressRepository;
            MyAccountUrlProvider = myAccountUrlProvider;
            OrderUrlProvider = orderUrlProvider;
            MyAccountViewService = myAccountViewService;
            OrderHistoryViewService = orderHistoryViewService;
            WishListViewService = wishListViewService;
            RecurringOrderTemplatesViewService = recurringOrderTemplatesViewService;
            RecurringOrderCartsViewService = recurringOrderCartsViewService;
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
        public virtual ActionResult MyAccountMenu()
        {
            var currentPageId = SitemapNavigator.CurrentPageId;
            var menuViewModel = MyAccountViewService.CreateMenu(currentPageId.ToString());

            return View("MyAccountMenuBlade", menuViewModel);
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

            var viewModel = CustomerViewService.GetUpdateAccountViewModelAsync(new GetUpdateAccountViewModelParam
            {
                Scope = ComposerContext.Scope,
                CultureInfo = ComposerContext.CultureInfo,
                CustomerId = ComposerContext.CustomerId
            }).Result;

            viewModel.ChangePasswordUrl = changePasswordUrl;
            viewModel.AddressListUrl = addressListUrl;

            return View("UpdateAccountBlade", viewModel);
        }

        [AuthorizeAndRedirect]
        [OutputCache(Duration = 0, NoStore = true)]
        public virtual ActionResult AddressList()
        {
            var urlParam = new BaseUrlParameter
            {
                CultureInfo = ComposerContext.CultureInfo
            };
            var addAddressUrl = MyAccountUrlProvider.GetAddAddressUrl(urlParam);
            var editAddressBaseUrl = MyAccountUrlProvider.GetUpdateAddressBaseUrl(urlParam);

            var viewModel = CustomerAddressViewService.GetAddressListViewModelAsync(new GetAddressListViewModelParam
            {
                CustomerId = ComposerContext.CustomerId,
                CultureInfo = ComposerContext.CultureInfo,
                Scope = ComposerContext.Scope,
                AddAddressUrl = addAddressUrl,
                EditAddressBaseUrl = editAddressBaseUrl,
                CountryCode = ComposerContext.CountryCode
            }).Result;

            return View("AddressListBlade", viewModel);
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
        public virtual ActionResult RecurringSchedule(XhtmlDocument emptyRecurringScheduleContent)
        {
            var vm = RecurringOrderTemplatesViewService.GetRecurringOrderTemplatesViewModelAsync(new GetRecurringOrderTemplatesParam {
                Scope = ComposerContext.Scope,
                CustomerId = ComposerContext.CustomerId,
                CultureInfo = ComposerContext.CultureInfo,
                BaseUrl = RequestUtils.GetBaseUrl(Request).ToString()
                }).Result;

            if (vm != null && vm.RecurringOrderTemplateViewModelList.Count == 0 && emptyRecurringScheduleContent != null)
            {
                return View("RecurringScheduleContainer", new { TotalQuantity = 0, EmptyContent = emptyRecurringScheduleContent.Body });
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
