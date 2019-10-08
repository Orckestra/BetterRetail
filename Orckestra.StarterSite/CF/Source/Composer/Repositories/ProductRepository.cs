using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orckestra.Composer.Configuration;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Utils;
using Orckestra.Overture;
using Orckestra.Overture.Caching;
using Orckestra.Overture.ServiceModel.Metadata;
using Orckestra.Overture.ServiceModel.Products;
using Orckestra.Overture.ServiceModel.Requests.Products;
using Orckestra.Overture.ServiceModel.Requests.Search;
using Orckestra.Overture.ServiceModel.Search;

namespace Orckestra.Composer.Repositories
{
    /// <summary>
    /// Implementation of the <see cref="IProductRepository"/> using Overture.
    /// </summary>
    public class ProductRepository : IProductRepository
    {
        protected readonly IOvertureClient _overtureClient;
        protected readonly ICacheProvider _cacheProvider;

        public ProductRepository(IOvertureClient overtureClient, ICacheProvider cacheProvider)
        {
            if (overtureClient == null) { throw new ArgumentNullException("overtureClient"); }
            if (cacheProvider == null) { throw new ArgumentNullException("cacheProvider"); }

            _overtureClient = overtureClient;
            _cacheProvider = cacheProvider;
        }

        /// <summary>
        /// Gets a product.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <returns>
        /// Instance of <see cref="Orckestra.Overture.ServiceModel.Products.Product" />.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">param</exception>
        /// <exception cref="System.ArgumentException"></exception>
        public virtual async Task<Overture.ServiceModel.Products.Product> GetProductAsync(GetProductParam param)
        {
            if (param == null)
            {
                throw new ArgumentNullException("param");
            }

            if (string.IsNullOrWhiteSpace(param.Scope))
            {
                throw new ArgumentException("param.Scope must not be null or whitespace.", "param");
            }

            if (param.CultureInfo == null)
            {
                throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("CultureInfo"), "param");
            }

            if (string.IsNullOrWhiteSpace(param.ProductId))
            {
                throw new ArgumentException("param.ProductId must not be null or whitespace.", "param");
            }

            var productCacheKey = new CacheKey(CacheConfigurationCategoryNames.Product)
            {
                Scope = param.Scope,
            };

            productCacheKey.AppendKeyParts(param.ProductId);

            var result = await _cacheProvider.GetOrAddAsync(productCacheKey, () =>
                {
                    var request = new GetProductV2Request
                    {
                        //get all cultures to avoid reloading product page to retrieve new product details
                        CultureName = string.Empty,
                        IncludePriceLists = true,
                        IncludeRelationships = true,
                        IncludeVariants = true,
                        ProductId = param.ProductId,
                        ScopeId = param.Scope,
                        IncludeMedia = true,
                        IncludeImageUrl = true

                    };

                    return _overtureClient.SendAsync(request);
                }).ConfigureAwait(false);

            return param.ReturnInactive || (result != null && result.Active.HasValue && result.Active.Value) ? result : null;
        }

        public virtual async Task<ProductDefinition> GetProductDefinitionAsync(GetProductDefinitionParam param)
        {
            if (param == null)
            {
                throw new ArgumentNullException("param");
            }

            var productDefinitionCacheKey = new CacheKey(CacheConfigurationCategoryNames.ProductDefinition);
            productDefinitionCacheKey.AppendKeyParts(param.Name);

            var result = await _cacheProvider.GetOrAddAsync(productDefinitionCacheKey, () =>
            {
                var request = new GetProductDefinitionRequest
                {
                    Name = param.Name,
                    CultureName = param.CultureInfo.Name
                };

                return _overtureClient.SendAsync(request);
            }).ConfigureAwait(false);

            return result;
        }


        /// <summary>
        /// Gets products prices.
        /// </summary>
        /// <param name="productIds">The product ids.</param>
        /// <param name="scope">The scope.</param>
        /// <returns></returns>
        public virtual Task<List<ProductPrice>> CalculatePricesAsync(List<string> productIds, string scope)
        {
            if (productIds == null)
            {
                throw new ArgumentNullException("productIds");
            }

            if (scope == null)
            {
                throw new ArgumentNullException("scope");
            }

            var request = new CalculatePricesofProductsRequest
            {
                ProductIds = productIds,
                ScopeId = scope,
                IncludeVariants = true
            };

            return _overtureClient.SendAsync(request);
        }

        /// <summary>
        /// Gets the effective price info for a given product.
        /// </summary>
        /// <param name="productId">The product id.</param>
        /// <param name="scope">The scope.</param>
        /// <returns></returns>
        public virtual Task<EffectivePriceEntryInfo> GetEffectivePrice(string productId, string scope)
        {
            if (productId == null) { throw new ArgumentNullException("productId"); }
            if (scope == null) { throw new ArgumentNullException("scope"); }

            var request = new GetEffectivePriceEntryInfoRequest()
            {
                ProductId = productId,
                ScopeId = scope,
            };

            return _overtureClient.SendAsync(request);
        }


        public Task<SearchResult> SearchProductByIdsAsync(List<string> productIds, string scope, string cultureName)
        {
            if (productIds == null) { throw new ArgumentNullException("productIds"); }
            if (scope == null) {  throw new ArgumentNullException("scope"); }

            var request = new SearchProductByIdsRequest
            {
                Ids = productIds,
                ScopeId = scope,
                CultureName = cultureName
            };

            return _overtureClient.SendAsync(request);
        }

        public Task<MediaList> GetProductMediaAsync(string sku, string scope, string cultureName)
        {
            if (sku == null) { throw new ArgumentNullException("sku"); }
            if (scope == null) { throw new ArgumentNullException("scope"); }

            var request = new GetMediaBySkuRequest
            {
                Sku = sku,
                ScopeId = scope,
                CultureName = cultureName
            };

            return _overtureClient.SendAsync(request);
        }
    }
}