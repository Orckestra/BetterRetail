using System.Linq;
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
    public class StoreLocatorController : ApiController
    {
        protected IStoreLocatorViewService StoreLocatorViewService { get; private set; }
        protected IStoreViewService StoreViewService { get; private set; }
        protected IMapConfigurationViewService MapConfigurationViewService { get; private set; }
        protected IComposerContext ComposerContext { get; private set; }

        public StoreLocatorController(IStoreLocatorViewService storesListViewService,
            IStoreViewService storeViewService,
            IComposerContext composerContext,
            IMapConfigurationViewService mapConfigurationViewService)
        {
            StoreLocatorViewService = storesListViewService;
            ComposerContext = composerContext;
            StoreViewService = storeViewService;
            MapConfigurationViewService = mapConfigurationViewService;

        }

        [ActionName("store")]
        [HttpPost]
        [ValidateModelState]
        public virtual async Task<IHttpActionResult> GetStore(StoreRequest request)
        {
            var baseUrl = RequestUtils.GetBaseUrl(Request).ToString();
            var vm = await StoreViewService.GetStoreViewModelAsync(new GetStoreByNumberParam
            {
                BaseUrl = baseUrl,
                CultureInfo = ComposerContext.CultureInfo,
                Scope = ComposerContext.Scope,
                StoreNumber = request.StoreNumber
            });
            return Ok(vm);
        }

        [ActionName("stores")]
        [HttpPost]
        [ValidateModelState]
        public virtual async Task<IHttpActionResult> GetStores(StoresRequest request)
        {
            var vm = await StoreLocatorViewService.GetStoreLocatorViewModelAsync(new GetStoreLocatorViewModelParam
            {
                Scope = ComposerContext.Scope,
                CultureInfo = ComposerContext.CultureInfo,
                BaseUrl = RequestUtils.GetBaseUrl(Request).ToString(),
                PageNumber = request.Page,
                PageSize = request.PageSize > 0 ? request.PageSize : StoreConfiguration.StoreLocatorMaxItemsPerPage,
                MapBounds = request.MapBounds,
                SearchPoint = request.SearchPoint,
                IncludeMarkers = false
            }).ConfigureAwait(false);

            return Ok(vm);
        }

        [ActionName("markers")]
        [HttpPost]
        [ValidateModelState]
        public virtual async Task<IHttpActionResult> GetMarkers(MarkersRequest request)
        {
            var vm = await StoreLocatorViewService.GetStoreLocatorViewModelAsync(new GetStoreLocatorViewModelParam
            {
                Scope = ComposerContext.Scope,
                CultureInfo = ComposerContext.CultureInfo,
                BaseUrl = RequestUtils.GetBaseUrl(Request).ToString(),
                PageNumber = 1,
                PageSize = request.PageSize > 0 ? request.PageSize : StoreConfiguration.StoreLocatorMaxItemsPerPage,
                MapBounds = request.MapBounds,
                ZoomLevel = request.ZoomLevel,
                SearchPoint = request.SearchPoint,
                IncludeMarkers = true
            }).ConfigureAwait(false);

            if (request.IsSearch && !vm.Markers.Any() && vm.NearestStoreCoordinate != null)
            {
                // if no markers in requested search bounds, return the coordinates of the nearest store, so always see one store on the map
                return Ok(vm.NearestStoreCoordinate.GetCoordinate());
            }

            return Ok(vm);
        }

        [ActionName("mapconfiguration")]
        [HttpGet]
        [ValidateModelState]
        public virtual async Task<IHttpActionResult> GetMapConfiguration()
        {
            var vm = await
                MapConfigurationViewService.GetMapConfigurationViewModelAsync(new GetMapConfigurationViewModelParam
                {
                    Scope = ComposerContext.Scope,
                    LoadStoresBounds = true
                });

            return Ok(vm);
        }
    }
}
