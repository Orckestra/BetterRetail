using System.Threading.Tasks;
using System.Web.Http;
using Orckestra.Composer.Services;
using Orckestra.Composer.Store.Parameters;
using Orckestra.Composer.Store.Requests;
using Orckestra.Composer.Store.Services;
using Orckestra.Composer.Utils;
using Orckestra.Composer.WebAPIFilters;


namespace Orckestra.Composer.Store.Api
{
    [ValidateLanguage]
    [JQueryOnlyFilter]
    [ValidateModelState]
    public class StoreInventoryController : ApiController
    {
        protected IStoreInventoryViewService StoreInventoryViewService { get; private set; }
        protected IComposerContext ComposerContext { get; private set; }

        public StoreInventoryController(IStoreInventoryViewService storeInventoryViewService,
            IComposerContext composerContext)
        {
            StoreInventoryViewService = storeInventoryViewService;
            ComposerContext = composerContext;
        }

        [ActionName("storesinventory")]
        [HttpPost]
        [ValidateModelState]
        public virtual async Task<IHttpActionResult> GetStoresInventory(StoresInventoryRequest request)
        {
            var vm = await StoreInventoryViewService.GetStoreInventoryViewModelAsync(new GetStoreInventoryViewModelParam
            {
                Scope = ComposerContext.Scope,
                CultureInfo = ComposerContext.CultureInfo,
                BaseUrl = RequestUtils.GetBaseUrl(Request).ToString(),
                Sku = request.Sku,
                SearchPoint = request.SearchPoint,
                PageNumber = request.Page,
                PageSize = request.PageSize > 0 ? request.PageSize : StoreConfiguration.InventoryListMaxItemsPerPage
            }).ConfigureAwait(false);

            return Ok(vm);
        }
    }
}
