using Orckestra.Composer.Configuration;
using Orckestra.Composer.Utils;
using Orckestra.Overture.ServiceModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Search.Helpers
{
    public static class ProductHelper
    {
        private static bool _recurringOrdersConfigEnabled = false;

        static ProductHelper()
        {
            _recurringOrdersConfigEnabled = ConfigurationUtil.GetRecurringOrdersConfigEnabled();
        }

        public static bool IsEligibleForRecurring(this PropertyBag bag)
        {        
            var program = GetAttribute(bag, Constants.ProductAttributes.RecurringOrderProgramName);
            return !string.IsNullOrWhiteSpace(program) && _recurringOrdersConfigEnabled;
        }

        private static string GetAttribute(PropertyBag bag, string attributeName)
        {
            return bag.GetValueOrDefault<string>(attributeName);
        }

    }
}
