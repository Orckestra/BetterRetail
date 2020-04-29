using System;
using Orckestra.Overture.ServiceModel.Metadata;

namespace Orckestra.Composer.Product.Extensions
{
    internal static class ProductPropertyDefinitionGroupExtensions
    {
        public static bool IsIncluded(this ProductPropertyDefinitionGroup propertyGroup)
        {
            if (propertyGroup == null) { throw new ArgumentNullException(nameof(propertyGroup)); }

            return !SpecificationConfiguration.ExcludedSpecificationPropertyGroups.Contains(propertyGroup.Name);
        }
    }
}