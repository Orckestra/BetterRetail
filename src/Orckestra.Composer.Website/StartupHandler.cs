using Autofac.Integration.Mvc;
using Composite.AspNet.MvcFunctions;
using Composite.Core.Application;
using Composite.Core.Xml;
using Composite.Data;
using Composite.Data.DynamicTypes;
using Composite.Data.Types;
using Composite.Functions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using Orckestra.Composer.CompositeC1.DataTypes;
using Orckestra.Composer.CompositeC1.Pages;
using Orckestra.Composer.HttpModules;
using Orckestra.Composer.Logging;
using Orckestra.Composer.Repositories;
using Orckestra.Composer.Search;
using Orckestra.Composer.Website.Controllers;
using Orckestra.ExperienceManagement.Configuration.DataTypes;
using System;
using System.Linq;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace Orckestra.Composer.Website
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

            Guid homepageId = GetHomePageId(data);
            using (var con = new DataConnection())
            {
                var meta = con.Get<ISiteConfigurationMeta>().FirstOrDefault(item => item.PageId == homepageId);
                if (meta == null) return;

                var categoryRepository = Composite.Core.ServiceLocator.GetService<ICategoryRepository>();
                categoryRepository.ClearCategoriesCache(meta.Scope);
            }
        }

        private static Guid GetHomePageId(IPage data)
        {
            Guid homepageId = Guid.Empty;
            Guid pageId = data.Id;

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

            functions.RegisterAction<CartController>("Minicart", "Composer.Cart.Minicart").AddParameter(
                "notificationTimeInSeconds",
                typeof(int),
                isRequired: true,
                defaultValueProvider: new ConstantValueProvider(5),
                label: "Minicart notification time",
                helpText: "Notification time of the minicart when an item is added/updated in cart (in seconds)."
            );

            functions.RegisterAction<CheckoutController>("CheckoutSignInAsGuest", "Composer.Checkout.CheckoutSignInAsGuest");
            functions.RegisterAction<CheckoutController>("CheckoutSignInAsCustomer", "Composer.Checkout.CheckoutSignInAsCustomer");
  
            functions.RegisterAction<MembershipController>("ReturningCustomerBlade", "Composer.Membership.ReturningCustomer");
            functions.RegisterAction<MembershipController>("NewCustomerBlade", "Composer.Membership.NewCustomer");
            functions.RegisterAction<MembershipController>("CreateAccountBlade", "Composer.Membership.CreateAccount");
            functions.RegisterAction<MembershipController>("ForgotPasswordBlade", "Composer.Membership.ForgotPassword");
            functions.RegisterAction<MembershipController>("NewPasswordBlade", "Composer.Membership.NewPassword");
            functions.RegisterAction<MembershipController>("ChangePasswordBlade", "Composer.Membership.ChangePassword");

            functions.RegisterAction<MyAccountController>("AccountHeader", "Composer.MyAccount.AccountHeader");
            functions.RegisterAction<MyAccountController>("CreateAddress", "Composer.MyAccount.CreateAddress");
            functions.RegisterAction<MyAccountController>("EditAddress", "Composer.MyAccount.UpdateAddress").IncludePathInfo();
            functions.RegisterAction<MyAccountController>("RecurringSchedule", "Composer.MyAccount.RecurringSchedule");
            functions.RegisterAction<MyAccountController>("RecurringScheduleDetails", "Composer.MyAccount.RecurringScheduleDetails");
            functions.RegisterAction<MyAccountController>("UpcomingOrders", "Composer.MyAccount.UpcomingOrders");
            functions.RegisterAction<MyAccountController>("RecurringCartDetails", "Composer.MyAccount.RecurringCartDetails");
            functions.RegisterAction<WishListController>("WishListInHeader", "Composer.WishList.WishListInHeader");
            functions.RegisterAction<OrderController>("FindMyOrder", "Composer.Order.FindMyOrder");

            functions.RegisterAction<StoreLocatorController>("StoreDetails", "Composer.Store.Details", "Store Details")
                .AddParameter("zoom", typeof(int), false, label: "Map Zoom Level", helpText: "Define the resolution of the map view. Zoom levels between 0 and 21+. Default is 14 (streets).");
            functions.RegisterAction<StoreLocatorController>("PageHeader", "Composer.Store.PageHeader");
            functions.RegisterAction<StoreLocatorController>("StoreDirectory", "Composer.Store.Directory");
            functions.RegisterAction<StoreLocatorController>("StoreLocatorInHeader", "Composer.Store.LocatorInHeader");
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