using Orckestra.Composer.ContentSearch.Services;
using Orckestra.Composer.Dependency;

namespace Orckestra.Composer.ContentSearch
{
    public class ContentSearchPlugin : IComposerPlugin
    {
        public void Register(IComposerHost host)
        {
            host.Register<ContentSearchViewService, IContentSearchViewService>(ComponentLifestyle.Transient);

            host.RegisterApiControllers(typeof(ContentSearchPlugin).Assembly);
        }
    }
}