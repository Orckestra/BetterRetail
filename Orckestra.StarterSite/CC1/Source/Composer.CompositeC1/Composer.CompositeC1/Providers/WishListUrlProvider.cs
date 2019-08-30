using Composite.Core;
using Composite.Core.Threading;
using Orckestra.Composer.Cart.Parameters.WishList;
using Orckestra.Composer.Cart.Providers.WishList;
using Orckestra.Composer.Cart.Utils;
using Orckestra.Composer.CompositeC1.Services;
using Orckestra.ExperienceManagement.Configuration;
using System;

namespace Orckestra.Composer.Mvc.Sample.Providers.UrlProvider
{
    public class WishListUrlProvider : IWishListUrlProvider
    {
        protected IPageService PageService { get; private set; }
        protected PagesConfiguration PagesConfiguration { get; private set; }

        public WishListUrlProvider(IPageService pageService)
        {
            if (pageService == null) { throw new ArgumentNullException("pageService"); }

            PageService = pageService;
            PagesConfiguration = SiteConfiguration.GetPagesConfiguration();
        }

        /// <summary>
        /// Get the Url of the WishList page.
        /// </summary>
        public virtual string GetWishListUrl(GetWishListUrlParam parameters)
        {
            using (ThreadDataManager.EnsureInitialize())
            {
                if (parameters == null)
                {
                    throw new ArgumentNullException("parameters");
                }
                if (parameters.CultureInfo == null)
                {
                    throw new ArgumentException("parameters.CultureInfo is required", "parameters");
                }

                return PageService.GetPageUrl(PagesConfiguration.MyWishListPageId, parameters.CultureInfo);
            }
        }

        /// <summary>
        /// Get the Url of the AddToWishList SignIn page.
        /// </summary>
        public virtual string GetSignInUrl(GetWishListUrlParam parameters)
        {
            using (ThreadDataManager.EnsureInitialize())
            {
                if (parameters == null)
                {
                    throw new ArgumentNullException("parameters");
                }
                if (parameters.CultureInfo == null)
                {
                    throw new ArgumentException("parameters.CultureInfo is required", "parameters");
                }

                var signInPath = PageService.GetPageUrl(PagesConfiguration.LoginPageId, parameters.CultureInfo);

                if (string.IsNullOrWhiteSpace(parameters.ReturnUrl))
                {
                    return signInPath;
                }

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

                var shareWishListPageUrl = PageService.GetPageUrl(PagesConfiguration.SharedWishListPageId,
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