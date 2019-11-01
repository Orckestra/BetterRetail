using System.Web.Http;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Services;

namespace Composer.LoadTest
{
    /// <summary>
    /// WARNING This Controller is for test ONLY
    /// </summary>
    public class TestController : ApiController
    {
        private readonly IInventoryLocationProvider _inventoryLocationProvider;
        private readonly IComposerRequestContext _composerContext;

        public TestController(
            IInventoryLocationProvider inventoryLocationProvider, 
            IComposerRequestContext composerContext)
        {
            _inventoryLocationProvider = inventoryLocationProvider;
            _composerContext = composerContext;
        }

        [HttpPost]
        [ActionName("scopeAndInventoryLocation")]
        //[ValidateCsrfToken]
        public IHttpActionResult UpdateScopeAndInventoryLocation(string scopeAndInventoryLocationId)
        {
            _inventoryLocationProvider.SetDefaultInventoryLocationId(scopeAndInventoryLocationId);
            //_composerContext.Scope = scopeAndInventoryLocationId;

            return Ok(scopeAndInventoryLocationId);
        }
    }
}
