using Orckestra.Composer.Services;
using Orckestra.Composer.Store.Parameters;
using Orckestra.Composer.Store.Requests;
using Orckestra.Composer.Store.Services;
using Orckestra.Composer.Utils;
using Orckestra.Composer.WebAPIFilters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Orckestra.Composer.Store.Api
{
    [ValidateLanguage]
    [JQueryOnlyFilter]
    [ValidateModelState]
    public class StoreController : ApiController
    {
        protected IStoreViewService StoreViewService { get; private set; }
        protected IComposerContext ComposerContext { get; private set; }

        public StoreController(IStoreViewService storeViewService,
            IComposerContext composerContext)
        {
            StoreViewService = storeViewService;
            ComposerContext = composerContext;
        }

        [ActionName("stores")]
        [HttpGet]
        [ValidateModelState]
        public virtual async Task<IHttpActionResult> GetStores()
        {
            var vm = await StoreViewService.GetAllStoresViewModelAsync(new GetStoresParam
            {
                BaseUrl = RequestUtils.GetBaseUrl(Request).ToString(),
                CultureInfo = ComposerContext.CultureInfo,
                Scope = ComposerContext.Scope
            });
            return Ok(vm);
        }

        [ActionName("stores")]
        [HttpPost]
        [ValidateModelState]
        public virtual async Task<IHttpActionResult> GetStoresByLocation(StoresRequest request)
        {
            var vm = await StoreViewService.GetAllStoresViewModelAsync(new GetStoresParam
            {
                BaseUrl = RequestUtils.GetBaseUrl(Request).ToString(),
                CultureInfo = ComposerContext.CultureInfo,
                Scope = ComposerContext.Scope,
                SearchPoint = request.SearchPoint
            });
            return Ok(vm);
        }
    }
}
