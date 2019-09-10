using System;
using System.Collections.Generic;
using System.Linq;
using Composite.Core;
using Composite.Data;
using Orckestra.Composer.CompositeC1.Services;
using Orckestra.Composer.Configuration;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Checkout;
using Orckestra.Composer.Services;
using Orckestra.ExperienceManagement.Configuration;
using Orckestra.Overture.Caching;

namespace Orckestra.Composer.CompositeC1.Providers
{
    class CartUrlProvider : ICartUrlProvider
    {
        protected IPageService PageService { get; private set; }
        protected ICacheProvider CacheProvider { get; private set; }
        protected IWebsiteContext WebsiteContext { get; private set; }

        public CartUrlProvider(IPageService pageService, ICacheProvider cacheProvider, IWebsiteContext websiteContext)
        {
            if (pageService == null) { throw new ArgumentNullException("pageService"); }
            if (cacheProvider == null) { throw new ArgumentNullException("cacheProvider"); }

            PageService = pageService;
            CacheProvider = cacheProvider;
            WebsiteContext = websiteContext;
          
        }

        public virtual string GetCartUrl(BaseUrlParameter parameters)
        {
            if (parameters == null) { throw new ArgumentNullException("parameters"); }
            if (parameters.CultureInfo == null) { throw new ArgumentException("parameters.CultureInfo is required", "parameters"); }


            //return PageService.GetPageUrl(PagesConfiguration.CartPageId, parameters.CultureInfo);
            var pagesConfiguration = SiteConfiguration.GetPagesConfiguration(parameters.CultureInfo, WebsiteContext.WebsiteId);
            return PageService.GetPageUrl(pagesConfiguration.CartPageId, parameters.CultureInfo);
        }

        public virtual string GetCheckoutSignInUrl(BaseUrlParameter parameters)
        {
            if (parameters == null) { throw new ArgumentNullException("parameters"); }
            if (parameters.CultureInfo == null) { throw new ArgumentException("parameters.CultureInfo is required", "parameters"); }

            var pagesConfiguration = SiteConfiguration.GetPagesConfiguration(parameters.CultureInfo, parameters.WebsiteId);
            var signInPath = PageService.GetPageUrl(pagesConfiguration.CheckoutSignInPageId, parameters.CultureInfo);

            if (string.IsNullOrWhiteSpace(parameters.ReturnUrl))
            {
                return signInPath;
            }

            var urlBuilder = new UrlBuilder(signInPath);
            urlBuilder["ReturnUrl"] = GetReturnUrl(parameters); // url builder will encode the query string value

            return urlBuilder.ToString();
        }

        private static string GetReturnUrl(BaseUrlParameter parameters)
        {
            var returnUrl = Uri.IsWellFormedUriString(parameters.ReturnUrl, UriKind.Relative)
                ? new Uri(parameters.ReturnUrl, UriKind.Relative)
                : new Uri(parameters.ReturnUrl);
            
            return returnUrl.ToString();
        }

        public virtual string GetCheckoutStepUrl(GetCheckoutStepUrlParam parameters)
        {
            var stepUrls = GetCheckoutStepPageInfos(new BaseUrlParameter
            {                
                CultureInfo = parameters.CultureInfo,
                WebsiteId = parameters.WebsiteId
            });

            if (!stepUrls.ContainsKey(parameters.StepNumber))
            {
                throw new ArgumentOutOfRangeException("parameters", "StepNumber is invalid");
            }

            return stepUrls[parameters.StepNumber].Url;
        }

        public virtual Dictionary<int, CheckoutStepPageInfo> GetCheckoutStepPageInfos(BaseUrlParameter parameters)
        {
            CacheKey cacheKey = new CacheKey(CacheConfigurationCategoryNames.CheckoutStepUrls)
            {
                CultureInfo = parameters.CultureInfo
            };
            Dictionary<int, CheckoutStepPageInfo> stepUrls = CacheProvider.Get< Dictionary<int, CheckoutStepPageInfo>>(cacheKey);

            if (stepUrls != null)
                return stepUrls;

            stepUrls = new Dictionary<int, CheckoutStepPageInfo>();

            var items = PageService.GetCheckoutStepPages(parameters.WebsiteId, parameters.CultureInfo);
            var navItems = PageService.GetCheckoutNavigationPages(parameters.WebsiteId, parameters.CultureInfo);
            var index = 0;
            foreach (var checkoutStepItem in items)
            {
                var pageGuid = Guid.Parse(checkoutStepItem);
                stepUrls.Add(index, new CheckoutStepPageInfo
                {
                    Url = PageService.GetPageUrl(pageGuid, parameters.CultureInfo),
                    IsDisplayedInHeader = navItems != null && navItems.Contains(checkoutStepItem),
                    Title = PageService.GetPage(pageGuid, parameters.CultureInfo).MenuTitle,
                    PageId = pageGuid
                });
                index++;
            }

            stepUrls = stepUrls.OrderBy(x => x.Key).ToDictionary(x => x.Key, y => y.Value);

            CacheProvider.Set(cacheKey, stepUrls);

            return stepUrls;
        }

        public virtual string GetCheckoutAddAddressUrl(BaseUrlParameter param)
        {
            if (param == null) { throw new ArgumentNullException("param"); }
            if (param.WebsiteId == Guid.Empty) { throw new ArgumentNullException("param.WebsiteId"); }

            var pagesConfiguration = SiteConfiguration.GetPagesConfiguration(param.CultureInfo, param.WebsiteId);
            var url = PageService.GetPageUrl(pagesConfiguration.CheckoutAddAddressPageId, param.CultureInfo);
            return UrlProviderHelper.BuildUrlWithParams(url, param.ReturnUrl);
        }

        public virtual string GetCheckoutUpdateAddressBaseUrl(BaseUrlParameter param)
        {
            if (param == null) { throw new ArgumentNullException("param"); }
            if (param.WebsiteId == Guid.Empty) { throw new ArgumentNullException("param.WebsiteId"); }

            var pagesConfiguration = SiteConfiguration.GetPagesConfiguration(param.CultureInfo, param.WebsiteId);
            var url = PageService.GetPageUrl(pagesConfiguration.CheckoutUpdateAddressPageId, param.CultureInfo);
            return UrlProviderHelper.BuildUrlWithParams(url, param.ReturnUrl);
        }

        public string GetHomepageUrl(BaseUrlParameter param)
        {
            if (param == null) { throw new ArgumentNullException("param"); }
            if (param.WebsiteId == Guid.Empty) {
                throw new ArgumentNullException("param.WebsiteId");
            }
         
            var url = PageService.GetPageUrl(param.WebsiteId, param.CultureInfo);
            ///TODO - fix this
            if (string.IsNullOrWhiteSpace(url))
                return url;

            return UrlProviderHelper.BuildUrlWithParams(url, param.ReturnUrl);
        }
    }
}
