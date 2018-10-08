using ServiceStack.Text;
using System.Collections.Generic;

namespace Orckestra.Composer.SearchQuery.Extensions
{
    public static class PropertyBagExtensions
    {
        public static TEntity GetOrDeserializePropertyBagEntity<TEntity>(this IDictionary<string, object> propertyBag, string propertyName)
        {
            if (propertyBag == null || !propertyBag.ContainsKey(propertyName) || propertyBag[propertyName] == null)
            {
                return default(TEntity);
            }

            var valueObject = propertyBag[propertyName];
            TEntity entity;

            //This means that we have a serialized object.
            if (valueObject is string && typeof(TEntity) != typeof(string))
            {
                entity = JsonSerializer.DeserializeFromString<TEntity>(valueObject as string);
            }
            else
            {
                entity = (TEntity)valueObject;
            }

            return entity;
        }
    }
}
