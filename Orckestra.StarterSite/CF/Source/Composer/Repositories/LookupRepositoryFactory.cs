using System;
using Orckestra.Composer.Enums;
using Orckestra.Overture;
using Orckestra.Overture.Caching;

namespace Orckestra.Composer.Repositories
{
    class LookupRepositoryFactory : ILookupRepositoryFactory
    {
        private readonly IOvertureClient _overtureClient;
        private readonly ICacheProvider _cacheProvider;

        public LookupRepositoryFactory(IOvertureClient overtureClient, ICacheProvider cacheProvider)
        {
            _overtureClient = overtureClient;
            _cacheProvider = cacheProvider;
        }

        public ILookupRepository CreateLookupRepository(LookupType lookupType)
        {
            switch (lookupType)
            {
                case LookupType.Product:    
                    return new ProductLookupRepository(_overtureClient, _cacheProvider);
                case LookupType.Order:
                    return new OrderLookupRepository(_overtureClient, _cacheProvider);
                case LookupType.Customer:
                    return new CustomerLookupRepository(_overtureClient, _cacheProvider);
                case LookupType.Marketing:
                    return new MarketingLookupRepository(_overtureClient, _cacheProvider);
            }
            

            throw new NotSupportedException(lookupType.ToString());
        }
    }
}
