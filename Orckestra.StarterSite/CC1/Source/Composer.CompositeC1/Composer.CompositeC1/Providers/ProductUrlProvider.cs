using System;
using System.Web.Routing;
using Composite.Core.Routing;
using Composite.Data.Types;
using Orckestra.Composer.CompositeC1.Services;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Services;
using Orckestra.Composer.Utils;
using Orckestra.ExperienceManagement.Configuration;

namespace Orckestra.Composer.CompositeC1.Providers
{
    public class ProductUrlProvider : IProductUrlProvider
    {
        protected IWebsiteContext WebsiteContext { get; private set; }
        protected ISiteConfiguration SiteConfiguration { get; private set; }
        //{
        //    get { return ComposerHost.Current.Resolve<IWebsiteContext>(); }
        //}
        protected IPageService PageService { get; private set; }

        private const string UrlTemplate = "{0}/p-{1}/{2}";
        private const string UrlTemplateVariant = "{0}/p-{1}/{2}/{3}";

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
            Assert(parameters);

            var productName = string.IsNullOrWhiteSpace(parameters.ProductName)
                ? parameters.ProductId
                : parameters.ProductName;
            var formattedProductName = UrlFormatter.FormatProductName(productName);
            string productPath;

            var pagesConfiguration = SiteConfiguration.GetPagesConfiguration(parameters.CultureInfo, WebsiteContext.WebsiteId);
            var homeUrl = PageService.GetPageUrl(pagesConfiguration.HomePageId, parameters.CultureInfo);

            if (string.IsNullOrWhiteSpace(parameters.VariantId))
            {
                productPath = string.Format(UrlTemplate,
                    homeUrl,
                    formattedProductName,
                    parameters.ProductId);
            }
            else
            {
                productPath = string.Format(UrlTemplateVariant,
                    homeUrl,
                    formattedProductName,
                    parameters.ProductId,
                    parameters.VariantId);
            }

            var uri = new Uri(productPath, UriKind.Relative);

            return uri.ToString();
        }

        private void Assert(GetProductUrlParam parameters)
        {
            if (parameters == null) { throw new ArgumentNullException(nameof(parameters)); }
            if (string.IsNullOrWhiteSpace(parameters.ProductId)) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("ProductId"), nameof(parameters)); }
            if (parameters.CultureInfo == null) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("CultureInfo"), nameof(parameters)); }
        }
    }
}
