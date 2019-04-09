using Orckestra.Composer.Cart.Requests;
using Orckestra.Composer.Cart.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Cart.Services
{
    public interface IRecurringOrderTemplatesViewService
    {
        Task<bool> GetIsPaymentMethodUsedInRecurringOrders(GetIsPaymentMethodUsedInRecurringOrdersRequest request);

        Task<RecurringOrderTemplatesViewModel> GetRecurringOrderTemplatesAsync(string scope, Guid customerId, CultureInfo culture, string baseUrl);
        //Task<RecurringOrderTemplatesViewModel> UpdateRecurringOrderTemplateLineItemQuantity(UpdateRecurringOrderTemplateLineItemQuantityRequest param);
       /* 
        Task<RecurringOrderProgramViewModel> GetRecurringOrderProgramAsync(GetRecurringOrderFrequenciesRequest param);
        Task<RecurringOrderProgramsViewModel> GetRecurringOrderProgramsByUserAsync(string scope, Guid customerId, CultureInfo culture);
        Task<RecurringOrderTemplatesViewModel> UpdateRecurringOrderTemplateLineItem(UpdateRecurringOrderTemplateLineItemRequest request);
        Task<RecurringOrderTemplatesViewModel> RemoveRecurringOrderTemplateLineItem(RemoveRecurringOrderTemplateLineItemRequest request);
        Task<RecurringOrderTemplatesViewModel> RemoveRecurringOrderTemplatesLineItems(RemoveRecurringOrderTemplateLineItemsRequest request);
        
        Task<RecurringOrderShippingMethodsViewModel> GetRecurringOrderShippingMethods(string scopeId, CultureInfo culture);
        Task<bool> ClearCustomerInactifItems(ClearCustomerInactifItemsRequest request);
        Task<RecurringOrderInactifProductsViewModel> GetInactifProducts(Guid customerId, string scope, CultureInfo cultureInfo);
        */
    }
}
