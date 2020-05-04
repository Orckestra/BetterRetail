using System;
using System.Threading.Tasks;
using Orckestra.Composer.Search.ViewModels;
using Orckestra.Composer.Services;
using Orckestra.Overture.ServiceModel.Search;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;

namespace Orckestra.Composer.Search.Providers
{
    public sealed class FromPriceProvider : IPriceProvider
    {
        public static readonly string GroupCurrentPriceFromProperty = "GroupCurrentPriceFrom";
        public static readonly string GroupCurrentPriceToProperty = "GroupCurrentPriceTo";
        public static readonly string GroupRegularPriceFromProperty = "GroupRegularPriceFrom";
        public static readonly string GroupRegularPriceToProperty = "GroupRegularPriceTo";
        public static readonly string CurrentPricePriceListIdProperty = "CurrentPricePriceListId";

        //TODO: To be refactored. Composer Context should NEVER be referenced from here.
        private IComposerContext ComposerContext { get; }

        public FromPriceProvider(IComposerContext composerContext)
        {
            ComposerContext = composerContext ?? throw new ArgumentNullException(nameof(composerContext));
        }

        // https://tfs12.orckestra.com/overture%20solutions/WorkItemTracking/v1.0/AttachFileHandler.ashx?FileID=5412&FileName=SearchItemPrice.pdf
        public Task<ProductPriceSearchViewModel> GetPriceAsync(bool? hasVariants, ProductDocument document)
        {
            if (document == null) { throw new ArgumentNullException(nameof(document)); }
            if (document.PropertyBag == null) { throw new ArgumentException(GetMessageOfNull(nameof(document.PropertyBag)), nameof(document)); }            

            var productPriceSearchViewModel = hasVariants.Value ?
                GetProductPriceWithVariant(document) :
                GetProductPriceWithoutVariant(document);

            return Task.FromResult(productPriceSearchViewModel);
        }

        private ProductPriceSearchViewModel GetProductPriceWithVariant(ProductDocument document)
        {
            return HasOneOrMoreVariantInDiscount(document)
                ? CreateOneOrMoreVariantInDiscountPrice(document)
                : CreateNoVariantInDiscountPrice(document);
        }

        private ProductPriceSearchViewModel CreateNoVariantInDiscountPrice(ProductDocument document)
        {
            return AreAllVariantRegularPriceTheSame(document)
                ? CreatePrice(document, false, false, null, GetFromRegularPrice(document))
                : CreatePrice(document, true, false, null, GetFromRegularPrice(document));
        }

        private ProductPriceSearchViewModel CreateOneOrMoreVariantInDiscountPrice(ProductDocument document)
        {
            return AreAllVariantRegularPriceTheSame(document)
                ? CreateAllVariantRegularPriceTheSamePrice(document)
                : CreateNotAllVariantRegularPriceTheSamePrice(document);
        }

        private ProductPriceSearchViewModel CreateNotAllVariantRegularPriceTheSamePrice(ProductDocument document)
        {
            if (AreAllVariantDiscountPriceTheSame(document))
            {
                var minRegularPrice = GetFromRegularPrice(document);
                var discountPrice = GetFromDiscountPrice(document);
                var minPrice = GetMinPrice(discountPrice, minRegularPrice);

                return CreatePrice(document, true, true, minPrice, null);
            }
            else
            {
                var minRegularPrice = GetFromRegularPrice(document);
                var minDiscountPrice = GetFromDiscountPrice(document);
                var minPrice = GetMinPrice(minDiscountPrice, minRegularPrice);

                return CreatePrice(document, true, true, minPrice, null);
            }
        }

        private ProductPriceSearchViewModel CreateAllVariantRegularPriceTheSamePrice(ProductDocument document)
        {
            if (AreAllVariantDiscountPriceTheSame(document))
            {
                var regularPrice = GetFromRegularPrice(document);
                var discountPrice = GetFromDiscountPrice(document);

                return CreatePrice(document, false, true, discountPrice, regularPrice);
            }
            else
            {
                var minDiscountPrice = GetFromDiscountPrice(document);

                return CreatePrice(document, true, true, minDiscountPrice, null);
            }
        }

        private static double? GetMinPrice(double? firstPrice, double? secondPrice)
        {
            if (!firstPrice.HasValue && !secondPrice.HasValue) { return null; }
            if (firstPrice.HasValue && !secondPrice.HasValue) { return firstPrice; }
            if (!firstPrice.HasValue && secondPrice.HasValue) { return secondPrice; }

            return Math.Min(firstPrice.Value, secondPrice.Value);
        }

