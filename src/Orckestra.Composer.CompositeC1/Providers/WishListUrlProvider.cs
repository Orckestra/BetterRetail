using System;
using Composite.Core;
using Composite.Core.Threading;
using Orckestra.Composer.Cart.Parameters.WishList;
using Orckestra.Composer.Cart.Providers.WishList;
using Orckestra.Composer.Cart.Utils;
using Orckestra.Composer.CompositeC1.Services;
using Orckestra.Composer.Services;
using Orckestra.ExperienceManagement.Configuration;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;

namespace Orckestra.Composer.Mvc.Sample.Providers.UrlProvider
{
    public class WishListUrlProvider : IWishListUrlProvider
    {
        protected IPageService PageService { get; private set; }
        protected IWebsiteContext WebsiteContext { get; private set; }
        protected ISiteConfiguration SiteConfiguration { get; private set; }

        public WishListUrlProvider(IPageService pageService, IWebsiteContext websiteContext, ISiteConfiguration siteConfiguration)
        {
            PageService = pageService ?? throw new ArgumentNullException(nameof(pageService));
            WebsiteContext = websiteContext;
            SiteConfiguration = siteConfiguration;
        }

        /// <summary>
        /// Get the Url of the WishList page.
        /// </summary>
        public virtual string GetWishListUrl(GetWishListUrlParam parameters)
        {
            if (parameters == null) { throw new ArgumentNullException(nameof(parameters)); }
            if (parameters.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(parameters.CultureInfo)), nameof(parameters)); }

            using (ThreadDataManager.EnsureInitialize())
            {
                var pagesConfiguration = SiteConfiguration.GetPagesConfiguration(parameters.CultureInfo, WebsiteContext.WebsiteId);
                if (pagesConfiguration == null) return null;

                return PageService.GetPageUrl(pagesConfiguration.MyWishListPageId, parameters.CultureInfo);
            }
        }

        /// <summary>
        /// Get the Url of the AddToWishList SignIn page.
        /// </summary>
        public virtual string GetSignInUrl(GetWishListUrlParam parameters)
        {
            if (parameters == null) { throw new ArgumentNullException(nameof(parameters)); }
            if (parameters.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(parameters.CultureInfo)), nameof(parameters)); }

            using (ThreadDataManager.EnsureInitialize())
            {
                var pagesConfiguration = SiteConfiguration.GetPagesConfiguration(parameters.CultureInfo, WebsiteContext.WebsiteId);
                var signInPath = PageService.GetPageUrl(pagesConfiguration.LoginPageId, parameters.CultureInfo);

                if (string.IsNullOrWhiteSpace(parameters.ReturnUrl)) { return signInPath; }

                var urlBuilder = new UrlBuilder(signInPath);
                urlBuilder["ReturnUrl"] = GetReturnUrl(parameters); // url builder will encode the query string value

                return urlBuilder.ToString();
            }
        }

        public virtual string GetShareUrl(GetShareWishListUrlParam parameters)
        {
            using (ThreadDataManager.EnsureInitialize())
            {
                var token = SharedWishListTokenizer.GenerateToken(new SharedWishListToken
                {
                    CustomerId = parameters.CustomerId,
                    Scope = parameters.Scope
                });

                var pagesConfiguration = SiteConfiguration.GetPagesConfiguration(parameters.CultureInfo, WebsiteContext.WebsiteId);
                var shareWishListPageUrl = PageService.GetPageUrl(pagesConfiguration.SharedWishListPageId,
                    parameters.CultureInfo);
                var url = $"{shareWishListPageUrl}?id={token}";
                var uri = new Uri(
                    new Uri(parameters.BaseUrl, UriKind.Absolute),
                    new Uri(url, UriKind.Relative));

                return uri.ToString();
            }
        }

        private static string GetReturnUrl(GetWishListUrlParam parameters)
        {
            var returnUrl = Uri.IsWellFormedUriString(parameters.ReturnUrl, UriKind.Relative)
                ? new Uri(new Uri(parameters.BaseUrl), parameters.ReturnUrl)
                : new Uri(parameters.ReturnUrl);

            return returnUrl.ToString();
        }
    }
}