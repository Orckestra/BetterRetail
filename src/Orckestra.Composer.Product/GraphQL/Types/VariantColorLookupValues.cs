using Composite.Data;
using GraphQL.Types;
using Orckestra.Composer.CompositeC1.DataTypes;
using System;

namespace Orckestra.Composer.Product.GraphQL.Types
{
    public class VariantColorLookupValues 
    {
        public string LookupValue { get; set; }
        public string Color { get; set; }
        public string Image { get; set; }
    }
}
