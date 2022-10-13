using Composite.Data;
using GraphQL;
using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.ExceptionServices;
using Orckestra.Composer.Product.GraphQL.Extensions;

namespace Orckestra.Composer.Product.GraphQL.Types
{
    public class DataInputObjectGraphType<TSourceType> : AutoRegisteringInputObjectGraphType<TSourceType> where TSourceType : class, IData
    {
        public DataInputObjectGraphType(params Expression<Func<TSourceType, object>>[] excludedProperties) : base(excludedProperties)
        {
            Name = $"{typeof(TSourceType).DataQLName()}Input";
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


            var obj = new DataInput<TSourceType>()
            {
                PropertyBag = new Dictionary<string, object>()
            };
            try
            {
                obj.Data = DataFacade.BuildNew<TSourceType>();
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
                    propertyInfo.SetValue(obj.Data, value, null); //issue: this works even if propertyInfo is ValueType and value is null
                }
                else
                {
                    obj.PropertyBag[item.Key] = item.Value;
                }
            }

            return obj;
        }

    }
}
