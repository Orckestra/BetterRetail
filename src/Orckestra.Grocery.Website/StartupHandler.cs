using Autofac.Integration.Mvc;
using Composite.AspNet.MvcFunctions;
using Composite.Core.Application;
using Composite.Core.Xml;
using Composite.Data;
using Composite.Data.DynamicTypes;
using Composite.Functions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using Orckestra.Composer.CompositeC1.DataTypes;
using Orckestra.Composer.CompositeC1.Pages;
using Orckestra.Composer.Grocery.Website.Controllers;
using Orckestra.Composer.HttpModules;
using Orckestra.Composer.Logging;
using Orckestra.Composer.Search;
using Orckestra.Composer.Configuration;
using Orckestra.ExperienceManagement.Configuration.DataTypes;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Composite.Data.Types;
using System;
using Orckestra.Composer.Repositories;
using System.Linq;
using Orckestra.Composer.Grocery.DataTypes;
using Orckestra.Composer.Cart.Repositories.Order;
using Orckestra.Composer.Services;
using Orckestra.Overture.Caching;

namespace Orckestra.Composer.Grocery.Website
{
    [ApplicationStartup(AbortStartupOnException = true)]
    public static class StartupHandler
    {
        private static IComposerHost _host;
        public static void Start()
        {
            if (!HostingEnvironment.IsHosted) return;

            DynamicModuleUtility.RegisterModule(typeof(SecurityModule));
            SetUpSearchConfiguration();
            SetUpQuantityConfiguration();
        }

        public static void OnBeforeInitialize()
        {
         
        }

        public static void OnInitialized()
        {
            if (!HostingEnvironment.IsHosted) return;

            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            LogProvider.SetCurrentLogProvider(C1LogProvider.Instance);

            // Needed by C1, do not remove!
          
            _host.Init();

            var log = LogProvider.GetCurrentClassLogger();

            log.Info("Application Starting");

            DynamicTypeManager.EnsureCreateStore(typeof(ComposerPage));
            DynamicTypeManager.EnsureCreateStore(typeof(CategoryPage));

            var functions = MvcFunctionRegistry.NewFunctionCollection();
            RegisterFunctions(functions);
            RegisterFunctionRoutes(functions);

            DataEvents<IPage>.OnAfterAdd += UpdateAfterPageChanged;

            log.Info("Application Started");
        }

        /// <summary>
        /// Do some updates when C1 page is added, for example clear Categories Cache
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="dataEventArgs"></param>
        private static void UpdateAfterPageChanged(object sender, DataEventArgs dataEventArgs)
        {
            var page = dataEventArgs.Data as IPage;
            if (page == null) return;

            ClearCategoriesCache(page);
        }

        private static void ClearCategoriesCache(IPage data)
        {
            if (data.PageTypeId != CategoryPages.CategoryPageTypeId) return;

            Guid homepageId = GetHomePageId(data.Id);
            using (var con = new DataConnection())
            {
                var siteConfigurationMeta = con.Get<ISiteConfigurationMeta>().FirstOrDefault(item => item.PageId == homepageId);
                var grocerySettingsMeta = con.Get<IGrocerySettingsMeta>().FirstOrDefault(item => item.PageId == homepageId);

                var categoryRepository = Composite.Core.ServiceLocator.GetService<ICategoryRepository>();

                if (siteConfigurationMeta != null)
                {
                    categoryRepository.ClearCategoriesCache(siteConfigurationMeta.Scope);
                }

                if (grocerySettingsMeta != null)
                {
                    categoryRepository.ClearCategoriesCache(grocerySettingsMeta.DefaultStore);
                }
            }
        }

        private static Guid GetHomePageId(Guid id)
        {
            Guid homepageId = Guid.Empty;
            Guid pageId = id;

            while (pageId != Guid.Empty)
            {
                homepageId = pageId;
                pageId = PageManager.GetParentId(pageId);
            }

            return homepageId;
        }

