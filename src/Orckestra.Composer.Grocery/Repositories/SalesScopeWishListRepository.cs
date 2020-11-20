using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Repositories;
using Orckestra.Composer.Providers;
using Orckestra.Overture;
using Orckestra.Overture.Caching;
using Orckestra.Overture.ServiceModel.Orders;
using System;
using System.Threading.Tasks;

namespace Orckestra.Composer.Grocery.Repositories
{
    public class SalesScopeWishListRepository: WishListRepository
    {
        private readonly IScopeProvider _scopeProvider;
        public SalesScopeWishListRepository(IOvertureClient overtureClient, ICacheProvider cacheProvider, IScopeProvider scopeProvider) : base(overtureClient, cacheProvider)
        {
            _scopeProvider = scopeProvider ?? throw new ArgumentNullException(nameof(scopeProvider));
        }

        public override async Task<ProcessedCart> GetWishListAsync(GetCartParam param)
        {
            var salesParam = param.Clone();
            salesParam.Scope = _scopeProvider.DefaultScope;
            return await base.GetWishListAsync(salesParam).ConfigureAwait(false);
        }

        public override async Task<ProcessedCart> AddLineItemAsync(AddLineItemParam param)
        {
            var salesParam = param.Clone();
            salesParam.Scope = _scopeProvider.DefaultScope;
            return await base.AddLineItemAsync(salesParam).ConfigureAwait(false);
        }

        public override async Task<ProcessedCart> RemoveLineItemAsync(RemoveLineItemParam param)
        {
            var salesParam = param.Clone();
            salesParam.Scope = _scopeProvider.DefaultScope;
            return await base.RemoveLineItemAsync(salesParam).ConfigureAwait(false);
        }
    }
}
