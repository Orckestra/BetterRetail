using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Orckestra.Composer.Configuration;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Product.Parameters;
using Orckestra.Composer.Product.Repositories;
using Orckestra.Composer.Product.ViewModels;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Dam;
using Orckestra.Composer.Providers.Localization;
using Orckestra.Composer.Repositories;
using Orckestra.Composer.Search.Helpers;
using Orckestra.Composer.Search.Repositories;
using Orckestra.Composer.Utils;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel.Products;

namespace Orckestra.Composer.Product.Services
{
	public abstract class BaseProductViewService<TParam>
	{
	    protected readonly IProductRepository ProductRepository;
	    protected readonly IRelationshipRepository RelationshipRepository;
        protected readonly IInventoryLocationProvider InventoryLocationProvider;
        private readonly IViewModelMapper _viewModelMapper;
	    private readonly IDamProvider _damProvider;
	    private readonly ILocalizationProvider _localizationProvider;
	    private readonly IProductUrlProvider _productUrlProvider;
        protected IRecurringOrdersSettings RecurringOrdersSettings { get; private set; }

        protected BaseProductViewService(
            IProductRepository productRepository, 
            IDamProvider damProvider, 
            IProductUrlProvider productUrlProvider, 
            IViewModelMapper viewModelMapper, 
            ILocalizationProvider localizationProvider,
            IRelationshipRepository relationshipRepository,
            IInventoryLocationProvider inventoryLocationProvider,
            IRecurringOrdersSettings recurringOrdersSettings)
	    {
            if (productRepository == null) { throw new ArgumentNullException("productRepository"); }
            if (damProvider == null) { throw new ArgumentNullException("damProvider"); }
            if (productUrlProvider == null) { throw new ArgumentNullException("productUrlProvider"); }
            if (viewModelMapper == null) { throw new ArgumentNullException("viewModelMapper"); }
            if (localizationProvider == null) { throw new ArgumentNullException("localizationProvider"); }
            if (relationshipRepository == null) { throw new ArgumentNullException("relationshipRepository"); }
            if (inventoryLocationProvider == null) { throw new ArgumentNullException("inventoryLocationProvider"); }

	        ProductRepository = productRepository;
	        _viewModelMapper = viewModelMapper;
	        _damProvider = damProvider;
	        _localizationProvider = localizationProvider;
	        _productUrlProvider = productUrlProvider;
            RelationshipRepository = relationshipRepository;
	        InventoryLocationProvider = inventoryLocationProvider;
            RecurringOrdersSettings = recurringOrdersSettings;

        }

        protected abstract Task<IEnumerable<ProductIdentifier>> GetProductIdentifiersAsync(TParam param);

	    /// <summary>
		/// A method to retrieve a list of Product and Variant Ids for products. This method can be used in collaboration with
		/// <seealso cref="GetRelatedProductsAsync"/> to retrieve the full set of products.
		/// </summary>
		/// <param name="param"></param>
		/// <returns>
		/// This method returns a <see cref="RelatedProductsViewModel"/>, however none of its properties will be set.
		/// The returned object is really just a container for the context to expose the produt identifiers to the
		/// JsonContext on the front end.
		/// </returns>
		public async virtual Task<RelatedProductsViewModel> GetProductIdsAsync(TParam param)
		{
			// the identifiers will only be used by the front end 
			var vm = new RelatedProductsViewModel();

            vm.Context["ProductIdentifiers"] = await GetProductIdentifiersAsync(param).ConfigureAwait(false);
			
            return vm;
		}

        /// <summary>
        /// This method retrieves all the products for a product given in the <see cref="GetRelatedProductsParam"/>
        /// then converts them to to a <see cref="RelatedProductsViewModel"/>. The change the behaviour of this method, 
        /// developers should override <seealso cref="RetrieveProductsAsync"/> or <seealso cref="CreateRelatedProductsViewModel(Orckestra.Composer.Product.Parameters.CreateRelatedProductViewModelParam)"/>.
        /// This method makes a number of asynchronous calls, so in cases where only the product IDs are desired,
        /// <seealso cref="GetProductIdsAsync"/> should be used.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async virtual Task<RelatedProductsViewModel> GetRelatedProductsAsync(GetRelatedProductsParam param)
        {
            if (param == null) { throw new ArgumentNullException("param"); }

            if (!param.ProductIds.Any())
            {
                return new RelatedProductsViewModel();
            }

            var products = await RetrieveProductsAsync(param).ConfigureAwait(false);

            // make a single request to get all product prices at once, instead of making a request for each product
            var productIds = param.ProductIds.Select(p => p.ProductId).ToList();
            var prices = ProductRepository.CalculatePricesAsync(productIds, param.Scope);
            var images = GetImagesAsync(products);
            await Task.WhenAll(prices, images).ConfigureAwait(false);

            var createVmParam = new CreateRelatedProductViewModelParam
            {
                BaseUrl = param.BaseUrl,
                CultureInfo = param.CultureInfo,
                ProductsWithVariant = products,
                Scope = param.Scope,
                Prices = prices.Result,
                Images = images.Result
            };

            var vm = CreateRelatedProductsViewModel(createVmParam);
            return vm;
        }

