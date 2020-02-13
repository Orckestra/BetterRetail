using Orckestra.Composer.ContentSearch.Parameters;
using Orckestra.Composer.ContentSearch.ViewModels;
using Orckestra.Search.WebsiteSearch;

namespace Orckestra.Composer.ContentSearch.Services
{
    public interface IContentSearchViewService
    {
        ContentSearchViewModel GetContentSearchViewModel(GetContentSearchParameter param);

        SearchResultsEntryViewModel GetSearchResultsEntryViewModel(SearchResultEntry entry);
    }
}
