﻿using Orckestra.Composer.Parameters;
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

        public ProductSitemapEntryProvider(IOvertureClient overtureClient, IProductUrlProvider productUrlProvider)
        {
            Guard.NotNull(overtureClient, nameof(overtureClient));
            Guard.NotNull(productUrlProvider, nameof(productUrlProvider));

            _overtureClient = overtureClient;
            _productUrlProvider = productUrlProvider;
        }

        public virtual async Task<IEnumerable<SitemapEntry>> GetEntriesAsync(string baseUrl, string scope, CultureInfo culture, int offset, int count)
        {
            Guard.NotNullOrWhiteSpace(baseUrl, nameof(baseUrl));
            Guard.NotNullOrWhiteSpace(scope, nameof(scope));
            Guard.NotNull(culture, nameof(culture));            

            var request = new SearchProductRequest
            {
                ScopeId = scope,
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

                return CreateStandardSitemapEntry(document.PropertyBag, baseUrl, culture);

            }).Where(entry => entry != null);
        }

        protected virtual SitemapEntry CreateStandardSitemapEntry(PropertyBag propertyBag, string baseUrl, CultureInfo culture)
        {
            var productRelativeLocation = _productUrlProvider.GetProductUrl(new GetProductUrlParam
            {                
                CultureInfo = culture,
                ProductId = (string)propertyBag["ProductId"],
                ProductName = (string)propertyBag["DisplayName"],
            });

            var productAbsoluteLocation = new Uri(new Uri(baseUrl), productRelativeLocation).ToString();

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
