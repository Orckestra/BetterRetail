using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Web;
using Orckestra.Composer.CompositeC1.Services;
using Orckestra.Composer.Utils;

namespace Orckestra.Composer.CompositeC1
{
    public class
        UrlRewriteModule: IHttpModule
    {
        private static readonly Regex ProductUrlPattern = new Regex(@"^\/(.*?)\/p-.*?\/(.*?)(?:\/(.*))?$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex StoreUrlPattern = new Regex(@"^\/(.*?)\/.*?\/s-(.*)\/(.*)?$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private readonly IPageService _pageService;

        public UrlRewriteModule()
        {
            _pageService = new PageService();
        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += OnBeginRequest;
        }

        private void OnBeginRequest(object sender, EventArgs args)
        {
            var context = ((HttpApplication) sender).Context;
            var rawUrl = context.Request.RawUrl;
            var urlLeftPart = context.Request.Url.GetLeftPart(UriPartial.Path);
            var isProductUrl = urlLeftPart.Contains("/p-");
            var isStoreUrl = urlLeftPart.Contains("/s-");

            if (!isProductUrl && !isStoreUrl)
            {
                return;
            }
            var newUrl = string.Empty;

            if (isProductUrl)
            {
                var match = ProductUrlPattern.Match(rawUrl);
                if (!match.Success || match.Groups.Count < 3)
                {
                    return;
                }

                var cultureName = match.Groups[1].Value;
                var productPageUrl = _pageService.GetPageUrl(PagesConfiguration.ProductPageId,
                    CultureInfo.CreateSpecificCulture(cultureName));

                var productId = match.Groups[2].Value;
                var variantId = match.Groups.Count >= 4 ? match.Groups[3].Value : string.Empty;

                newUrl = UrlFormatter.AppendQueryString(productPageUrl, new NameValueCollection
                {
                    {"id", productId},
                    {"variantId", variantId}
                });
            }

            if (isStoreUrl)
            {
                var match = StoreUrlPattern.Match(rawUrl);
                if (!match.Success)
                {
                    return;
                }

                var cultureName = match.Groups[1].Value;
                var storePageUrl = _pageService.GetPageUrl(PagesConfiguration.StorePageId,
                    CultureInfo.CreateSpecificCulture(cultureName));

                var storeNumber = match.Groups[3].Value;
                newUrl = UrlFormatter.AppendQueryString(storePageUrl, new NameValueCollection
                {
                    {"storeNumber", storeNumber}
                });
            }

            context.RewritePath(newUrl);
        }

        public void Dispose()
        {
            // Do not try to unregister OnBeginRequest here, otherwise it will throw an exception.
        }
    }
}
