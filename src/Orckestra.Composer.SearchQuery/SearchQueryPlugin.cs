using Orckestra.Composer.SearchQuery.Context;
using Orckestra.Composer.SearchQuery.Repositories;
using Orckestra.Composer.SearchQuery.Services;
using Orckestra.Overture;

namespace Orckestra.Composer.SearchQuery
{
    public class SearchPlugin : IComposerPlugin
    {
        public void Register(IComposerHost host)
        {
            host.Register<SearchQueryContext, ISearchQueryContext>(ComponentLifestyle.PerRequest);
            host.Register<SearchQueryRepository, ISearchQueryRepository>();
            host.Register<InventoryRepository, IInventoryRepository>();
            host.Register<SearchQueryViewService, ISearchQueryViewService>();

            host.MetadataRegistry.LoadViewModelMetadataInAssemblyOf(typeof(SearchPlugin).Assembly);
        }
    }
}