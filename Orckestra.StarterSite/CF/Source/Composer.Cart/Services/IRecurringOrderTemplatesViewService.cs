using Orckestra.Composer.Cart.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Cart.Services
{
    public interface IRecurringOrderTemplatesViewService
    {
        Task<bool> GetIsPaymentMethodUsedInRecurringOrders(GetIsPaymentMethodUsedInRecurringOrdersRequest request);

    }
}
