using System;
using System.Threading.Tasks;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Configuration;
using Orckestra.Composer.Providers;
using Orckestra.Overture;
using Orckestra.Overture.Caching;
using Orckestra.Overture.ServiceModel.Orders;
using Orckestra.Overture.ServiceModel.Requests.Orders.Shopping;
using Orckestra.Overture.ServiceModel.Requests.Orders.Shopping.LineItems;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;

namespace Orckestra.Composer.Cart.Repositories
{
    public class WishListRepository: IWishListRepository
    {
        protected IOvertureClient OvertureClient { get; private set; }
        protected ICacheProvider CacheProvider { get; private set; }

        public WishListRepository(IOvertureClient overtureClient, ICacheProvider cacheProvider)
        {
            OvertureClient = overtureClient ?? throw new ArgumentNullException(nameof(overtureClient));
            CacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
        }

        public virtual async Task<ProcessedCart> GetWishListAsync(GetCartParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope)), nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.CartName)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.CartName)), nameof(param)); }
            if (param.CustomerId == Guid.Empty) { throw new ArgumentException(GetMessageOfEmpty(nameof(param.CustomerId)), nameof(param)); }

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
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope)), nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.CartName)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.CartName)), nameof(param)); }
            if (param.CustomerId == Guid.Empty) { throw new ArgumentException(GetMessageOfEmpty(nameof(param.CustomerId)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.ProductId)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.ProductId)), nameof(param)); }
            if (param.Quantity < 1) { throw new ArgumentOutOfRangeException(nameof(param), param.Quantity, GetMessageOfZeroNegative(nameof(param.Quantity))); }

            var request = BuildAddLineItemRequestFromParam(param);
            var cacheKey = BuildWishListCacheKey(param.Scope, param.CustomerId, param.CartName);

            return await CacheProvider.ExecuteAndSetAsync(cacheKey, () => OvertureClient.SendAsync(request)).ConfigureAwait(false);
        }

        public virtual async Task<ProcessedCart> RemoveLineItemAsync(RemoveLineItemParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope)), nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.CartName)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.CartName)), nameof(param)); }
            if (param.LineItemId == Guid.Empty) { throw new ArgumentException(GetMessageOfEmpty(nameof(param.LineItemId)), nameof(param)); }
            if (param.CustomerId == Guid.Empty) { throw new ArgumentException(GetMessageOfEmpty(nameof(param.CustomerId)), nameof(param)); }

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
                VariantId = param.VariantId,
                RecurringOrderFrequencyName = param.RecurringOrderFrequencyName,
                RecurringOrderProgramName = param.RecurringOrderProgramName
            };
        }

        /// <summary>
        /// Builds the cache key for a WishList.
        /// </summary>
        protected virtual CacheKey BuildWishListCacheKey(string scope, Guid customerId, string cartName)
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
