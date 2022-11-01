using GraphQL.Types;
using Orckestra.Composer.Product.GraphQL.Interfaces;
using System;

namespace Orckestra.Composer.Product.GraphQL
{
    public class VariantColorSchema : Schema, IVariantColorSchema
    {
        public VariantColorSchema(IServiceProvider provider)
            : base(provider)
        {
            Query = (VariantColorQuery)provider.GetService(typeof(VariantColorQuery));
            Mutation = (VariantColorMutation)provider.GetService(typeof(VariantColorMutation));
        }

    }
}