        private static void RegisterFunctions(FunctionCollection functions)
        {
            functions.AutoDiscoverFunctions(typeof(StartupHandler).Assembly);

            functions.RegisterAction<HeaderController>("GeneralErrors", "Composer.Header.GeneralErrors");
            functions.RegisterAction<HeaderController>("PageHeader", "Composer.Header.PageHeader");

            functions.RegisterAction<SearchController>("PageHeader", "Composer.Search.PageHeader");
            functions.RegisterAction<BrowsingCategoriesController>("ChildCategories", "Composer.BrowsingCategories.ChildCategories");
  
            functions.RegisterAction<CheckoutController>("CheckoutSignInAsGuest", "Composer.Checkout.CheckoutSignInAsGuest");
            functions.RegisterAction<CheckoutController>("CheckoutSignInAsCustomer", "Composer.Checkout.CheckoutSignInAsCustomer");

            functions.RegisterAction<MembershipController>("ReturningCustomerBlade", "Composer.Membership.ReturningCustomer");
            functions.RegisterAction<MembershipController>("NewCustomerBlade", "Composer.Membership.NewCustomer");
            functions.RegisterAction<MembershipController>("CreateAccountBlade", "Composer.Membership.CreateAccount");
            functions.RegisterAction<MembershipController>("ForgotPasswordBlade", "Composer.Membership.ForgotPassword");
            functions.RegisterAction<MembershipController>("NewPasswordBlade", "Composer.Membership.NewPassword");
            functions.RegisterAction<MembershipController>("ChangePasswordBlade", "Composer.Membership.ChangePassword");

            functions.RegisterAction<MyAccountController>("AccountHeader", "Composer.MyAccount.AccountHeader");
            functions.RegisterAction<MyAccountController>("UpdateAccount", "Composer.MyAccount.UpdateAccount");
            functions.RegisterAction<MyAccountController>("CreateAddress", "Composer.MyAccount.CreateAddress");
            functions.RegisterAction<MyAccountController>("EditAddress", "Composer.MyAccount.UpdateAddress").IncludePathInfo();
            functions.RegisterAction<MyAccountController>("RecurringSchedule", "Composer.MyAccount.RecurringSchedule");
            functions.RegisterAction<MyAccountController>("RecurringScheduleDetails", "Composer.MyAccount.RecurringScheduleDetails");
            functions.RegisterAction<MyAccountController>("UpcomingOrders", "Composer.MyAccount.UpcomingOrders");
            functions.RegisterAction<MyAccountController>("RecurringCartDetails", "Composer.MyAccount.RecurringCartDetails");

            functions.RegisterAction<WishListController>("WishListInHeader", "Composer.WishList.WishListInHeader");
            //also rewrite
            functions.RegisterAction<WishListController>("SharedWishList", "Composer.WishList.Shared")
                .AddParameter("emptyWishListContent", typeof(XhtmlDocument), true, label: "Empty Wish List Content", helpText: "That content will be shown when Wish List is Empty"); ;
            functions.RegisterAction<WishListController>("SharedWishListTitle", "Composer.WishList.SharedTitle");

            functions.RegisterAction<OrderController>("FindMyOrder", "Composer.Order.FindMyOrder");

            functions.RegisterAction<StoreLocatorController>("PageHeader", "Composer.Store.PageHeader");
            functions.RegisterAction<StoreLocatorController>("StoreDirectory", "Composer.Store.Directory");
            functions.RegisterAction<StoreLocatorController>("StoreInventory", "Composer.Store.Inventory")
                .AddParameter("pagesize", typeof(int), false, label: "Page Size", helpText: "The max count of the items to show in the list.");
            functions.RegisterAction<PageNotFoundController>("PageNotFoundAnalytics", "Composer.PageNotFound.Analytics");
        }

        private static void RegisterFunctionRoutes(FunctionCollection functions)
        {
            functions.RouteCollection.MapMvcAttributeRoutes();
            functions.RouteCollection.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { action = "Index", id = UrlParameter.Optional } // Parameter defaults
                );
        }

        private static void SetUpSearchConfiguration()
        {
            SearchConfiguration.ShowAllPages = true;
            SearchConfiguration.MaxItemsPerPage = 20;
        }

        private static void SetUpQuantityConfiguration()
        {
            QuantityConfiguration.MinQuantity = 0;
        }

        public static void ConfigureServices(IServiceCollection collection)
        {
            if (!HostingEnvironment.IsHosted) return;

            _host = new ComposerHost();
            _host.LoadPlugins();
            foreach(var type in _host.RegisteredInterfaces)
            {
                collection.AddTransient(type, provider => AutofacDependencyResolver.Current.GetService(type));
            }
        }

    }
}