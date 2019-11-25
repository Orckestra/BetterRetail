using System;
using System.Web.Routing;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Utils;

namespace Orckestra.Composer.Product.Providers
{
    public class ProductUrlProvider : IProductUrlProvider
    {
        private const string UrlTemplate = "/{0}/p-{1}/{2}";
        private const string UrlTemplateVariant = "/{0}/p-{1}/{2}/{3}";

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

            if (string.IsNullOrWhiteSpace(parameters.VariantId))
            {
                productPath = string.Format(UrlTemplate,
                parameters.CultureInfo.Name,
                formattedProductName,
                parameters.ProductId);
            }
            else
            {
                productPath = string.Format(UrlTemplateVariant,
                parameters.CultureInfo.Name,
                formattedProductName,
                parameters.ProductId,
                parameters.VariantId);
            }

            var uri = new Uri(productPath, UriKind.Relative);

            return uri.ToString();
        }

        private void Assert(GetProductUrlParam parameters)
        {
            if (parameters == null) { throw new ArgumentNullException("parameters"); }            
            if (string.IsNullOrWhiteSpace(parameters.ProductId)) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("ProductId"), "parameters"); }
            if (parameters.CultureInfo == null) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("CultureInfo"), "parameters"); }
        }
    }
}
