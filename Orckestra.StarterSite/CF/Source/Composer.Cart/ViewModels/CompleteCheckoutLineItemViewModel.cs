using System;
using System.Collections.Generic;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Cart.ViewModels
{
    public sealed class CompleteCheckoutLineItemViewModel : BaseViewModel
    {
        public string Name { get; set; }

        public string ProductId { get; set; }

        [MapTo("ListPrice")]
        public decimal? Price { get; set; }

        public string VariantId { get; set; }

        public double Quantity { get; set; }

        public string CategoryId { get; set; }

        [MapTo("Product.Brand")]
        public string BrandId { get; set; }

        public string Brand { get; set; }

        public List<KeyVariantAttributes> KeyVariantAttributesList { get; set; }

        [MetadataIgnore]
        public string Coupon { get; set; }
    }
}