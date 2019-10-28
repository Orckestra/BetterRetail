using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Composite.Data;
using Orckestra.Composer.CompositeC1.DataTypes.SearchAutocomplete;
using Orckestra.Composer.Search.Providers;
using Orckestra.Composer.Services;

namespace Orckestra.Composer.CompositeC1.Providers
{
    public class AutocompleteProvider: IAutocompleteProvider
    {
        private IWebsiteContext WebsiteContext { get; set; }
        public AutocompleteProvider(IWebsiteContext websiteContext)
        {
            WebsiteContext = websiteContext;
        }

        public virtual Task<List<string>> GetSearchSuggestedTerms(CultureInfo cultureInfo, string keyword)
        {
            string keywordLower = keyword.ToLower();
            var websiteId = WebsiteContext.WebsiteId;

            using (DataConnection data = new DataConnection(cultureInfo))
            {
                var searchTerms = data.Get<ISearchTerm>()
                    .Where(suggestion => suggestion.Value.ToLower().Contains(keywordLower) && suggestion.WebsiteId == websiteId)
                    .Select(x => x.Value).ToList();
                return Task.FromResult(searchTerms);
            }
        }
    }
}
