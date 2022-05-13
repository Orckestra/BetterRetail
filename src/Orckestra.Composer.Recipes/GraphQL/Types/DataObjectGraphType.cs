using Composite.Data;
using GraphQL;
using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.ExceptionServices;
using Orckestra.Composer.Recipes.GraphQL.Extensions;

namespace Orckestra.Composer.Recipes.GraphQL.Types
{
    public class DataObjectGraphType<TSourceType> : AutoRegisteringObjectGraphType<TSourceType>
        where TSourceType : class, IData
    {
        public DataObjectGraphType(params Expression<Func<TSourceType, object>>[] excludedProperties) : base(
            excludedProperties)
        {
            Name = $"{typeof(TSourceType).DataQLName()}";
        }

        public void FieldList<TChildType, TGraphType>(
            Func<TChildType, TSourceType, bool> childPredicate)
            where TChildType : class, IData
            where TGraphType : IGraphType
        {
            Field<ListGraphType<TGraphType>>(typeof(TChildType).DataQLNameList(), resolve: context =>
            {
                using (var connection = new DataConnection(PublicationScope.Unpublished))
                {
                    return connection.Get<TChildType>()
                        .Where(child => childPredicate(child, context.Source));
                }
            });

        }
    }
}
