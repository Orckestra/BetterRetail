using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Orckestra.Composer.Enums;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Product.Requests;
using Orckestra.Composer.Product.Services;
using Orckestra.Composer.Product.ViewModels.Inventory;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Services;
using Orckestra.Composer.WebAPIFilters;

namespace Orckestra.Composer.Product.Api
{
    [ValidateLanguage]
    [JQueryOnlyFilter]
    public class InventoryController : ApiController
    {
        protected IComposerContext ComposerContext { get; private set; }
        protected IInventoryViewService InventoryViewService { get; private set; }
        protected IInventoryLocationProvider InventoryLocationProvider { get; set; }
        protected IProductSettingsViewService ProductSettingsViewService { get; set; }

        public InventoryController(
            IComposerContext composerContext,
            IInventoryViewService inventoryViewService,
            IInventoryLocationProvider inventoryLocationProvider,
            IProductSettingsViewService productSettingsViewService)
        {
            ComposerContext = composerContext ?? throw new ArgumentNullException(nameof(composerContext));
            InventoryViewService = inventoryViewService ?? throw new ArgumentNullException(nameof(inventoryViewService));
            InventoryLocationProvider = inventoryLocationProvider ?? throw new ArgumentNullException(nameof(inventoryLocationProvider));
            ProductSettingsViewService = productSettingsViewService ?? throw new ArgumentNullException(nameof(productSettingsViewService));
        }

        [ActionName("findInventoryItems")]
        [HttpPost]
        [ValidateModelState]
        public virtual async Task<IHttpActionResult> FindInventoryItems(FindInventoryItemsRequest request)
        {
            if (request == null) { return BadRequest("No request found."); }

            List<string> productSkusAvailableToSell;

            //TODO: Log if inventory is enabled or disabled
            if (await IsInventoryEnabled())
            {
                productSkusAvailableToSell = await GetInventoryItems(request.Skus.ToList());
            }
            else
            {
                //Inventory is disabled. Will return the same list of sku as available.
                productSkusAvailableToSell = request.Skus.ToList();
            }

            return Ok(productSkusAvailableToSell);
        }

        protected virtual async Task<List<string>> GetInventoryItems(List<string> skus)
        {
            var availableInventoryStatuses = GetAvailableInventoryStatuses();
            var inventoryItemsAvailabilityViewModel = await FindInventoryItemStatus(skus);

            var productSkusAvailableToSell = GetProductSkusAvailableToSell(availableInventoryStatuses, inventoryItemsAvailabilityViewModel);

            return productSkusAvailableToSell.ToList();
        }

        protected virtual async Task<List<InventoryItemAvailabilityViewModel>> FindInventoryItemStatus(List<string> skus)
        {
            var inventoryItemsAvailabilityViewModel = await InventoryViewService.FindInventoryItemStatus(new FindInventoryItemStatusParam
            {
                CultureInfo = ComposerContext.CultureInfo,
                Scope = ComposerContext.Scope,
                Date = DateTime.UtcNow,
                Skus = skus,
                InventoryLocationId = await InventoryLocationProvider.GetDefaultInventoryLocationIdAsync()
            });

            return inventoryItemsAvailabilityViewModel;
        }

        private static IEnumerable<string> GetProductSkusAvailableToSell(
            ICollection<InventoryStatusEnum> availableInventoryStatuses,
            IEnumerable<InventoryItemAvailabilityViewModel> inventoryItemsAvailabilityViewModel)
        {
            return from inventoryItemAvailabilityViewModel
                   in inventoryItemsAvailabilityViewModel
                   let firstStatus = inventoryItemAvailabilityViewModel.Statuses.FirstOrDefault()
                   where firstStatus != null && availableInventoryStatuses.Contains(firstStatus.Status)
                   select inventoryItemAvailabilityViewModel.Identifier.Sku;
        }

        protected virtual async Task<bool> IsInventoryEnabled()
        {
            var productSettingsViewModel = await ProductSettingsViewService.GetProductSettings(ComposerContext.Scope, ComposerContext.CultureInfo);

            return productSettingsViewModel.IsInventoryEnabled;
        }

        protected static List<InventoryStatusEnum> GetAvailableInventoryStatuses()
        {
            return ComposerConfiguration.AvailableStatusForSell;
        }
    }
}