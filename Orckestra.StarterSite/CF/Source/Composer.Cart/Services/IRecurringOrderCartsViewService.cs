using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Cart.Services
{
    public interface IRecurringOrderCartsViewService
    {
        Task<RecurringOrderCartsViewModel> GetRecurringOrderCartListViewModelAsync(GetRecurringOrderCartsViewModelParam param);
        Task<RecurringOrderCartsViewModel> GetRecurringOrderCartListViewModelFromCartsAsync(GetRecurringOrderCartsViewModelFromCartsParam param);
        Task<LightRecurringOrderCartsViewModel> GetLightRecurringOrderCartListViewModelAsync(GetLightRecurringOrderCartListViewModelParam param);
        Task<CartViewModel> GetRecurringOrderCartViewModelAsync(GetRecurringOrderCartViewModelParam param);
        Task<CartViewModel> UpdateRecurringOrderCartShippingAddressAsync(UpdateRecurringOrderCartShippingAddressParam param);
        Task<CartViewModel> UpdateRecurringOrderCartBillingAddressAsync(UpdateRecurringOrderCartBillingAddressParam param);
        Task<CartViewModel> CreateCartViewModelAsync(CreateRecurringOrderCartViewModelParam param);
        Task<RecurringOrderCartsRescheduleResultViewModel> UpdateRecurringOrderCartNextOccurenceAsync(UpdateRecurringOrderCartNextOccurenceParam param);
        Task<CartViewModel> RemoveLineItemAsync(RemoveRecurringCartLineItemParam removeRecurringCartLineItemParam);
        Task<CartViewModel> UpdateLineItemAsync(UpdateLineItemParam updateLineItemParam);
        Task<bool> UpdateRecurringOrderCartsAddressesAsync(UpdateRecurringOrderCartsAddressesParam param);
    }
}