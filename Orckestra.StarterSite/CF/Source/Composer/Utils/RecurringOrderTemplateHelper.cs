using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel.Orders;
using Orckestra.Overture.ServiceModel.RecurringOrders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Helper
{
    public static class RecurringOrderTemplateHelper
    {
        public static bool IsRecurringOrderLineItemValid(RecurringOrderTemplateLineItemViewModel recurringOrderLineitem)
        {
            return (!string.IsNullOrEmpty(recurringOrderLineitem.RecurringOrderFrequencyName)
                        && !string.IsNullOrEmpty(recurringOrderLineitem.RecurringOrderProgramName));
        }

        public static bool IsLineItemAndRecurringTemplateLineItemSameProduct(LineItem lineItem, RecurringOrderLineItem recurringOrderLineItem)
        {
            return IsLineItemAndRecurringTemplateLineItemSameProduct(lineItem.ProductId, lineItem.VariantId, recurringOrderLineItem.ProductId, recurringOrderLineItem.VariantId);
        }
        public static bool IsLineItemAndRecurringTemplateLineItemSameProduct(string lineItemProducId, string lineItemVariantId,
                                                                            string recurringOrderLineItemProductId, string recurringOrderLineItemVariantId)
        {
            //True if same product
            return (string.Equals(lineItemProducId, recurringOrderLineItemProductId, StringComparison.OrdinalIgnoreCase)
                && string.Equals(lineItemVariantId, recurringOrderLineItemVariantId, StringComparison.OrdinalIgnoreCase));
        }
    }
}
