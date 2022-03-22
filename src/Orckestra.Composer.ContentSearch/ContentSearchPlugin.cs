using Composite.Data.DynamicTypes;
using Composite.Search.Crawling;
using Orckestra.Composer.ContentSearch.DataTypes;
using Orckestra.Composer.ContentSearch.Search;
using Orckestra.Composer.ContentSearch.Services;
using Orckestra.Overture;
using System;

namespace Orckestra.Composer.ContentSearch
{
    public class ContentSearchPlugin : IComposerPlugin
    {
        public void Register(IComposerHost host)
        {
            host.Register<ContentSearchViewService, IContentSearchViewService>(ComponentLifestyle.Transient);
            host.Register<MediaSearchDocumentBuilderExtension, ISearchDocumentBuilderExtension>(ComponentLifestyle.Singleton);

            host.MetadataRegistry.LoadViewModelMetadataInAssemblyOf(typeof(ContentSearchPlugin).Assembly);

            host.RegisterApiControllers(typeof(ContentSearchPlugin).Assembly);

            

            host.Initialized += HostOnInitialized;
        }

        private void HostOnInitialized(object sender, EventArgs eventArgs)
        {
            IComposerHost host = (IComposerHost)sender;

            DynamicTypeManager.EnsureCreateStore(typeof(IContentTab));
            DynamicTypeManager.EnsureCreateStore(typeof(ISortOption));
        }
    }
}