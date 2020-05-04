using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Orckestra.Composer.Enums;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Product.ViewModels.Inventory;
using Orckestra.Composer.Repositories;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel.Products.Inventory;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;

namespace Orckestra.Composer.Product.Services
{
    public class InventoryViewService : IInventoryViewService
    {
        protected IInventoryRepository InventoryRepository { get; private set; }
        protected IViewModelMapper ViewModelMapper { get; private set; }

        public InventoryViewService(IInventoryRepository inventoryRepository, IViewModelMapper viewModelMapper)
        {
            InventoryRepository = inventoryRepository ?? throw new ArgumentNullException(nameof(inventoryRepository));
            ViewModelMapper = viewModelMapper ?? throw new ArgumentNullException(nameof(viewModelMapper));
        }

        /// <summary>
        /// Retrieve the detail about the status of Inventory Items represented by the specified InventoryLocationId and a list of skus for the specified date
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual async Task<List<InventoryItemAvailabilityViewModel>> FindInventoryItemStatus(FindInventoryItemStatusParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (param.Scope == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.Scope)), nameof(param)); }
            if (param.Skus == null || !param.Skus.Any()) { throw new ArgumentException(GetMessageOfNullEmpty(nameof(param.Skus)), nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)), nameof(param)); }

            var inventoryItemsAvailability = await InventoryRepository.FindInventoryItemStatus(param).ConfigureAwait(false) ??
                throw new NullReferenceException("Inventory is not properly configured. Make sure Enable Inventory Management is set to True");

            var inventoryItemsAvailabilityViewModel = new List<InventoryItemAvailabilityViewModel>();

            foreach (var inventoryItemAvailability in inventoryItemsAvailability)
            {
                var inventoryItemStatusesViewModel = new List<InventoryItemStatusViewModel>();

                foreach (var inventoryItemStatus in inventoryItemAvailability.Statuses)
                {
                    inventoryItemStatusesViewModel.Add(new InventoryItemStatusViewModel
                    {
                        Quantity = inventoryItemStatus.Quantity,
                        Status = GetInventoryStatus(inventoryItemStatus.Status)
                    });
                }

                var inventoryItemIdentifierViewModel = ViewModelMapper.MapTo<InventoryItemIdentifierViewModel>(
                    inventoryItemAvailability.Identifier, param.CultureInfo);

                inventoryItemsAvailabilityViewModel.Add(new InventoryItemAvailabilityViewModel
                {
                    Date = inventoryItemAvailability.Date,
                    Identifier = inventoryItemIdentifierViewModel,
                    Statuses = inventoryItemStatusesViewModel
                });
            }

            return inventoryItemsAvailabilityViewModel;
        }

        protected virtual InventoryStatusEnum GetInventoryStatus(InventoryStatus inventoryStatus)
        {
            switch (inventoryStatus)
            {
                case InventoryStatus.BackOrder:
                    return InventoryStatusEnum.BackOrder;
                case InventoryStatus.InStock:
                    return InventoryStatusEnum.InStock;
                case InventoryStatus.OutOfStock:
                    return InventoryStatusEnum.OutOfStock;
                case InventoryStatus.PreOrder:
                    return InventoryStatusEnum.PreOrder;
                default:
                    return InventoryStatusEnum.Unspecified;
            }
        }
    }
}