        /// <summary>
        /// Creates a <see cref="RelatedProductsViewModel"/> which is just a container for a list of <see cref="RelatedProductViewModel"/>
        /// </summary>
        /// <returns></returns>
        protected virtual RelatedProductsViewModel CreateRelatedProductsViewModel(CreateRelatedProductViewModelParam param)
        {
            var relatedProductsViewModel = new RelatedProductsViewModel();
            
            //iterate over the requested products
            foreach (var productVariant in param.ProductsWithVariant)
            {
                // call the method that actually does the mapping for an individual product
                var productVm =  CreateRelatedProductsViewModel(param.BaseUrl, param.CultureInfo, productVariant, param.Prices, param.Images);
                relatedProductsViewModel.Products.Add(productVm);
            }

            return relatedProductsViewModel;
        }

        /// <summary>
        /// Gets a list of products with the defined variant specified.
        /// </summary>
        /// <param name="param"></param>
        /// <returns>
        /// An array of <see cref="ProductWithVariant"/>.
        /// This class is used instead of the <see cref="Overture.ServiceModel.Products.Product"/> class
        /// because specific variants are presented to the user when specified in Orchestrator.
        /// </returns>
        protected virtual async Task<ProductWithVariant[]> RetrieveProductsAsync(GetRelatedProductsParam param)
        {
            if (param == null)
            {
                throw new ArgumentNullException("param");
            }

            var productRequestTasks = new List<Task<ProductWithVariant>>();
            
            foreach (var productIdentifier in param.ProductIds)
            {
                // it's helpful to tie the VariantId to the product at this point
                var task = GetProductWithVariantAsync(productIdentifier.ProductId, productIdentifier.VariantId,
                    param.Scope, param.CultureInfo);
                productRequestTasks.Add(task);
            }

            var products = await Task.WhenAll(productRequestTasks).ConfigureAwait(false);

            var results = products.Where(p => p != null).ToArray();

            return results;
        }

        protected virtual async Task<List<ProductMainImage>> GetImagesAsync(IEnumerable<ProductWithVariant> products)
        {
            var imageRequests =
                products.Select(
                    identifier => new ProductImageRequest
                    {
                        ProductId = identifier.Product.Id,
                        Variant = identifier.Variant == null ? VariantKey.Empty : new VariantKey
                        {
                            Id = identifier.Variant.Id,
                            KeyVariantAttributeValues = identifier.Variant.PropertyBag
                        },
                        PropertyBag = identifier.Variant != null ? identifier.Variant.PropertyBag : identifier.Product.PropertyBag,
                        ProductDefinitionName = identifier.Product.DefinitionName

                    }).ToList();
            var imageRequestParam = new GetProductMainImagesParam
            {
                ImageSize = ProductConfiguration.ProductSummaryImageSize,
                ProductImageRequests = imageRequests
            };

            var mainImages = await _damProvider.GetProductMainImagesAsync(imageRequestParam).ConfigureAwait(false);
            
            return mainImages;
        }

