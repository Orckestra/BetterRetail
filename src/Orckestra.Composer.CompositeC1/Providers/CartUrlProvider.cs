using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Composite.Core;
using Orckestra.Composer.CompositeC1.Services;
using Orckestra.Composer.Configuration;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Checkout;
using Orckestra.Composer.Services;
using Orckestra.ExperienceManagement.Configuration;
using Orckestra.Overture.Caching;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;

namespace Orckestra.Composer.CompositeC1.Providers
{
    class CartUrlProvider : ICartUrlProvider
    {
        protected IPageService PageService { get; private set; }
        protected ICacheProvider CacheProvider { get; private set; }
        protected IWebsiteContext WebsiteContext { get; private set; }
        protected ISiteConfiguration SiteConfiguration { get; private set; }

        public CartUrlProvider(IPageService pageService, 
            ICacheProvider cacheProvider, 
            IWebsiteContext websiteContext,
            ISiteConfiguration siteConfiguration)
        {
            PageService = pageService ?? throw new ArgumentNullException(nameof(pageService));
            CacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
            WebsiteContext = websiteContext ?? throw new ArgumentNullException(nameof(websiteContext));
            SiteConfiguration = siteConfiguration ?? throw new ArgumentNullException(nameof(siteConfiguration));
        }

        public virtual string GetCartUrl(BaseUrlParameter parameters)
        {
            if (parameters == null) { throw new ArgumentNullException(nameof(parameters)); }
            if (parameters.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(parameters.CultureInfo)), nameof(parameters)); }

            var pagesConfiguration = SiteConfiguration.GetPagesConfiguration(parameters.CultureInfo, WebsiteContext.WebsiteId);
            return PageService.GetPageUrl(pagesConfiguration.CartPageId, parameters.CultureInfo);
        }

        public virtual string GetCheckoutConfirmationPageUrl(BaseUrlParameter parameters)
        {
            if (parameters == null) { throw new ArgumentNullException(nameof(parameters)); }
            if (parameters.CultureInfo == null) { throw new ArgumentException($"{nameof(parameters.CultureInfo)} is required", nameof(parameters)); }

            var pagesConfiguration = SiteConfiguration.GetPagesConfiguration(parameters.CultureInfo, WebsiteContext.WebsiteId);
            return PageService.GetPageUrl(pagesConfiguration.CheckoutConfirmationPageId, parameters.CultureInfo);
        }

        public virtual string GetCheckoutSignInUrl(BaseUrlParameter parameters)
        {
            if (parameters == null) { throw new ArgumentNullException(nameof(parameters)); }
            if (parameters.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(parameters.CultureInfo)), nameof(parameters)); }

            var pagesConfiguration = SiteConfiguration.GetPagesConfiguration(parameters.CultureInfo, WebsiteContext.WebsiteId);
            var signInPath = PageService.GetPageUrl(pagesConfiguration.CheckoutSignInPageId, parameters.CultureInfo);

            if (string.IsNullOrWhiteSpace(parameters.ReturnUrl)) { return signInPath; }

            var urlBuilder = new UrlBuilder(signInPath);
            urlBuilder["ReturnUrl"] = GetReturnUrl(parameters); // url builder will encode the query string value

            return urlBuilder.ToString();
        }

        public virtual string GetCheckoutPageUrl(BaseUrlParameter parameters)
        {
            if (parameters == null) { throw new ArgumentNullException(nameof(parameters)); }
            if (parameters.CultureInfo == null) { throw new ArgumentException($"{nameof(parameters.CultureInfo)} is required", nameof(parameters)); }

            var pagesConfiguration = SiteConfiguration.GetPagesConfiguration(parameters.CultureInfo, WebsiteContext.WebsiteId);
            return PageService.GetPageUrl(pagesConfiguration.CheckoutPageId, parameters.CultureInfo);
        }

        private static string GetReturnUrl(BaseUrlParameter parameters)
        {
            var returnUrl = Uri.IsWellFormedUriString(parameters.ReturnUrl, UriKind.Relative)
                ? new Uri(parameters.ReturnUrl, UriKind.Relative)
                : new Uri(parameters.ReturnUrl);
            
            return returnUrl.ToString();
        }

         public string GetHomepageUrl(BaseUrlParameter param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }

            var url = PageService.GetPageUrl(WebsiteContext.WebsiteId, param.CultureInfo);
            ///TODO - fix this
            if (string.IsNullOrWhiteSpace(url)) return url;

            return UrlProviderHelper.BuildUrlWithParams(url, param.ReturnUrl);
        }

        public string GetForgotPasswordPageUrl(BaseUrlParameter param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException($"{nameof(param.CultureInfo)} is required", nameof(param)); }

            var pagesConfiguration = SiteConfiguration.GetPagesConfiguration(param.CultureInfo, WebsiteContext.WebsiteId);
            return PageService.GetPageUrl(pagesConfiguration.ForgotPasswordPageId, param.CultureInfo);
        }
    }
}