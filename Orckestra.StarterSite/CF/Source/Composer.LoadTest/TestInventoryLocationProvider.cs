using Composer.LoadTest.Cookie;
using Orckestra.Composer.Product.Providers;
using Orckestra.Composer.Repositories;
using Orckestra.Composer.Services;
using Orckestra.ExperienceManagement.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Composer.LoadTest
{
    public class TestInventoryLocationProvider : ConfigurationInventoryLocationProvider
    {
        private readonly ICookieAccessor<TestCookieDto> _testCookieAccessor;

        public TestInventoryLocationProvider(
            IFulfillmentLocationsRepository fulfillmentLocationsRepository, 
            IInventoryRepository inventoryRepository,
            ICookieAccessor<TestCookieDto> testCookieAccessor,
            IWebsiteContext websiteContext,
            ISiteConfiguration siteConfiguration)
            
            : base(
            fulfillmentLocationsRepository, 
            inventoryRepository,
            websiteContext,
            siteConfiguration)
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
