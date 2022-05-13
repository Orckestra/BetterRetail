﻿using System;
using System.Globalization;
using System.Net;
using System.Web.Mvc;
using Composite.Data;
using Orckestra.Composer.Services;
using Orckestra.Composer.Store.Services;
using Orckestra.Composer.Store.Providers;
using Orckestra.Composer.Services.Breadcrumb;
using Orckestra.Composer.Store.Parameters;
using Orckestra.Composer.Utils;
using Orckestra.Composer.Store;
using Orckestra.Composer.Store.ViewModels;
using Orckestra.Composer.CompositeC1.Context;

namespace Orckestra.Composer.CompositeC1.Controllers
{
    public abstract class StoreLocatorBaseController :Controller
    {
        protected IComposerContext ComposerContext { get; private set; }
        protected IStoreContext StoreContext { get; private set; }
        protected IStoreViewService StoreViewService { get; set; }
        protected IStoreDirectoryViewService StoreDirectoryViewService { get; set; }
        protected IStoreUrlProvider StoreUrlProvider { get; private set; }
        protected IBreadcrumbViewService BreadcrumbViewService { get; private set; }
        protected ILanguageSwitchService LanguageSwitchService { get; private set; }

        protected StoreLocatorBaseController(
           IComposerContext composerContext,
           IStoreViewService storeViewService,
           IStoreDirectoryViewService storeDirectoryViewService,
           IStoreUrlProvider storeUrlProvider,
           IBreadcrumbViewService breadcrumbViewService,
           ILanguageSwitchService languageSwitchService,
           IStoreContext storeContext)
        {
            ComposerContext = composerContext ?? throw new ArgumentNullException(nameof(composerContext));
            StoreViewService = storeViewService ?? throw new ArgumentNullException(nameof(storeViewService));
            StoreDirectoryViewService = storeDirectoryViewService ?? throw new ArgumentNullException(nameof(storeDirectoryViewService));
            StoreUrlProvider = storeUrlProvider ?? throw new ArgumentNullException(nameof(storeUrlProvider));
            BreadcrumbViewService = breadcrumbViewService ?? throw new ArgumentNullException(nameof(breadcrumbViewService));
            LanguageSwitchService = languageSwitchService ?? throw new ArgumentNullException(nameof(languageSwitchService));
            StoreContext = storeContext ?? throw new ArgumentNullException(nameof(storeContext));
        }

        public virtual ActionResult StoreDirectory(int page = 1)
        {
            var baseUrl = RequestUtils.GetBaseUrl(Request).ToString();

            var model = StoreDirectoryViewService.GetStoreDirectoryViewModelAsync(new GetStoresParam
            {
                Scope = ComposerContext.Scope,
                CultureInfo = ComposerContext.CultureInfo,
                BaseUrl = baseUrl,
                PageNumber = page,
                PageSize = StoreConfiguration.DirectoryListMaxItemsPerPage,
                WebsiteId = SitemapNavigator.CurrentHomePageId
            }).Result;

            if (model == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            return View("StoreDirectory", model);
        }

        public virtual ActionResult StoreDetails(string storeNumber, int zoom = 14)
        {
            var model = StoreContext.ViewModel;

            if (model == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            model.Context.Add("zoom", zoom);

            return View("StoreDetails", model);
        }

        public virtual ActionResult StoreInventory(string id, string variantId, int pagesize = 4)
        {
            if (string.IsNullOrEmpty(id))
            {
                return View();
            }

            if (variantId != null && variantId.Contains("?"))
            {
                variantId = variantId.Substring(0, variantId.IndexOf('?'));
            }

            var vm = new StoreInventoryViewModel();
            vm.Context.Add("productId", id);
            vm.Context.Add("selectedSku", variantId);
            vm.Context.Add("pageSize", pagesize);
            if (ComposerContext.IsAuthenticated)
            {
                vm.Context.Add("isAuthenticated", true);
            }

            return View(vm);
        }

        public virtual ActionResult StoreLocatorInHeader()
        { 
            var model = new StoreLocatorInHeaderViewModel
            {
                Url = StoreUrlProvider.GetStoreLocatorUrl(new GetStoreLocatorUrlParam
                {
                    BaseUrl = RequestUtils.GetBaseUrl(Request).ToString(),
                    CultureInfo = ComposerContext.CultureInfo
                })
            };
            return View("StoreLocatorInHeader", model);
        }

        public virtual ActionResult PageHeader(string storeNumber)
        {
            var model = StoreContext.ViewModel;
            if (model == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            var vm = StoreViewService.GetPageHeaderViewModel(model, new GetStorePageHeaderViewModelParam
            {
                Scope = ComposerContext.Scope,
                CultureInfo = ComposerContext.CultureInfo,
                StoreNumber = model.Number,
                BaseUrl = RequestUtils.GetBaseUrl(Request).ToString()
            });

            return View(vm);
        }

        public virtual ActionResult LanguageSwitch(string storeNumber)
        {
            var model = StoreContext.ViewModel;
            var baseUrl = RequestUtils.GetBaseUrl(Request).ToString();

            if (model == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            var languageSwitchViewModel = LanguageSwitchService.GetViewModel(cultureInfo => BuildUrl(
                baseUrl,
                cultureInfo,
                model.LocalizedDisplayNames[cultureInfo.Name],
                model.Number),
                ComposerContext.CultureInfo);

            return View("LanguageSwitch", languageSwitchViewModel);
        }

        protected virtual string BuildUrl(string baseUrl, CultureInfo cultureInfo, string storeName, string storeNumber)
        {
            var storeUrl = StoreUrlProvider.GetStoreUrl(new GetStoreUrlParam
            {
                StoreNumber = storeNumber,
                CultureInfo = cultureInfo,
                BaseUrl = baseUrl,
                StoreName = storeName,
                WebsiteId = SitemapNavigator.CurrentHomePageId
            });

            return storeUrl;
        }
    }
}
