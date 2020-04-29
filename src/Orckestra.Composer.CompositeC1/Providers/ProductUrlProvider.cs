using System;
using System.IO;
using System.Web;
using System.Web.Routing;
using Orckestra.Composer.CompositeC1.Services;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Services;
using Orckestra.Composer.Utils;
using Orckestra.ExperienceManagement.Configuration;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;

namespace Orckestra.Composer.CompositeC1.Providers
{
    public class ProductUrlProvider : IProductUrlProvider
    {
        protected IWebsiteContext WebsiteContext { get; private set; }
        protected ISiteConfiguration SiteConfiguration { get; private set; }
        protected IPageService PageService { get; private set; }

        public ProductUrlProvider(IPageService pageService, IWebsiteContext websiteContext, ISiteConfiguration siteConfiguration)
        {
            PageService = pageService;//TODO: ?? throw new ArgumentNullException(nameof(pageService));
            WebsiteContext = websiteContext; //TODO: ?? throw new ArgumentNullException(nameof(websiteContext));
            SiteConfiguration = siteConfiguration;
        }

        public virtual void RegisterRoutes(RouteCollection routeCollection)
        {
            //No route to register in Pure MVC.
        }

        public virtual string GetProductUrl(GetProductUrlParam parameters)
        {
            if (parameters == null) { throw new ArgumentNullException(nameof(parameters)); }
            if (string.IsNullOrWhiteSpace(parameters.ProductId)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(parameters.ProductId)), nameof(parameters)); }
            if (parameters.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(parameters.CultureInfo)), nameof(parameters)); }

            var productName = string.IsNullOrWhiteSpace(parameters.ProductName)
                ? parameters.ProductId
                : parameters.ProductName;
            var formattedProductName = UrlFormatter.FormatProductName(productName);

            var httpContext = !string.IsNullOrEmpty(parameters.BaseUrl) ? new HttpContext(
                new HttpRequest(string.Empty, parameters.BaseUrl, string.Empty),
                new HttpResponse(new StringWriter())
            ) : null;

            var homeUrl = PageService.GetPageUrl(WebsiteContext.WebsiteId, parameters.CultureInfo, httpContext)?.Trim('/');

            string productPath = string.Empty;
            if (!string.IsNullOrWhiteSpace(homeUrl))
            {
                productPath += $"/{homeUrl}";
            }

            productPath += $"/p-{formattedProductName}/{parameters.ProductId}";

            if (!string.IsNullOrWhiteSpace(parameters.VariantId))
            {
                productPath += $"/{parameters.VariantId}";
            }

            var uri = new Uri(productPath, UriKind.Relative);

            return uri.ToString();
        }
    }
}