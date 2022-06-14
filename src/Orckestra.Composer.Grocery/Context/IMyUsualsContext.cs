using Orckestra.Composer.Search.ViewModels;

namespace Orckestra.Composer.Grocery.Context
{
   public interface IMyUsualsContext
    {
        SearchViewModel EmptyProductResultsViewModel { get; }
        SearchViewModel ProductResultsViewModel { get; }
        string[] ListMyUsualsSkus { get; }
        string SearchQuery { get; }
    }
}
