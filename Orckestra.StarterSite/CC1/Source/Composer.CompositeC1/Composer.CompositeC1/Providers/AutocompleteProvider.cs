using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Composite.Data;
using Orckestra.Composer.CompositeC1.DataTypes.SearchAutocomplete;
using Orckestra.Composer.Search.Providers;

namespace Orckestra.Composer.CompositeC1.Providers
{
    public class AutocompleteProvider: IAutocompleteProvider
    {
        public virtual Task<List<string>> GetSearchSuggestedTerms(CultureInfo cultureInfo, string keyword)
        {
            string keywordLower = keyword.ToLower();

            using (DataConnection data = new DataConnection(cultureInfo))
            {
                var searchTerms = data.Get<ISearchTerm>().Select(x => x.Value).Where(suggestion => suggestion.ToLower().Contains(keywordLower)).ToList();
                return Task.FromResult(searchTerms);
            }
        }
    }
}
