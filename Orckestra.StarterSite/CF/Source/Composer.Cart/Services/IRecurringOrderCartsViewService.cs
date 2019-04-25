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
        Task<LightRecurringOrderCartsViewModel> GetLightRecurringOrderCartListViewModelAsync(GetLightRecurringOrderCartListViewModelParam param);
        Task<IRecurringOrderCartViewModel> GetRecurringOrderCartViewModelAsync(GetRecurringOrderCartViewModelParam param);
        Task<IRecurringOrderCartViewModel> UpdateRecurringOrderCartShippingAddressAsync(UpdateRecurringOrderCartShippingAddressParam param);
        Task<IRecurringOrderCartViewModel> UpdateRecurringOrderCartBillingAddressAsync(UpdateRecurringOrderCartBillingAddressParam param);
        Task<IRecurringOrderCartViewModel> CreateCartViewModelAsync(CreateRecurringOrderCartViewModelParam param);
        Task<RecurringOrderCartsViewModel> UpdateRecurringOrderCartNextOccurenceAsync(UpdateRecurringOrderCartNextOccurenceParam param);
    }
}