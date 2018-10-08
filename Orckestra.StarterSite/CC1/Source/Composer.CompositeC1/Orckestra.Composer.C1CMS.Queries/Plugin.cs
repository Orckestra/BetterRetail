using Orckestra.Composer.C1CMS.Queries.Providers;
using Orckestra.Composer.SearchQuery.Providers;

namespace Orckestra.Composer.C1CMS.Queries
{
    public class Plugin : IComposerPlugin
    {
        public void Register(IComposerHost host)
        {
            host.Register<SearchQueryUrlProvider, ISearchQueryUrlProvider>();

            host.RegisterApiControllers(typeof(Plugin).Assembly);
            host.RegisterControllers(typeof(Plugin).Assembly);
           
        }
    }
}
