using Orckestra.Composer.Configuration;
using Orckestra.Composer.Utils;
using Orckestra.Overture.ServiceModel;
using Orckestra.Overture.ServiceModel.Products;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Search.Helpers
{
    public static class ProductExtension
    {
        private static bool _recurringOrdersConfigEnabled = false;

        static ProductExtension()
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
