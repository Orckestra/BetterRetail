using System.Threading.Tasks;
using Orckestra.Composer.Configuration;
using Orckestra.Composer.Product.Parameters;
using Orckestra.Composer.Product.Repositories;
using Orckestra.Composer.Product.Services;
using Orckestra.Composer.Product.ViewModels;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Repositories;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Product.Tests.Services
{
    class RelatedProductViewServiceProxy : RelatedProductViewService
    {
        public RelatedProductViewServiceProxy(IProductRepository productRepository, 
            IRelationshipRepository relationshipRepository, 
            IDamProvider damProvider, IProductUrlProvider 
            productUrlProvider, IViewModelMapper viewModelMapper,
            ILocalizationProvider localizationProvider, 
            IInventoryLocationProvider inventoryLocationProvider,
            IRecurringOrdersSettings recurringOrdersSettings) : base(productRepository, relationshipRepository, damProvider, productUrlProvider, viewModelMapper, localizationProvider, inventoryLocationProvider, recurringOrdersSettings)
        {
        }

        public async Task<ProductWithVariant[]> RetrieverRelatedProductsAsyncProxy(GetRelatedProductsParam param)
        {
            return await RetrieveProductsAsync(param);
        }

        public  RelatedProductsViewModel CreateRelatedProductsViewModelProxy(CreateRelatedProductViewModelParam param)
        {
            return  CreateRelatedProductsViewModel(param);
        }
    }
}
