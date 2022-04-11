using Composite.Data;
using GraphQL;
using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;

namespace Orckestra.Composer.Recipes.GraphQL.Types
{
    public class DataInputObjectGraphType<TSourceType> : AutoRegisteringInputObjectGraphType<TSourceType> where TSourceType : class, IData
    {
        public DataInputObjectGraphType()
        {
            Name = $"{typeof(TSourceType).GraphQLName()}Input";
            Field<GuidGraphType>("Id");
        }

        public override object ParseDictionary(IDictionary<string, object> value)
        {
            if (value == null)
                return null;

            // for InputObjectGraphType just return the dictionary
            if (typeof(TSourceType) == typeof(object))
                return value;

            // for InputObjectGraphType<TSourceType>, convert to TSourceType via ToObject.
            return ToObject(value, typeof(TSourceType), this);
        }

        protected override IEnumerable<PropertyInfo> GetRegisteredProperties()
        {
            return base.GetRegisteredProperties().Where(d => d.Name != "Id");
        }

        public static object ToObject(IDictionary<string, object> source, Type type, IGraphType mappedType = null)
        {
            // Given Field(x => x.FName).Name("FirstName") and key == "FirstName" returns "FName"
            string GetPropertyName(string key, out FieldType field)
            {
                var complexType = mappedType?.GetNamedType() as IComplexGraphType;

                // type may not contain mapping information
                field = complexType?.GetField(key);
                return field?.GetMetadata("ORIGINAL_EXPRESSION_PROPERTY_NAME", key) ?? key;
            }

            if (source == null)
                throw new ArgumentNullException(nameof(source));


            object obj;
            try
            {
                obj = DataFacade.BuildNew<TSourceType>();
                //NOTE: may be more simple
                //Type generatedType = DataTypeTypesManager.GetDataTypeEmptyClass(typeof(T));
                //IData data = (IData)Activator.CreateInstance(generatedType, new object[] { });
            }
            catch (TargetInvocationException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                return ""; // never executed, necessary only for intellisense
            }

            foreach (var item in source)
            {

                string propertyName = GetPropertyName(item.Key, out var field);
                PropertyInfo propertyInfo = null;

                try
                {
                    propertyInfo = type.GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                }
                catch (AmbiguousMatchException)
                {
                    propertyInfo = type.GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                }

                if (propertyInfo != null && propertyInfo.CanWrite)
                {
                    object value = ObjectExtensions.GetPropertyValue(item.Value, propertyInfo.PropertyType, field?.ResolvedType);
                    propertyInfo.SetValue(obj, value, null); //issue: this works even if propertyInfo is ValueType and value is null
                }
            }

            return obj;
        }

    }
}