        protected virtual RelatedProductViewModel CreateRelatedProductsViewModel(
            Uri baseUrl, 
            CultureInfo cultureInfo, 
            ProductWithVariant productVariant, 
            List<ProductPrice> prices, 
            IEnumerable<ProductMainImage> images)
        {
            var productId = productVariant.Product.Id;
            var variantId = productVariant.Variant != null ? productVariant.Variant.Id : null;           
            var mainImage = images.FirstOrDefault(image => image.ProductId == productId && image.VariantId == variantId);
            var vm = _viewModelMapper.MapTo<RelatedProductViewModel>(productVariant.Product, cultureInfo);

            if (mainImage != null)
            {
                vm.ImageUrl = mainImage.ImageUrl;
                vm.FallbackImageUrl = mainImage.FallbackImageUrl;
            }

            vm.Url = GetProductUrl(baseUrl, cultureInfo, productId, variantId,
                productVariant.Product.DisplayName.GetLocalizedValue(cultureInfo.Name), productVariant.Product.Sku);

            vm.Quantity = GetQuantity();
            vm.ListPrice = GetProductBasePrice(prices, productVariant.Product, productVariant.Variant);
            vm.Price = GetCurrentPrice(prices, productVariant.Product, productVariant.Variant);
            vm.ProductId = productVariant.Product.Id;
            vm.HasVariants = productVariant.Product.Variants != null && productVariant.Product.Variants.Count > 0;
            //For now all the related products add to cart button is enable to add to cart
            vm.IsAvailableToSell = true;

            if (vm.Price != null)
            {
                vm.DisplaySpecialPrice = _localizationProvider.FormatPrice((decimal)vm.Price, cultureInfo);
            }

            var recurringOrdersEnabled = RecurringOrdersSettings.Enabled;
            var recurringOrderProgramName = productVariant.Product.PropertyBag.GetValueOrDefault<string>(Constants.ProductAttributes.RecurringOrderProgramName);

            vm.RecurringOrderProgramName = recurringOrderProgramName;
            vm.IsRecurringOrderEligible = recurringOrdersEnabled && !string.IsNullOrWhiteSpace(recurringOrderProgramName);

            return vm;
        }

	    protected virtual ProductQuantityViewModel GetQuantity()
	    {
	        ProductQuantityViewModel quantity = null;

            if (ProductConfiguration.IsQuantityDisplayed &&
                ProductConfiguration.MinQuantity >= 1 &&
                ProductConfiguration.MaxQuantity >= ProductConfiguration.MinQuantity)
            {
                quantity = new ProductQuantityViewModel
                {
                    Min = ProductConfiguration.MinQuantity,
                    Max = ProductConfiguration.MaxQuantity,
                    Value = ProductConfiguration.MinQuantity
                };
            }

            return quantity;
	    }

	    protected virtual decimal? GetCurrentPrice(IEnumerable<ProductPrice> prices, Overture.ServiceModel.Products.Product product, Variant variant)
        {
            // there may be multiple copies of the product if this is a variant, just take the first
            var price = prices.FirstOrDefault(p => p.ProductId == product.Id);
            
            if (price == null) { return null; }

	        if (variant == null) { return price.Pricing.Price; }

	        var variantPrice = price.VariantPrices.SingleOrDefault(vp => vp.VariantId == variant.Id);

	        return variantPrice == null ? null : (decimal?)variantPrice.Pricing.Price;
        }

        protected virtual decimal? GetProductBasePrice(IEnumerable<ProductPrice> prices, Overture.ServiceModel.Products.Product product, Variant variant)
        {
            // there may be multiple copies of the product if this is a variant, just take the first
            var price = prices.FirstOrDefault(p => p.ProductId == product.Id);
            
            if (price == null)
            {
                return null;
            }

            if (variant == null)
            {
                return price.DefaultPrice;
            }

            var variantPrice = price.VariantPrices.SingleOrDefault(vp => vp.VariantId == variant.Id);

            return variantPrice == null ? null : (decimal?)variantPrice.DefaultPrice;
        }

        /// <summary>
        /// This method uses the ProductRepository to find the requested product, but also ties in the desired variant id
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="variantId"></param>
        /// <param name="scope"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        protected virtual async Task<ProductWithVariant> GetProductWithVariantAsync(
            string productId, 
            string variantId, 
            string scope,
            CultureInfo cultureInfo)
        {
            var baseProduct = await ProductRepository.GetProductAsync(new GetProductParam
                {
                    CultureInfo = cultureInfo,
                    ProductId = productId,
                    Scope = scope
                }).ConfigureAwait(false);

            Variant baseVariant = null;

            if (baseProduct == null)
            {
                return null;
            }

            if (baseProduct?.Variants != null)
            {
                baseVariant = baseProduct.Variants.FirstOrDefault(variant => variant.Id == variantId);
            }

            var productWithVariant = new ProductWithVariant { Product = baseProduct, Variant = baseVariant };
            
            return productWithVariant;
        }

        protected virtual string GetProductUrl(Uri baseUrl, CultureInfo cultureInfo, string productId, string variantId, string productName, string sku)
        {
            var param = new GetProductUrlParam
            {               
                CultureInfo = cultureInfo, 
                ProductId = productId, 
                VariantId = variantId, 
                ProductName = productName,
                SKU = sku
            };
            
            return _productUrlProvider.GetProductUrl(param);
        }
	}
}
