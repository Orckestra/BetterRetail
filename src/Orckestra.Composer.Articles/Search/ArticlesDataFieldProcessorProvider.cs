using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Composite.Search;
using Composite.Search.Crawling;
using Composite.Search.Crawling.DataFieldProcessors;

namespace Orckestra.Composer.Articles.Search
{
    internal class ArticlesDataFieldProcessorProvider : IDataFieldProcessorProvider
    {
        public IDataFieldProcessor GetDataFieldProcessor(PropertyInfo dataTypeProperty)
        {
            if (dataTypeProperty.DeclaringType.FullName == "Orckestra.Composer.Articles.DataTypes.IArticle")
            {
                switch (dataTypeProperty.Name)
                {
                    case "Tags":
                        return new TagsDataFieldProcessor();
                    case "Date":
                        return new DateDataFieldProcessor();
                }
            }
            return null;
        }

        internal class TagsDataFieldProcessor : DefaultDataFieldProcessor
        {
            public override string[] GetFacetValues(object value)
            {
                var str = (string)value;
                var strList = str.Split(',').Select(i => i.Trim()).ToArray();
                return strList;
            }

            public override DocumentFieldFacet GetDocumentFieldFacet(PropertyInfo propertyInfo)
            {
                var result = base.GetDocumentFieldFacet(propertyInfo);

                result.FacetType = FacetType.MultipleValues;

                return result;
            }

        }

        internal class DateDataFieldProcessor : DateTimeDataFieldProcessor
        {
            public override string GetDocumentFieldName(PropertyInfo pi)
            {
                return DocumentFieldNames.LastUpdated;
            }
        }

    }
}
