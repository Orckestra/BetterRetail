using Composite.Data;
using GraphQL;
using GraphQL.Types;
using System;
using System.Linq;

namespace Orckestra.Composer.Recipes.GraphQL.Extensions
{
    public static class DataGraphQLExtensions
    {
        /// <summary>
        /// Adds a field for a specific IData entity. 
        /// </summary>
        /// <typeparam name="TSourceType"></typeparam>
        /// <param name="objectGraphType"></param>
        /// <param name="idSelector"></param>
        public static void Field<TSourceType>(this ObjectGraphType<object> objectGraphType,
            Func<TSourceType, Guid> idSelector)
            where TSourceType : class, IData
        {
            Type type = typeof(TSourceType);
            objectGraphType.Field<AutoRegisteringObjectGraphType<TSourceType>>(
                GetFieldName<TSourceType>(),
                arguments: new QueryArguments(new QueryArgument<GuidGraphType>
                { Name = "id" }),
                resolve: (context) =>
                {
                    using (var connection = new DataConnection(PublicationScope.Unpublished))
                    {
                        var id = context.GetArgument<Guid>("id");
                        return connection.Get<TSourceType>()
                            .FirstOrDefault(d => idSelector(d) == id);
                    }
                });
        }

        /// <summary>
        /// Adds a field with a list of specified IData
        /// </summary>
        /// <typeparam name="TSourceType"></typeparam>
        /// <param name="objectGraphType"></param>
        public static void FieldList<TSourceType>(this ObjectGraphType<object> objectGraphType)
            where TSourceType : class, IData
        {

            objectGraphType.Field<ListGraphType<AutoRegisteringObjectGraphType<TSourceType>>>(
                GetFieldName<TSourceType>(true),
            arguments: new QueryArguments(new QueryArgument<IntGraphType>
            { Name = "startingIndex", DefaultValue = 0 },
            new QueryArgument<IntGraphType>
            { Name = "maximumItems", DefaultValue = 100 }),
            resolve: (context) =>
            {
                using (var connection = new DataConnection(PublicationScope.Unpublished))
                {
                    var start = context.GetArgument<int>("startingIndex");
                    var count = context.GetArgument<int>("maximumItems");
                    return connection.Get<TSourceType>().Skip(start).Take(count);
                }
            });
        }

        private static string GetFieldName<TSourceType>(bool multiple = false)
        {
            var name = typeof(TSourceType).Name;

            if (name.StartsWith("I") && name.Length > 1 && Char.IsUpper(name, 1))
            {
                name = name.Substring(1);
            }

            return multiple ? $"{name}Items" : name;
        }
    }
}