        private static bool AreAllVariantRegularPriceTheSame(ProductDocument document)
        {
            var fromRegularPrice = GetPriceForComparison(GetFromRegularPrice(document) ?? 0);
            var toRegularPrice = GetPriceForComparison(GetToRegularPrice(document) ?? 0);

            return fromRegularPrice == toRegularPrice;
        }

        private static bool AreAllVariantDiscountPriceTheSame(ProductDocument document)
        {
            var fromDiscountPrice = GetPriceForComparison(GetFromDiscountPrice(document) ?? 0);
            var toDiscountPrice = GetPriceForComparison(GetToDiscountPrice(document) ?? 0);

            return fromDiscountPrice == toDiscountPrice;
        }

        private static bool HasOneOrMoreVariantInDiscount(ProductDocument document)
        {
            var fromDiscountPrice = GetPriceForComparison(GetFromDiscountPrice(document) ?? 0);
            var toDiscountPrice = GetPriceForComparison(GetToDiscountPrice(document) ?? 0);
            var fromRegularPrice = GetPriceForComparison(GetFromRegularPrice(document) ?? 0);
            var toRegularPrice = GetPriceForComparison(GetToRegularPrice(document) ?? 0);

            return fromDiscountPrice < fromRegularPrice || toDiscountPrice < toRegularPrice;
        }

        private ProductPriceSearchViewModel GetProductPriceWithoutVariant(ProductDocument document)
        {
            return IsInDiscount(document) 
                ? CreatePrice(document, false, true, document.EntityPricing.CurrentPrice, document.EntityPricing.RegularPrice) 
                : CreatePrice(document, false, false, null, document.EntityPricing.RegularPrice);
        }

        private ProductPriceSearchViewModel CreatePrice(
            ProductDocument document,
            bool hasPriceRange,
            bool isOnSale,
            double? discountPrice,
            double? regularPrice)
        {
            var productPriceSearchViewModel = new ProductPriceSearchViewModel
            {
                PriceListId = GetPriceListId(document),
                HasPriceRange = hasPriceRange,
                IsOnSale = isOnSale,
                Price = discountPrice,
                ListPrice = regularPrice,
                DisplaySpecialPrice = GetDisplayPrice(discountPrice),
                DisplayPrice = GetDisplayPrice(regularPrice)
            };

            return productPriceSearchViewModel;
        }

        private static string GetPriceListId(ProductDocument document)
        {
            var propertyBag = document.PropertyBag;

            return propertyBag.ContainsKey(CurrentPricePriceListIdProperty) ?
                   propertyBag[CurrentPricePriceListIdProperty].ToString() :
                   null;
        }

        private string GetDisplayPrice(double? price)
        {
            return !price.HasValue ? null : price.Value.ToString("C2", ComposerContext.CultureInfo);
        }

        private static int GetPriceForComparison(double price)
        {
            return (int)(price * 100);
        }

        private static bool IsInDiscount(ProductDocument document)
        {
            var listPrice = GetPriceForComparison(document.EntityPricing.RegularPrice.GetValueOrDefault(0));
            var price = GetPriceForComparison(document.EntityPricing.CurrentPrice.GetValueOrDefault(0));

            return listPrice > price;
        }

        private static double? GetFromDiscountPrice(ProductDocument document)
        {
            var propertyBag = document.PropertyBag;

            return propertyBag.ContainsKey(GroupCurrentPriceFromProperty) 
                ? Convert.ToDouble(propertyBag[GroupCurrentPriceFromProperty]) 
                : (double?)null;
        }

        private static double? GetToDiscountPrice(ProductDocument document)
        {
            var propertyBag = document.PropertyBag;

            return propertyBag.ContainsKey(GroupCurrentPriceToProperty) 
                ? Convert.ToDouble(propertyBag[GroupCurrentPriceToProperty]) 
                : (double?)null;
        }

        private static double? GetFromRegularPrice(ProductDocument document)
        {
            var propertyBag = document.PropertyBag;

            return propertyBag.ContainsKey(GroupRegularPriceFromProperty) 
                ? Convert.ToDouble(propertyBag[GroupRegularPriceFromProperty]) 
                : (double?)null;
        }

        private static double? GetToRegularPrice(ProductDocument document)
        {
            var propertyBag = document.PropertyBag;

            return propertyBag.ContainsKey(GroupRegularPriceToProperty) 
                ? Convert.ToDouble(propertyBag[GroupRegularPriceToProperty]) 
                : (double?)null;
        }
    }
}