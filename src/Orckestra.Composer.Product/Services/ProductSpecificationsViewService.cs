using System;
using System.Collections.Generic;
using System.Linq;
using Orckestra.Composer.Product.Extensions;
using Orckestra.Composer.Product.Parameters;
using Orckestra.Composer.Product.ViewModels;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Services;
using Orckestra.Composer.Services.Lookup;
using Orckestra.Overture.ServiceModel.Metadata;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;

namespace Orckestra.Composer.Product.Services
{
    /// <summary>
    /// Service for dealing with Product Specifications.
    /// </summary>
    public class ProductSpecificationsViewService : IProductSpecificationsViewService
    {
        protected IComposerContext Context { get; set; }
        protected ProductFormatter ProductFormatter { get; set; }

        public ProductSpecificationsViewService(IComposerContext context, ILocalizationProvider localizationProvider, ILookupService lookupService)
        {
            if (localizationProvider == null) { throw new ArgumentNullException(nameof(localizationProvider)); }
            if (lookupService == null) { throw new ArgumentNullException(nameof(lookupService)); }

            Context = context ?? throw new ArgumentNullException(nameof(context));    
            
            ProductFormatter = new ProductFormatter(localizationProvider, lookupService);
        }

        /// <summary>
        /// Gets a <see cref="SpecificationsViewModel" /> for a given product.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <returns>
        /// Instance of <see cref="SpecificationsViewModel" />.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">param</exception>
        public virtual SpecificationsViewModel GetProductSpecificationsViewModel(GetProductSpecificationsParam param)
        {
            if (param == null) throw new ArgumentNullException(nameof(param));
            if (param.Product == null) throw new ArgumentException(GetMessageOfNull(nameof(param.Product)), nameof(param));
            if (param.ProductDefinition == null) throw new ArgumentException(GetMessageOfNull(nameof(param.ProductDefinition)), nameof(param));

            if (IsInheritedSpecification(param)) { return null; }

            var vm = new SpecificationsViewModel
            {
                ProductId = param.Product.Id,
                VariantId = param.VariantId,
                Groups = GetSpecificationsGroups(param)
            };

            return vm;
        }

        protected virtual List<SpecificationsGroupViewModel> GetSpecificationsGroups(GetProductSpecificationsParam param)
        {
            var specificationGroups = param.ProductDefinition.PropertyGroups
                .Where(group => @group.IsIncluded())
                .OrderBy(group => @group.DisplayOrder)
                .Select(group => new SpecificationsGroupViewModel
                {
                    Title = GetLocalizedTitle(@group.DisplayName),
                    Attributes = GetSpecificationsAttributes(@group, param)
                })
                .Where(group => @group.Attributes.Any())
                .ToList();

            return specificationGroups;
        }

        protected virtual List<SpecificationsAttributeViewModel> GetSpecificationsAttributes(ProductPropertyDefinitionGroup group, GetProductSpecificationsParam param)
        {
            var specificationAttributes = group.Properties
                .Where(property => property.IsIncluded())
                .OrderBy(property => property.DisplayOrder)
                .Select(property => new SpecificationsAttributeViewModel
                {
                    PropertyName = property.PropertyName,
                    Title = GetLocalizedTitle(property.DisplayName),
                    Value = GetSpecificationsAttributeValue(property, param)
                })
                .Where(attribute => !string.IsNullOrWhiteSpace(attribute.Value))
                .ToList();

            return specificationAttributes;
        }

        protected virtual string GetSpecificationsAttributeValue(ProductPropertyDefinition property, GetProductSpecificationsParam param)
        {
            var variantProperty = param.Product.Variants.Find(v => v.Id == param.VariantId)?.PropertyBag;
            if (variantProperty?.ContainsKey(property.PropertyName) ?? false)
            {
                return ProductFormatter.FormatValue(property, variantProperty[property.PropertyName], Context.CultureInfo);
            }

            if (param.Product.PropertyBag?.ContainsKey(property.PropertyName) ?? false)
            {
                return ProductFormatter.FormatValue(property, param.Product.PropertyBag[property.PropertyName], Context.CultureInfo);
            }

            return string.Empty;
        }

        protected string GetLocalizedTitle(Overture.ServiceModel.LocalizedString displayName)
        {
            return displayName.GetLocalizedValue(Context.CultureInfo.Name);
        }

        protected virtual bool IsInheritedSpecification(GetProductSpecificationsParam param)
        {
            var variant = param.Product.Variants.FirstOrDefault(v => v.Id == param.VariantId);

            if (variant == null) return false;

            var allPropertyNames = param.ProductDefinition.PropertyGroups
                .Where(group => @group.IsIncluded())
                .Aggregate(new List<string>(), (allProperties, next) =>
                 {
                     var properties = next.Properties
                        .Where(property => property.IsIncluded())
                        .Select(property => property.PropertyName);
                     allProperties.AddRange(properties);
                     return allProperties;
                 });

            return !allPropertyNames.Any(propertyName => variant.PropertyBag.ContainsKey(propertyName));
        }
    }
}