using Composite.Data;
using GraphQL;
using GraphQL.Types;
using System;
using System.Linq;
using Orckestra.Composer.Recipes.GraphQL.Types;

namespace Orckestra.Composer.Recipes.GraphQL.Extensions
{
    public static class DataGraphQLExtensions
    {
        /// <summary>
        /// Adds a field for a specific IData entity. 
        /// </summary>
        /// <typeparam name="TSourceType"></typeparam>
        /// <param name="objectGraphType"></param>
        /// <param name="getId"></param>
        public static void Field<TSourceType>(this ObjectGraphType<object> objectGraphType,
            Func<TSourceType, Guid> getId)
            where TSourceType : class, IData
        {
            objectGraphType.Field<TSourceType, DataObjectGraphType<TSourceType>>(getId);
        }

        public static void Field<TSourceType, TGraphType>(this ObjectGraphType<object> objectGraphType,
            Func<TSourceType, Guid> idSelector)
            where TSourceType : class, IData
            where TGraphType : ObjectGraphType<TSourceType>
        {
            objectGraphType.Field<TGraphType>(
                typeof(TSourceType).DataQLName(),
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
            objectGraphType.FieldList<TSourceType, DataObjectGraphType<TSourceType>>();
        }

        /// <summary>
        /// Adds a field with a list of specified IData
        /// </summary>
        /// <typeparam name="TSourceType"></typeparam>
        /// <typeparam name="TGraphType"></typeparam>
        /// <param name="objectGraphType"></param>
        public static void FieldList<TSourceType, TGraphType>(this ObjectGraphType<object> objectGraphType)
            where TSourceType : class, IData
            where TGraphType: ObjectGraphType<TSourceType>
        {
            objectGraphType.Field<ListGraphType<TGraphType>>(
                typeof(TSourceType).DataQLNameList(),
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

        public static string DataQLName(this Type type)
        {
            var name = type.GraphQLName();

            if (name.StartsWith("I") && name.Length > 1 && char.IsUpper(name, 1))
            {
                name = name.Substring(1);
            }

            return name.ToCamelCase();
        }

        public static string DataQLNameList(this Type type)
        {
            return $"{type.DataQLName()}Items";
        }

        public static string DataQLId(this Type type)
        {
            return $"{type.DataQLName()}Id";
        }
    }
}