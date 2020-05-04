using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Orckestra.Composer.Logging;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Sitemap.Factory;
using Orckestra.Composer.Sitemap.Models;
using Orckestra.Overture;
using Orckestra.Overture.ServiceModel;
using Orckestra.Overture.ServiceModel.Requests.Search;
using Orckestra.Overture.ServiceModel.Search;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;

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
            _overtureClient = overtureClient ?? throw new ArgumentNullException(nameof(overtureClient));
            _productUrlProvider = productUrlProvider ?? throw new ArgumentNullException(nameof(productUrlProvider));
            _productUrlParamFactory = productUrlParamFactory;
        }

        public virtual async Task<IEnumerable<SitemapEntry>> GetEntriesAsync(SitemapParams param, CultureInfo culture, int offset, int count)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.BaseUrl)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.BaseUrl)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope)), nameof(param)); }
            if (culture == null) { throw new ArgumentNullException(nameof(culture)); }

            var request = new SearchProductRequest
            {
                ScopeId = param.Scope,
                CultureName = culture.Name,
                Keywords = "*",
                StartingIndex = offset,
                MaximumItems = count,
                VariantGroupingStrategy = SearchVariantGroupingStrategy.PerProduct,
            };

            var response = await _overtureClient.SendAsync(request).ConfigureAwait(false);

            if (!response.Documents.Any()) { return Enumerable.Empty<SitemapEntry>(); }

            return CreateStandardSitemapEntries(param, response.Documents, culture);
        }

        protected virtual IEnumerable<SitemapEntry> CreateStandardSitemapEntries(SitemapParams sitemapParams, List<Document> documents, CultureInfo culture)
        {
            foreach (var product in documents)
            {
                if (!IsDocumentValid(product.PropertyBag)) continue;

                var productRelativeLocation = _productUrlProvider.GetProductUrl(
                    _productUrlParamFactory.GetProductUrlParams(sitemapParams, culture, product.PropertyBag)
                );

                var productAbsoluteLocation = new Uri(new Uri(sitemapParams.BaseUrl), productRelativeLocation).ToString();

                yield return new SitemapEntry
                {
                    Location = productAbsoluteLocation,
                };
            }
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