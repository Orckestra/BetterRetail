using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Composer.LoadTest.Cookie;
using Orckestra.Composer.Product.Providers;
using Orckestra.Composer.Product.Repositories;
using Orckestra.Composer.Repositories;
using Orckestra.Composer.Services;

namespace Composer.LoadTest
{
    public class TestInventoryLocationProvider : ConfigurationInventoryLocationProvider
    {
        private readonly ICookieAccessor<TestCookieDto> _testCookieAccessor;

        public TestInventoryLocationProvider(
            IFulfillmentLocationsRepository fulfillmentLocationsRepository, 
            IInventoryRepository inventoryRepository,
            ICookieAccessor<TestCookieDto> testCookieAccessor)
            
            : base(
            fulfillmentLocationsRepository, 
            inventoryRepository)
        {
            _testCookieAccessor = testCookieAccessor;
        }

        public override Task<string> GetDefaultInventoryLocationIdAsync()
        {
            var payload = _testCookieAccessor.Read();

            return Task.FromResult(payload.ScopeAndLocationId);
        }

        public override Task<List<string>> GetInventoryLocationIdsForSearchAsync()
        {
            var payload = _testCookieAccessor.Read();

            return Task.FromResult(new List<string>{ payload.ScopeAndLocationId });
        }

        public override string SetDefaultInventoryLocationId(string inventoryLocationId)
        {
            var testCookieDto = _testCookieAccessor.Read();
            testCookieDto.ScopeAndLocationId = inventoryLocationId;
            _testCookieAccessor.Write(testCookieDto);

            return inventoryLocationId;
        }
    }
}
