using Orckestra.Composer.Cart.Helper;
using Orckestra.Composer.Cart.Repositories.Order;
using Orckestra.Composer.Cart.Requests;
using Orckestra.Overture.ServiceModel.RecurringOrders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Cart.Services
{
    public class RecurringOrderTemplatesViewService : IRecurringOrderTemplatesViewService
    {
        private readonly IRecurringOrdersRepository _recurringOrderRepository;

        public RecurringOrderTemplatesViewService(IRecurringOrdersRepository recurringOrdersRepository)
        {
            _recurringOrderRepository = recurringOrdersRepository;
        }
        public async Task<bool> GetIsPaymentMethodUsedInRecurringOrders(GetIsPaymentMethodUsedInRecurringOrdersRequest request)
        {
            if (RecurringOrderCartHelper.IsRecurringOrdersEnabled())
                return false;

            if (request.ScopeId == null) { throw new ArgumentNullException(nameof(request.ScopeId)); }
            if (request.CustomerId == null) { throw new ArgumentNullException(nameof(request.CustomerId)); }
            if (request.CultureInfo == null) { throw new ArgumentNullException(nameof(request.CultureInfo)); }
            if (request.PaymentMethodId == null) { throw new ArgumentNullException(nameof(request.PaymentMethodId)); }

            var listOfRecurringOrderLineItems = await _recurringOrderRepository.GetRecurringOrderTemplates(request.ScopeId, request.CustomerId).ConfigureAwait(false);

            if (listOfRecurringOrderLineItems != null)
            {
                foreach (var item in listOfRecurringOrderLineItems.RecurringOrderLineItems ?? Enumerable.Empty<RecurringOrderLineItem>())
                {
                    if (item.PaymentMethodId == RecurringOrderCartHelper.ConvertStringToGuid(request.PaymentMethodId))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        
    }
}
