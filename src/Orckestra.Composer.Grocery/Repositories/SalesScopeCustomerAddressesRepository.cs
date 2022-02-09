using Orckestra.Composer.MyAccount.Repositories;
using Orckestra.Composer.Providers;
using Orckestra.Overture;
using Orckestra.Overture.Caching;
using Orckestra.Overture.ServiceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Grocery.Repositories
{
    public class SalesScopeCustomerAddressesRepository: CustomerAddressRepository
    {
        protected IScopeProvider ScopeProvider { get; set; }

        public SalesScopeCustomerAddressesRepository(IOvertureClient overtureClient,
            ICacheProvider cacheProvider,
            IScopeProvider scopeProvider) : base(overtureClient, cacheProvider)
        {
            ScopeProvider = scopeProvider;
        }

        public override Task<List<Address>> GetCustomerAddressesAsync(Guid customerId, string scopeId)
        {
            var salesScope = ScopeProvider.DefaultScope;
            return base.GetCustomerAddressesAsync(customerId, salesScope);
        }

        public override Task<Address> CreateAddressAsync(Guid customerId, Address address, string scope)
        {
            var salesScope = ScopeProvider.DefaultScope;
            return base.CreateAddressAsync(customerId, address, salesScope);
        }

    }
}
