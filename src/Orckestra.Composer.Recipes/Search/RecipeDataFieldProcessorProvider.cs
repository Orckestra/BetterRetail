using Composite.Data;
using Composite.Search;
using Composite.Search.Crawling;
using Orckestra.Composer.Recipes.DataTypes;
using System.Linq;
using System.Reflection;

namespace Orckestra.Composer.Recipes.Search
{
    internal class RecipeDataFieldProcessorProvider : IDataFieldProcessorProvider
    {
        public IDataFieldProcessor GetDataFieldProcessor(PropertyInfo dataTypeProperty)
        {
            if (dataTypeProperty.DeclaringType.FullName == "Orckestra.Composer.Recipes.DataTypes.IRecipe")
            {
                switch (dataTypeProperty.Name)
                {
                    case "DietType":
                        return new DietTypeDataFieldProcessor();
                }
            }
            return null;
        }

        internal class DietTypeDataFieldProcessor : DefaultDataFieldProcessor
        {
            public override string[] GetFacetValues(object value)
            {
                var data = DataFacade.GetData<IDietType>().ToLookup(_=> _.Id.ToString());
                var str = (string)value;
                var strList = str.Split(',').Select(i => data[i].FirstOrDefault()?.Title).Where(i=> i != null).ToArray();
                return strList;
            }

            public override DocumentFieldFacet GetDocumentFieldFacet(PropertyInfo propertyInfo)
            {
                var result = base.GetDocumentFieldFacet(propertyInfo);

                result.FacetType = FacetType.MultipleValues;

                return result;
            }

        }

    }
}
