using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Product.Exceptions;
using Orckestra.Composer.Product.Extensions;
using Orckestra.Composer.Product.Parameters;
using Orckestra.Composer.Product.Repositories;
using Orckestra.Composer.Product.ViewModels;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Repositories;
using Orckestra.Composer.Services;
using Orckestra.Composer.Services.Lookup;
using Orckestra.Overture.ServiceModel.Metadata;
using Orckestra.Overture.ServiceModel.Products;

namespace Orckestra.Composer.Product.Services
{
    /// <summary>
    /// Service for dealing with Product Specifications.
    /// </summary>
    public class ProductSpecificationsViewService : IProductSpecificationsViewService
    {
        private readonly IComposerContext _context;
        private readonly IProductRepository _productRepository;
        private readonly ProductFormatter _productFormatter;

        public ProductSpecificationsViewService(IComposerContext context,
                                                IProductRepository productRepository, 
                                                ILocalizationProvider localizationProvider,
                                                ILookupService lookupService)
        {
            if (context == null) { throw new ArgumentNullException("context"); }
            if (productRepository == null) { throw new ArgumentNullException("productRepository"); }
            if (localizationProvider == null) { throw new ArgumentNullException("localizationProvider"); }
            if (lookupService == null) { throw new ArgumentNullException("lookupService"); }

            _context = context;
            _productRepository = productRepository;
            _productFormatter = new ProductFormatter(localizationProvider, lookupService);

        }

        /// <summary>
        /// Gets a <see cref="SpecificationsViewModel" /> for a given product.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <returns>
        /// Instance of <see cref="SpecificationsViewModel" />.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">param</exception>
        public virtual async Task<SpecificationsViewModel> GetProductSpecificationsViewModelAsync(GetProductSpecificationsParam param)
        {
            SpecificationsViewModel vm = GetEmptySpecificationsViewModel(param);
            vm.Groups = await GetSpecificationsGroupsAsync(param).ConfigureAwait(false);
            return vm;
        }

        public virtual SpecificationsViewModel GetEmptySpecificationsViewModel(GetProductSpecificationsParam param)
        {
            if (param == null) { throw new ArgumentNullException("param"); }
            if (string.IsNullOrWhiteSpace(param.ProductId)) { throw new ArgumentException("The product id cannot be null or white space"); }

            var vm = new SpecificationsViewModel
            {
                ProductId = param.ProductId,
                VariantId = param.VariantId,
                Groups = new List<SpecificationsGroupViewModel>()
            };

            vm.Context["productId"] = vm.ProductId;
            vm.Context["variantId"] = vm.VariantId;

            return vm;
        }

        private async Task<List<SpecificationsGroupViewModel>> GetSpecificationsGroupsAsync(GetProductSpecificationsParam param)
        {
            var productDefinition = await GetProductDefinitionAsync(param).ConfigureAwait(false);

            if (productDefinition == null) { return null; }

            var tasks = productDefinition.PropertyGroups
                .Where(group => @group.IsIncluded())
                .OrderBy(group => @group.DisplayOrder)
                .Select(async group => new SpecificationsGroupViewModel
                {
                    Title = GetLocalizedTitle(@group),
                    Attributes = await GetSpecificationsAttributesAsync(@group, param).ConfigureAwait(false)
                });

            var specificationGroups = await Task.WhenAll(tasks).ConfigureAwait(false);

            return specificationGroups.Where(group => @group.Attributes.Any()).ToList();
        }

        private async Task<ProductDefinition> GetProductDefinitionAsync(GetProductSpecificationsParam param)
        {
            var product = await GetProductAsync(param).ConfigureAwait(false);

            if (product == null)
            {
                throw new ProductSpecificationsNotFoundException("The specifications could not be found because the product does not exist");
            }

            return await _productRepository.GetProductDefinitionAsync(new GetProductDefinitionParam
            {
                Name = product.DefinitionName,
                CultureInfo = _context.CultureInfo

            }).ConfigureAwait(false);
        }

