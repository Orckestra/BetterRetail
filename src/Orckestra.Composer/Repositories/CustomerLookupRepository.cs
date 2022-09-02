﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orckestra.Composer.Configuration;
using Orckestra.Overture.Caching;
using Orckestra.Overture.ServiceModel.Metadata;
using Orckestra.Overture.ServiceModel.Requests.Metadata;

namespace Orckestra.Composer.Repositories
{
    public class CustomerLookupRepository : ILookupRepository
    {
        private readonly IComposerOvertureClient _overtureClient;
        private readonly ICacheProvider _cacheProvider;

        public CustomerLookupRepository(IComposerOvertureClient overtureClient, ICacheProvider cacheProvider)
        {
            _overtureClient = overtureClient ?? throw new ArgumentNullException(nameof(overtureClient));
            _cacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
        }

        /// <summary>
        /// Gets all the available product lookups.
        /// </summary>
        /// <returns></returns>
        public virtual Task<List<Lookup>> GetLookupsAsync()
        {
            var cacheKey = new CacheKey(CacheConfigurationCategoryNames.Lookup);
            cacheKey.AppendKeyParts("customerlookups");

            // the request type uniquely identifies what type of lookup is being searched
            var request = new GetCustomerLookupsRequest();

            //removed await because there is no logic after return from the call
            return _cacheProvider.GetOrAddAsync(cacheKey, () => _overtureClient.SendAsync(request));
        }

        public Task<Lookup> GetLookupAsync(string name)
        {
            var cacheKey = new CacheKey(CacheConfigurationCategoryNames.Lookup);
            cacheKey.AppendKeyParts(name);

            // the request type uniquely identifies what type of lookup is being searched
            var request = new GetCustomerLookupRequest {LookupName = name};

            //removed await because there is no logic after return from the call
            return _cacheProvider.GetOrAddAsync(cacheKey, () => _overtureClient.SendAsync(request));
        }
    }


    
}