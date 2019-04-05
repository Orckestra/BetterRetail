using Orckestra.Composer.Configuration;
using Orckestra.Overture.ServiceModel.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Orckestra.Composer.Cart.ViewModels;
using Orckestra.Composer.Utils;

namespace Orckestra.Composer.Cart.Helper
{
    public static class RecurringOrderCartHelper
    {
        private static bool _recurringOrdersConfigEnabled = false;

        static RecurringOrderCartHelper()
        {
            _recurringOrdersConfigEnabled = ConfigurationUtil.GetRecurringOrdersConfigEnabled();
        }

        public static bool IsRecurringOrdersEnabled()
        {
            return _recurringOrdersConfigEnabled;
        }
        public static bool IsCartContainsRecurringOrderItems(ProcessedCart cart)
        {
            if (!_recurringOrdersConfigEnabled)
                return false;

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
            if (!_recurringOrdersConfigEnabled)
                return false;

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

        public static Guid ConvertStringToGuid(string str)
        {
            if (String.IsNullOrWhiteSpace(str))
            {
                throw new ArgumentException("Guid value cannot be null, empty or whitespaces");
            }
            return Guid.Parse(str);
        }

    }
}
