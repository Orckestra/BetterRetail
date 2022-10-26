using System;
using System.Collections.Generic;
using System.Linq;
using Composite.Core.Linq;
using Composite.Data;
using GraphQL;
using GraphQL.Types;
using Orckestra.Composer.CompositeC1.DataTypes;
using Orckestra.Composer.Product.GraphQL.Types;
using Orckestra.Composer.GraphQL.Extensions;
using Orckestra.Composer.GraphQL.Types;

namespace Orckestra.Composer.Product.GraphQL
{
    public class VariantColorMutation : ObjectGraphType
    {
        public VariantColorMutation()
        {
            Name = "VariantColorMutation";

            Field<StringGraphType>(
                "updateVariantColors",
                arguments: new QueryArguments(
                    new QueryArgument<ListGraphType<VariantColorConfigurationInputGraphType>> { Name = typeof(IVariantColorConfiguration).DataQLNameList() }

                ),
                resolve: context =>
                {
                    using (var connection = new DataConnection(PublicationScope.Unpublished))
                    {
                        var configurationModifications = context.GetArgument<List<DataInput<IVariantColorConfiguration>>>(typeof(IVariantColorConfiguration).DataQLNameList());
                        var oldItems = connection.Get<IVariantColorConfiguration>().ToList();

                        var itemsToUpdate = oldItems.Where(d => configurationModifications.Any(x => x.Data.LookupValue == d.LookupValue)).ToList();
                        var itemsToRemove = oldItems.Where(d =>
                        configurationModifications.Any(x => x.Data.LookupValue == d.LookupValue
                            && String.IsNullOrEmpty(x.Data.Color) && String.IsNullOrEmpty(x.Data.Image))).ToList();
                        var itemsToAdd = configurationModifications.Where(d => !oldItems.Any(x => x.LookupValue == d.Data.LookupValue)).ToList();

                        foreach (var item in itemsToUpdate)
                        {
                            if (!itemsToRemove.Any(i => i.LookupValue == item.LookupValue))
                            {
                                var inputItem = configurationModifications.First(d => item.LookupValue == d.Data.LookupValue);
                                inputItem.Data.ProjectedCopyTo(item);
                                connection.Update(item);
                            }

                        }

                        foreach (var inputItem in itemsToAdd)
                        {
                            var item = connection.Add(inputItem.Data);
                        }

                        connection.Delete(itemsToRemove);

                        return "Ok";
                    }
                });
        }
    }
}