        private string GetLocalizedTitle(ProductPropertyDefinitionGroup group)
        {
            return group.DisplayName.GetLocalizedValue(_context.CultureInfo.Name);
        }

        private async Task<List<SpecificationsAttributeViewModel>> GetSpecificationsAttributesAsync(ProductPropertyDefinitionGroup group, GetProductSpecificationsParam param)
        {
            var tasks = group.Properties
                             .Where(property => property.IsIncluded())
                             .OrderBy(property => property.DisplayOrder)
                             .Select(async property => new SpecificationsAttributeViewModel
                             {
                                 PropertyName = property.PropertyName,
                                 Title = GetLocalizedTitle(property),
                                 Value = await GetSpecificationsAttributeValueAsync(property, param).ConfigureAwait(false)
                             });

            var specificationAttributes = await Task.WhenAll(tasks).ConfigureAwait(false);
            return specificationAttributes.Where(attribute => !string.IsNullOrWhiteSpace(attribute.Value)).ToList();
        }

        private string GetLocalizedTitle(ProductPropertyDefinition property)
        {
            return property.DisplayName.GetLocalizedValue(_context.CultureInfo.Name);
        }

        private async Task<string> GetSpecificationsAttributeValueAsync(ProductPropertyDefinition property, GetProductSpecificationsParam param)
        {
            if (await IsVariantAttributeAvailableAsync(property, param).ConfigureAwait(false))
            {
                return await GetVariantAttributeValueAsync(property, param).ConfigureAwait(false);
            }

            if (await IsProductAttributeAvailableAsync(property, param).ConfigureAwait(false))
            {
                return await GetProductAttributeValueAsync(property, param).ConfigureAwait(false);
            }

            return string.Empty;
        }

        private async Task<bool> IsVariantAttributeAvailableAsync(ProductPropertyDefinition property, GetProductSpecificationsParam param)
        {
            Variant variant = await GetVariantAsync(param).ConfigureAwait(false);
            return variant != null && variant.PropertyBag != null && variant.PropertyBag.ContainsKey(property.PropertyName);
        }

        private async Task<string> GetVariantAttributeValueAsync(ProductPropertyDefinition property, GetProductSpecificationsParam param)
        {
            Variant variant = await GetVariantAsync(param).ConfigureAwait(false);
            return _productFormatter.FormatValue(property, variant.PropertyBag[property.PropertyName], _context.CultureInfo);
        }

        private async Task<bool> IsProductAttributeAvailableAsync(ProductPropertyDefinition property, GetProductSpecificationsParam param)
        {
            var product = await GetProductAsync(param).ConfigureAwait(false);
            if (product == null)
            {
                throw new ProductSpecificationsNotFoundException("The specifications could not be found because the product does not exist or is inactive");
            }
            return product.PropertyBag != null && product.PropertyBag.ContainsKey(property.PropertyName);
        }

        private async Task<string> GetProductAttributeValueAsync(ProductPropertyDefinition property, GetProductSpecificationsParam param)
        {
            var product = await GetProductAsync(param).ConfigureAwait(false);
            if (product == null)
            {
                throw new ProductSpecificationsNotFoundException("The specifications could not be found because the product does not exist or is inactive");
            }
            object value = product.PropertyBag[property.PropertyName];
            return _productFormatter.FormatValue(property, value, _context.CultureInfo);
        }

        private async Task<Variant> GetVariantAsync(GetProductSpecificationsParam param)
        {
            var product = await GetProductAsync(param).ConfigureAwait(false);
            if (product == null)
            {
                throw new ProductSpecificationsNotFoundException("The specifications could not be found because the product does not exist or is inactive");
            }
            return product.Variants.FirstOrDefault(v => v.Id == param.VariantId);
        }

        private async Task<Overture.ServiceModel.Products.Product> GetProductAsync(GetProductSpecificationsParam param) // it is ok to call this method multiple times within one request because the product is cached
        {
            return await _productRepository.GetProductAsync(new GetProductParam
            {
                ProductId = param.ProductId,
                Scope = _context.Scope,
                CultureInfo = _context.CultureInfo

            }).ConfigureAwait(false);
        }
    }
}
