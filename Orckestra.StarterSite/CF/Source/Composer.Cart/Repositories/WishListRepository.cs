﻿using System;
using System.Threading.Tasks;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Configuration;
using Orckestra.Composer.Providers;
using Orckestra.Overture;
using Orckestra.Overture.Caching;
using Orckestra.Overture.ServiceModel.Orders;
using Orckestra.Overture.ServiceModel.Requests.Orders.Shopping;
using Orckestra.Overture.ServiceModel.Requests.Orders.Shopping.LineItems;

namespace Orckestra.Composer.Cart.Repositories
{
    public class WishListRepository: IWishListRepository
    {
        protected IOvertureClient OvertureClient { get; private set; }
        protected ICacheProvider CacheProvider { get; private set; }

        public WishListRepository(IOvertureClient overtureClient, ICacheProvider cacheProvider)
        {
            if (overtureClient == null) { throw new ArgumentNullException("overtureClient"); }
            if (cacheProvider == null) { throw new ArgumentNullException("cacheProvider"); }

            OvertureClient = overtureClient;
            CacheProvider = cacheProvider;
        }

        public virtual async Task<ProcessedCart> GetWishListAsync(GetCartParam param)
        {
            if (param == null) { throw new ArgumentNullException("param", "param is required"); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException("param.Scope is required", "param"); }
            if (param.CultureInfo == null) { throw new ArgumentException("param.CultureInfo is required", "param"); }
            if (string.IsNullOrWhiteSpace(param.CartName)) { throw new ArgumentException("param.CartName is required", "param"); }
            if (param.CustomerId == Guid.Empty) { throw new ArgumentException("param.CustomerId is required", "param"); }

            var cacheKey = BuildWishListCacheKey(param.Scope, param.CustomerId, param.CartName);

            var request = new GetCartRequest
            {
                CultureName = param.CultureInfo.Name,
                CustomerId = param.CustomerId,
                ScopeId = param.Scope,
                CartName = param.CartName,
                ExecuteWorkflow = param.ExecuteWorkflow,
                WorkflowToExecute = param.WorkflowToExecute
            };

            return await CacheProvider.GetOrAddAsync(cacheKey, () => OvertureClient.SendAsync(request)).ConfigureAwait(false);
        }

        public virtual async Task<ProcessedCart> AddLineItemAsync(AddLineItemParam param)
        {
            if (param == null) { throw new ArgumentNullException("param", "param is required"); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException("param.Scope is required", "param"); }
            if (param.CultureInfo == null) { throw new ArgumentException("param.CultureInfo is required", "param"); }
            if (string.IsNullOrWhiteSpace(param.CartName)) { throw new ArgumentException("param.CartName is required", "param"); }
            if (param.CustomerId == Guid.Empty) { throw new ArgumentException("param.CustomerId is required", "param"); }
            if (string.IsNullOrWhiteSpace(param.ProductId)) { throw new ArgumentException("param.ProductId is required", "param"); }
            if (param.Quantity <= 0) { throw new ArgumentException("param.Quantity must be positive, greater than 0", "param"); }

            var request = BuildAddLineItemRequestFromParam(param);
            var cacheKey = BuildWishListCacheKey(param.Scope, param.CustomerId, param.CartName);

            return await CacheProvider.ExecuteAndSetAsync(cacheKey, () => OvertureClient.SendAsync(request)).ConfigureAwait(false);
        }

        public virtual async Task<ProcessedCart> RemoveLineItemAsync(RemoveLineItemParam param)
        {
            if (param == null) { throw new ArgumentNullException("param", "param is required"); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException("param.Scope is required", "param"); }
            if (param.CultureInfo == null) { throw new ArgumentException("param.CultureInfo is required", "param"); }
            if (string.IsNullOrWhiteSpace(param.CartName)) { throw new ArgumentException("param.CartName is required", "param"); }
            if (param.LineItemId == Guid.Empty) { throw new ArgumentException("param.LineItemId is required", "param"); }
            if (param.CustomerId == Guid.Empty) { throw new ArgumentException("param.CustomerId is required", "param"); }

            var request = new RemoveLineItemRequest
            {
                ScopeId = param.Scope,
                CultureName = param.CultureInfo.Name,
                CartName = param.CartName,
                Id = param.LineItemId,
                CustomerId = param.CustomerId
            };

            var cacheKey = BuildWishListCacheKey(param.Scope, param.CustomerId, param.CartName);
            return await CacheProvider.ExecuteAndSetAsync(cacheKey, () => OvertureClient.SendAsync(request)).ConfigureAwait(false);
        }

        protected virtual AddLineItemRequest BuildAddLineItemRequestFromParam(AddLineItemParam param)
        {
            return new AddLineItemRequest
            {
                ScopeId = param.Scope,
                CultureName = param.CultureInfo.Name,
                CartName = param.CartName,
                CustomerId = param.CustomerId,
                ProductId = param.ProductId,
                Quantity = param.Quantity,
                VariantId = param.VariantId
            };
        }

        /// <summary>
        /// Builds the cache key for a WishList.
        /// </summary>
        protected CacheKey BuildWishListCacheKey(string scope, Guid customerId, string cartName)
        {
            var key = new CacheKey(CacheConfigurationCategoryNames.Cart)
            {
                Scope = scope
            };

            key.AppendKeyParts(customerId, cartName);
            return key;
        }
    }
}
