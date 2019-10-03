using Orckestra.Composer.Product.Factory;
using Orckestra.Composer.Product.Providers;
using Orckestra.Composer.Product.Repositories;
using Orckestra.Composer.Product.Services;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Dam;
using Orckestra.Composer.Services;

namespace Orckestra.Composer.Product
{
    public class ProductPlugin : IComposerPlugin
    {
        public void Register(IComposerHost host)
        {
            host.Register<ProductViewService, IProductViewService>();
            host.Register<RelationshipRepository, IRelationshipRepository>();
            host.Register<CategoryViewService, ICategoryViewService>();
            host.Register<ConventionBasedDamProvider, IDamProvider>();
            host.Register<ProductBreadcrumbService, IProductBreadcrumbService>();
            host.Register<ProductSpecificationsViewService, IProductSpecificationsViewService>();
            host.Register<ProductViewModelFactory, IProductViewModelFactory>();
            host.Register<RelatedProductViewService, IRelatedProductViewService>();
            host.Register<InventoryViewService, IInventoryViewService>();
            host.Register<ConfigurationInventoryLocationProvider, IInventoryLocationProvider>();

            host.MetadataRegistry.LoadViewModelMetadataInAssemblyOf(typeof(ProductPlugin).Assembly);

            host.RegisterApiControllers(typeof(ProductPlugin).Assembly);
        }
    }
}
