using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orckestra.Overture.ServiceModel.RecurringOrders;
using Orckestra.Overture.ServiceModel.Requests.RecurringOrders;

namespace Orckestra.Composer.Repositories
{
    public interface IRecurringOrdersRepository
    {
        /// <summary>
        /// Get the list of recurring order line item templates for a customer
        /// </summary>     
        /// <returns></returns>
        Task<ListOfRecurringOrderLineItems> GetRecurringOrderTemplates(string scope, Guid customerId);

        /// <summary>
        /// Get the Overture RecurringOrderProgram
        /// </summary>
        /// <returns></returns>
        Task<RecurringOrderProgram> GetRecurringOrderProgram(string scope, string recurringOrderProgramName);

    }
}
