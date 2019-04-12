using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Orckestra.Composer.Parameters;
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

        /// <summary>
        /// Update the quantity or a recurringOrderLineItem
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<ListOfRecurringOrderLineItems> UpdateRecurringOrderTemplateLineItemQuantityAsync(UpdateRecurringOrderTemplateLineItemQuantityParam param);

        /// <summary>
        /// Removes a recurringOrder template LineItem
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<HttpWebResponse> RemoveRecurringOrderTemplateLineItem(RemoveRecurringOrderTemplateLineItemParam param);

        /// <summary>
        /// Update a recurring order lineItem
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<ListOfRecurringOrderLineItems> UpdateRecurringOrderTemplateLineItemAsync(UpdateRecurringOrderTemplateLineItemParam param);

    }
}
