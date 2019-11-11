using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Utils;
using Orckestra.Overture;
using Orckestra.Overture.ServiceModel.Requests.Search;
using Orckestra.Overture.ServiceModel.Search;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orckestra.Overture.ServiceModel;
using Orckestra.Composer.Logging;
using Orckestra.Composer.Sitemap.Factory;
using Orckestra.Composer.Sitemap.Models;

namespace Orckestra.Composer.Sitemap.Product
{
    public class ProductSitemapEntryProvider : ISitemapEntryProvider
    {
        private static ILog Log = LogProvider.GetCurrentClassLogger();

        private const string ProductId = "ProductId";
        private const string DisplayName = "DisplayName";        

        private static string[] RequiredDocumentKeys = new[] { DisplayName };

        private readonly IOvertureClient _overtureClient;
        private readonly IProductUrlProvider _productUrlProvider;
        private readonly IProductUrlParamFactory _productUrlParamFactory;

        public ProductSitemapEntryProvider(IOvertureClient overtureClient, IProductUrlProvider productUrlProvider, IProductUrlParamFactory productUrlParamFactory)
        {
            Guard.NotNull(overtureClient, nameof(overtureClient));
            Guard.NotNull(productUrlProvider, nameof(productUrlProvider));

            _overtureClient = overtureClient;
            _productUrlProvider = productUrlProvider;
            _productUrlParamFactory = productUrlParamFactory;
        }

        public virtual async Task<IEnumerable<SitemapEntry>> GetEntriesAsync(SitemapParams sitemapParams, CultureInfo culture, int offset, int count)
        {
            Guard.NotNull(sitemapParams, nameof(sitemapParams));
            Guard.NotNullOrWhiteSpace(sitemapParams.BaseUrl, $"{nameof(sitemapParams)}.{nameof(sitemapParams.BaseUrl)}");
            Guard.NotNullOrWhiteSpace(sitemapParams.Scope, $"{nameof(sitemapParams)}.{nameof(sitemapParams.Scope)}");
            Guard.NotNull(culture, nameof(culture));            

            var request = new SearchProductRequest
            {
                ScopeId = sitemapParams.Scope,
                CultureName = culture.Name,
                Keywords = "*",
                StartingIndex = offset,
                MaximumItems = count,
                VariantGroupingStrategy = SearchVariantGroupingStrategy.PerProduct,
            };

            var response = await _overtureClient.SendAsync(request).ConfigureAwait(false);

            if (!response.Documents.Any())
            {
                return Enumerable.Empty<SitemapEntry>();                
            }

            return response.Documents.Select(document =>
            {
                if (!IsDocumentValid(document.PropertyBag))
                {
                    // If document is not valid just skip it...
                    return null;
                }

                return CreateStandardSitemapEntry(sitemapParams, document.PropertyBag, culture);

            }).Where(entry => entry != null);
        }

        protected virtual SitemapEntry CreateStandardSitemapEntry(SitemapParams sitemapParams, PropertyBag propertyBag,
            CultureInfo culture)
        {

            var productRelativeLocation = _productUrlProvider.GetProductUrl(
                _productUrlParamFactory.GetProductUrlParams(sitemapParams, culture, propertyBag)
            );

            var productAbsoluteLocation = new Uri(new Uri(sitemapParams.BaseUrl), productRelativeLocation).ToString();

            return new SitemapEntry
            {
                Location = productAbsoluteLocation,                
            };
        }

        protected virtual bool IsDocumentValid(PropertyBag propertyBag)
        {
            return RequiredDocumentKeys.All(key =>
            {
                var containsKey = propertyBag.ContainsKey(key);

                if (!containsKey)
                {
                    Log.Warn($"Skipping insertion of product {propertyBag[ProductId]} in sitemap because property {key} is not defined.");                    
                }

                return containsKey;
            });
        }
    }
}
