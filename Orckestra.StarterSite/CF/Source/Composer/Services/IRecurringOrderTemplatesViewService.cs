
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Requests;
using Orckestra.Composer.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Services
{
    public interface IRecurringOrderTemplatesViewService
    {
        Task<bool> GetIsPaymentMethodUsedInRecurringOrders(GetIsPaymentMethodUsedInRecurringOrdersRequest request);

        Task<RecurringOrderTemplatesViewModel> GetRecurringOrderTemplatesViewModelAsync(GetRecurringOrderTemplatesParam param);
        Task<RecurringOrderTemplatesViewModel> UpdateRecurringOrderTemplateLineItemQuantityAsync(UpdateRecurringOrderTemplateLineItemQuantityParam param);
        Task<RecurringOrderTemplatesViewModel> RemoveRecurringOrderTemplateLineItemAsync(RemoveRecurringOrderTemplateLineItemParam request);
        Task<RecurringOrderTemplatesViewModel> RemoveRecurringOrderTemplatesLineItemsAsync(RemoveRecurringOrderTemplateLineItemsParam request);
        Task<RecurringOrderTemplatesViewModel> UpdateRecurringOrderTemplateLineItemAsync(UpdateRecurringOrderTemplateLineItemParam request);
        Task<RecurringOrderTemplateViewModel> GetRecurringOrderTemplateDetailViewModelAsync(GetRecurringOrderTemplateDetailParam param);


        //Task<RecurringOrderTemplatesViewModel> UpdateRecurringOrderTemplateLineItemQuantity(UpdateRecurringOrderTemplateLineItemQuantityRequest param);
        /* 
         Task<RecurringOrderProgramViewModel> GetRecurringOrderProgramAsync(GetRecurringOrderFrequenciesRequest param);
         Task<RecurringOrderProgramsViewModel> GetRecurringOrderProgramsByUserAsync(string scope, Guid customerId, CultureInfo culture);
         Task<RecurringOrderTemplatesViewModel> RemoveRecurringOrderTemplateLineItem(RemoveRecurringOrderTemplateLineItemRequest request);
         Task<RecurringOrderTemplatesViewModel> RemoveRecurringOrderTemplatesLineItems(RemoveRecurringOrderTemplateLineItemsRequest request);

         Task<RecurringOrderShippingMethodsViewModel> GetRecurringOrderShippingMethods(string scopeId, CultureInfo culture);
         Task<bool> ClearCustomerInactifItems(ClearCustomerInactifItemsRequest request);
         Task<RecurringOrderInactifProductsViewModel> GetInactifProducts(Guid customerId, string scope, CultureInfo cultureInfo);
         */
    }
}
