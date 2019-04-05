using System;
using System.Collections.Generic;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Cart.ViewModels
{
    public sealed class LineItemDetailViewModel : BaseViewModel
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Product LineItemId
        /// </summary>
        public string ProductId { get; set; }


        /// <summary>
        /// Image path of the product
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// Fallback image to use when the ProductMainImage does a 404
        /// </summary>
        public string FallbackImageUrl { get; set; }

        /// <summary>
        /// The Url of the product page
        /// </summary>
        public string ProductUrl { get; set; }

        /// <summary>
        /// Summary of the product
        /// </summary>
        public CartProductSummaryViewModel ProductSummary { get; set; }

        /// <summary>
        /// Display the VariantId cause we don't have access to the Variant object yet
        /// </summary>
        public string VariantId { get; set; }

        /// <summary>
        /// If the item price is discounted
        /// </summary>
        public bool IsOnSale { get; set; }

        /// <summary>
        /// The price for one instance of the item (quantity = 1), at the moment that the item is to be processed by the workflow
        /// </summary>
        [Formatting("General", "PriceFormat")]
        public string ListPrice { get; set; }

        /// <summary>
        /// The default price for one instance of the item (quantity = 1), at the moment that the item is to be processed by the workflow
        /// </summary>
        [Formatting("General", "PriceFormat")]
        public string DefaultListPrice { get; set; }

        /// <summary>
        /// Quantity ordered of a product
        /// </summary>
        public double Quantity { get; set; }

        /// <summary>
        /// Total price of a product (normally Product x Quantity)
        /// </summary>
        [Formatting("General", "PriceFormat")]
        public string Total { get; set; }

        /// <summary>
        /// Total price of a product x quantity without discounts applied
        /// </summary>
        [Formatting("General", "PriceFormat")]
        public string TotalWithoutDiscount { get; set; }

        /// <summary>
        /// The total amount of discount.
        /// </summary>
        [Formatting("General", "PriceFormat")]
        public string SavingsTotal { get; set; }

        /// <summary>
        /// List of discounts applicable to this line item.
        /// </summary>
        /// TODO: Add the DoNotMap attribute. Mapping discounts is too complex for the mapper.
        /// TODO: Not yet created as 02/06/15 MK I think you were doing it
        public List<RewardViewModel> Rewards { get; set; }

        /// <summary>
        /// Is true if a product is on sale (i.e. <see cref="IsOnSale"/> is true)
        /// or Discounts have been applied in Overture.
        /// </summary>
        public bool IsPriceDiscounted { get; set; }

        /// <summary>
        /// The collection of additional fees to apply on this line item.
        /// </summary>
        public List<AdditionalFeeViewModel> AdditionalFees { get; set; }

        /// <summary>
        /// The amount for all line item additional fees applied to this line item. This value is usually computed during the workflow execution. When null, indicates that the value has not been calculated.
        /// </summary>
        public decimal? AdditionalFeeAmount { get; set; }

        /// <summary>
        /// the key value attributes display values.
        /// </summary>
        public List<KeyVariantAttributes> KeyVariantAttributesList { get; set; }

        /// <summary>
        /// Determines if the LineItem is Valid.
        /// </summary>
        public bool? IsValid { get; set; }

        /// <summary>
        /// Gets or sets the name of the recurring order program frequency
        /// </summary>
        public string RecurringOrderFrequencyName { get; set; }

        /// <summary>
        /// Gets or sets the name of the recurring order program
        /// </summary>
        public string RecurringOrderProgramName { get; set; }

        /// <summary>
        /// Gets or sets the display name of the recurring order program frequency
        /// </summary>
        public string RecurringOrderFrequencyDisplayName { get; set; }


        public LineItemDetailViewModel()
        {
            Rewards = new List<RewardViewModel>();
            AdditionalFees = new List<AdditionalFeeViewModel>();
            KeyVariantAttributesList = new List<KeyVariantAttributes>();
        }
    }
}
