using Composite.Data;
using Composite.Data.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Orckestra.Composer.ContentSearch.Functions
{
    public class WebsiteSearch
    {
        //Exclude RefApp internal PageTypes
          public static List<string> PageTypesToExclude = new List<string>
        {
            "916a50c8-6044-46d0-8c47-13c88f8b0d25", //Account Page
            "9a89191f-fc93-47ee-8b9b-022611c37fa6", //Folder
            "f64fe76f-bb5a-4b6d-9499-e81762f25db6", //Product Page
            "a014b691-fc87-42c1-b664-84e0b951e4ed" //Search Page
        };

        public static (string Name, string Id)[] GetSearchablePageTypesOptions()
        {
            var pageTypes = DataFacade.GetData<IPageType>().Where(p => p.Available && !PageTypesToExclude.Contains(p.Id.ToString()));

            return pageTypes.AsEnumerable().Select(pageType => (pageType.Name, pageType.Id.ToString())).ToArray();
        }
    }
}
