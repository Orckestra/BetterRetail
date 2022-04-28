using Orckestra.Composer.Cart;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Repositories;
using Orckestra.Composer.Providers;
using Orckestra.Overture;
using Orckestra.Overture.Caching;
using Orckestra.Overture.ServiceModel.Orders;
using System.Threading.Tasks;

namespace Orckestra.Composer.Grocery.Repositories
{
    public class SalesScopeCartRepository : CartRepository
    {
        protected IScopeProvider ScopeProvider { get; set; }

        public SalesScopeCartRepository(IOvertureClient overtureClient, ICacheProvider cacheProvider, IScopeProvider scopeProvider) : base(overtureClient, cacheProvider)
        {
            ScopeProvider = scopeProvider;
        }
        public override Task<ProcessedCart> GetCartAsync(GetCartParam param)
        {
            if (param.CartType == CartConfiguration.OrderDraftCartType)
            {
                param.Scope = ScopeProvider.DefaultScope;
            }
            return base.GetCartAsync(param);
        }

        public override Task<ProcessedCart> RemoveLineItemsAsync(RemoveLineItemsParam param)
        {
            if (param.CartType == CartConfiguration.OrderDraftCartType)
            {
                param.Scope = ScopeProvider.DefaultScope;
            }
            return base.RemoveLineItemsAsync(param);
        }

        public override Task<ProcessedCart> AddLineItemAsync(AddLineItemParam param)
        {
            if (param.CartType == CartConfiguration.OrderDraftCartType)
            {
                param.Scope = ScopeProvider.DefaultScope;
            }
            return base.AddLineItemAsync(param);
        }

        public override Task<ProcessedCart> RemoveLineItemAsync(RemoveLineItemParam param)
        {
            if (param.CartType == CartConfiguration.OrderDraftCartType)
            {
                param.Scope = ScopeProvider.DefaultScope;
            }
            return base.RemoveLineItemAsync(param);
        }

        public override Task<ProcessedCart> UpdateLineItemAsync(UpdateLineItemParam param)
        {
            if (param.CartType == CartConfiguration.OrderDraftCartType)
            {
                param.ScopeId = ScopeProvider.DefaultScope;
            }
            return base.UpdateLineItemAsync(param);
        }
    }
}