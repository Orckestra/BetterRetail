using System;
using Orckestra.Overture.ServiceModel.Metadata;

namespace Orckestra.Composer.Product.Extensions
{
    internal static class ProductPropertyDefinitionExtensions
    {
        public static bool IsIncluded(this ProductPropertyDefinition property)
        {
            if (property == null) { throw new ArgumentNullException(nameof(property)); }

            return !SpecificationConfiguration.ExcludedSpecificationProperty.Contains(property.PropertyName);
        }
    }
}