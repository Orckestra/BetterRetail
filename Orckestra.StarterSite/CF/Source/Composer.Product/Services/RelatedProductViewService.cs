using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Orckestra.Composer.Product.Parameters;
using Orckestra.Composer.Product.Repositories;
using Orckestra.Composer.Product.ViewModels;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Search.Repositories;
using Orckestra.Composer.Utils;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel.Products;

namespace Orckestra.Composer.Product.Services
{
    public class RelatedProductViewService : BaseProductViewService<GetProductIdentifiersParam>, IRelatedProductViewService
    {
        private const string VariantPropertyBagKey = "VariantId";
        public RelatedProductViewService(
            IProductRepository productRepository,
            IRelationshipRepository relationshipRepository,
            IDamProvider damProvider,
            IProductUrlProvider productUrlProvider,
            IViewModelMapper viewModelMapper,
            ILocalizationProvider localizationProvider,
            IInventoryLocationProvider inventoryLocationProvider)

            : base(productRepository, damProvider, productUrlProvider, viewModelMapper, localizationProvider, relationshipRepository, inventoryLocationProvider)
        {          
        }

        protected override async Task<IEnumerable<ProductIdentifier>> GetProductIdentifiersAsync(GetProductIdentifiersParam param)
        {
            if (param == null) { throw new ArgumentNullException("param"); }
            if (param.CultureInfo == null) { throw new ArgumentException("param argument passed to GetRelatedProductIds missing required CultureInfo property");}
            if (param.MerchandiseTypes == null) { throw new ArgumentException("param argument passed to GetRelatedProductIds missing required MerchandiseTypes property");}
            if (param.MerchandiseTypes.Length == 0) { throw new ArgumentException("param.MerchandiseTypes must contain at least one element"); }
            if (param.ProductId == null) { throw new ArgumentException("param argument passed to GetRelatedProductIds missing required ProductId property");}
            if (param.Scope == null) { throw new ArgumentException("param argument passed to GetRelatedProductIds missing required MerchandiseType property");}

            var getProductParam = new GetProductParam
            {
                ProductId = param.ProductId,
                CultureInfo = param.CultureInfo,
                Scope = param.Scope
            };

            var product = await ProductRepository.GetProductAsync(getProductParam).ConfigureAwait(false);
            var relatedProductIdentifiers = ExtractRelatedProductIdentifiers(product, param.MerchandiseTypes, param.MaxItems).ToList();

            if (!relatedProductIdentifiers.Any() && param.FallbackOnSameCategoriesProduct)
            {
                var sameCategoryProductIdentifier = await GetSameCategoryProductIdentifier(param, product.PrimaryParentCategoryId).ConfigureAwait(false);
                if (sameCategoryProductIdentifier.Any())
                {
                    relatedProductIdentifiers = relatedProductIdentifiers.Concat(sameCategoryProductIdentifier).ToList();
                }
            }

            return relatedProductIdentifiers;
        }

        protected virtual async Task<IEnumerable<ProductIdentifier>> GetSameCategoryProductIdentifier(GetProductIdentifiersParam getProductIdentifiersParam, string categoryId)
        {
            Guard.NotNull(getProductIdentifiersParam, nameof(getProductIdentifiersParam));
            Guard.NotNullOrWhiteSpace(categoryId, nameof(categoryId));
            var sameCategoryProductIdentifier = new List<ProductIdentifier>();
            var productInSameCategoryParam = new GetProductsInSameCategoryParam()
            {
                CategoryId = categoryId,
                CultureInfo = getProductIdentifiersParam.CultureInfo,
                Scope = getProductIdentifiersParam.Scope,
                MaxItems = getProductIdentifiersParam.MaxItems,
                SortBy = "score",
                InventoryLocationIds = await InventoryLocationProvider.GetInventoryLocationIdsForSearchAsync(),
                CurrentProductId = getProductIdentifiersParam.ProductId
            };

            var sameCategoryProducts = await RelationshipRepository.GetProductInSameCategoryAsync(productInSameCategoryParam).ConfigureAwait(false);
            if (sameCategoryProducts.Documents.Any())
            {
                sameCategoryProductIdentifier = sameCategoryProducts.Documents.Select(d =>
                {
                    string variantId = null;
                    if (d.PropertyBag.ContainsKey(VariantPropertyBagKey))
                    {
                        variantId = d.PropertyBag[VariantPropertyBagKey] as string;
                    }

                    return new ProductIdentifier
                    {
                        ProductId = d.ProductId,
                        VariantId = variantId

                    };
                }).ToList();
            }
            return sameCategoryProductIdentifier;
        }

        protected virtual IEnumerable<ProductIdentifier> ExtractRelatedProductIdentifiers(Overture.ServiceModel.Products.Product product, string[] merchandiseTypes, int maxItems)
        {
            if (product == null) { return Enumerable.Empty<ProductIdentifier>(); }

            var relatedProducts = product.Relationships
                .Where(r => r.RelationshipType == RelationshipType.Product || 
                       r.RelationshipType == RelationshipType.Variant);
            
            var relatedProductIdentifiers = relatedProducts
                .Where(r => merchandiseTypes.Contains(r.MerchandiseType))
                .OrderBy(r => r.SequenceNumber)
                .Take(maxItems)
                .Select(r => new ProductIdentifier
            {
                // if the product is a variant, it's VariantProductId will be set to the product ID,
                // otherwise it's EntityId is the correct product id
                ProductId = r.VariantProductId ?? r.EntityId,
                VariantId = r.RelationshipType == RelationshipType.Variant ? r.EntityId : null
            });

            return relatedProductIdentifiers;
        }

        public override async Task<RelatedProductsViewModel> GetRelatedProductsAsync(GetRelatedProductsParam param)
        {
            var vm = await base.GetRelatedProductsAsync(param).ConfigureAwait(false);
            vm.Context["ListName"] = "Related Products";

            return vm;
        }
    }
}
