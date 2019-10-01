using Orckestra.Composer.Configuration;
using Orckestra.Overture.ServiceModel.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Orckestra.Composer.Utils;
using Orckestra.Composer.Cart.ViewModels;

namespace Orckestra.Composer.Cart.Helper
{
    public static class RecurringOrderCartHelper
    {
  
        static RecurringOrderCartHelper()
        {
           
        }

        public static bool IsCartContainsRecurringOrderItems(Overture.ServiceModel.Orders.Cart cart)
        {

            if (cart != null)
            {
                foreach (var shipment in cart.Shipments ?? Enumerable.Empty<Shipment>())
                {
                    foreach (var lineitem in shipment.LineItems ?? Enumerable.Empty<LineItem>())
                    {
                        if (IsRecurringOrderLineItemValid(lineitem))
                            return true;
                    }
                }
            }

            return false;
        }

        public static bool IsRecurringOrderLineItemValid(LineItem recurringOrderLineitem)
        {
            return (!string.IsNullOrEmpty(recurringOrderLineitem.RecurringOrderFrequencyName)
                        && !string.IsNullOrEmpty(recurringOrderLineitem.RecurringOrderProgramName));
        }
        public static bool IsRecurringOrderLineItemValid(LineItemDetailViewModel lineitem)
        {
            return (!string.IsNullOrEmpty(lineitem.RecurringOrderFrequencyName)
                        && !string.IsNullOrEmpty(lineitem.RecurringOrderProgramName));
        }

        public static bool IsCartContainsRecurringOrderItems(List<LineItemDetailViewModel> lineitems)
        {
 
            if (lineitems != null)
            {
                foreach (var lineitem in lineitems ?? Enumerable.Empty<LineItemDetailViewModel>())
                {
                    if (IsRecurringOrderLineItemValid(lineitem))
                        return true;
                }
            }

            return false;
        }
    }
}
