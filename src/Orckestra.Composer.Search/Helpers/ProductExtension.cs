using Orckestra.Composer.Utils;
using Orckestra.Overture.ServiceModel;

namespace Orckestra.Composer.Search.Helpers
{
    public static class ProductExtension
    {
   
        static ProductExtension()
        {
        }

        public static bool IsRecurringOrderEligible(this PropertyBag bag)
        {        
            var program = GetAttribute(bag, Utils.Constants.ProductAttributes.RecurringOrderProgramName);
            return !string.IsNullOrWhiteSpace(program);
        }

        private static string GetAttribute(PropertyBag bag, string attributeName)
        {
            return bag.GetValueOrDefault<string>(attributeName);
        }
    }
}
