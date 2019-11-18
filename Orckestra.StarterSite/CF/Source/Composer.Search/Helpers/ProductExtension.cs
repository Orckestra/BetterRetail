using Orckestra.Composer.Utils;
using Orckestra.Overture.ServiceModel;

namespace Orckestra.Composer.Search.Helpers
{
    public static class ProductExtension
    {
   
        static ProductExtension()
        {
        }

        public static bool IsEligibleForRecurring(this PropertyBag bag)
        {        
            var program = GetAttribute(bag, Constants.ProductAttributes.RecurringOrderProgramName);
            return !string.IsNullOrWhiteSpace(program);
        }

        private static string GetAttribute(PropertyBag bag, string attributeName)
        {
            return bag.GetValueOrDefault<string>(attributeName);
        }
    }
}
