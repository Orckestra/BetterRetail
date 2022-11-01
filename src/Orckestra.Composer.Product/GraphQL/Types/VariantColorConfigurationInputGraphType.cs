using GraphQL.Types;
using Orckestra.Composer.CompositeC1.DataTypes;
using Orckestra.Composer.GraphQL.Types;

namespace Orckestra.Composer.Product.GraphQL.Types
{
    public class VariantColorConfigurationInputGraphType : DataInputObjectGraphType<IVariantColorConfiguration>
    {
        public VariantColorConfigurationInputGraphType() : base()
        {
        }
    }
}
