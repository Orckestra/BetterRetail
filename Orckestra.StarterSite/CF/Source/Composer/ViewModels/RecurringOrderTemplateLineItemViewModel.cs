using System;
using System.Collections.Generic;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.ViewModels
{
    /// <summary>
    /// Recurring order Lineitem detail viewmodel
    /// </summary>
    public class RecurringOrderTemplateLineItemViewModel : BaseViewModel
    {
        public RecurringOrderTemplateLineItemViewModel()
        {
            KeyVariantAttributesList = new List<KeyVariantAttributes>();
        }

        /// <summary>
        /// Gets or sets the recurring order LineItemId
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the product Id
        /// </summary>
        public string ProductId { get; set; }

        /// <summary>
        /// Gets or sets the image path of the product
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// Gets or sets the fallback image to use when the ProductMainImage does a 404
        /// </summary>
        public string FallbackImageUrl { get; set; }

        /// <summary>
        /// Gets or sets the url of the product page
        /// </summary>
        public string ProductUrl { get; set; }

        /// <summary>
        ///Gets or sets the summary of the product
        /// </summary>
        public RecurringProductSummaryViewModel ProductSummary { get; set; }

        /// <summary>
        /// Gets or sets the display the VariantId cause we don't have access to the Variant object yet
        /// </summary>
        public string VariantId { get; set; }

        /// <summary>
        /// Gets or sets the price for one instance of the item (quantity = 1), at the moment that the item is to be processed by the workflow
        /// </summary>
        [Formatting("General", "PriceFormat")]
        public string ListPrice { get; set; }

        /// <summary>
        /// Gets or sets the default price for one instance of the item (quantity = 1), at the moment that the item is to be processed by the workflow
        /// </summary>
        [Formatting("General", "PriceFormat")]
        public string DefaultListPrice { get; set; }

        /// <summary>
        /// If the item price is discounted
        /// </summary>
        public bool IsOnSale { get; set; }

        /// <summary>
        /// Gets or sets the quantity ordered of a product
        /// </summary>
        public double Quantity { get; set; }

        /// <summary>
        /// Gets or sets the total price of a product (normally Product x Quantity)
        /// </summary>
        [Formatting("General", "PriceFormat")]
        public string Total { get; set; }

        /// <summary>
        /// Gets or sets the total price of a product x quantity without discounts applied
        /// </summary>
        [Formatting("General", "PriceFormat")]
        public string TotalWithoutDiscount { get; set; }

        /// <summary>
        /// Gets or sets the key value attributes display values.
        /// </summary>
        public List<KeyVariantAttributes> KeyVariantAttributesList { get; set; }

        /// <summary>
        /// Gets or sets if the lineItem is Valid.
        /// </summary>
        public bool? IsValid { get; set; }

        /// <summary>
        /// Gets or sets the recurring order frequency id
        /// </summary>
        public string RecurringOrderFrequencyName { get; set; }

        /// <summary>
        /// Gets or sets the recurring order program id
        /// </summary>
        public string RecurringOrderProgramName { get; set; }

        /// <summary>
        /// Gets or sets the recurring order frequency display name
        /// </summary>
        public string RecurringOrderFrequencyDisplayName { get; set; }

        /// <summary>
        /// Available frequencies associated to the RecurringOrderProgramName
        /// </summary>
        public List<RecurringOrderProgramFrequencyViewModel> RecurringOrderProgramFrequencies { get; set; }

        /// <summary>
        /// Gets or sets the next occurrence of the reccuring template lineitem
        /// </summary>
        public DateTime NextOccurence { get; set; }

        /// <summary>
        /// Gets or sets the formatted date of the next occurrence of the reccuring template lineitem
        /// </summary>
        public string FormattedNextOccurence { get; set; }

        /// <summary>
        ///  NextOccurence formatted in YYYY-MM-DD
        /// </summary>
        public string NextOccurenceValue { get; set; }

        /// <summary>
        /// Gets or sets the recurring order template shipping address id
        /// </summary>
        public Guid ShippingAddressId { get; set; }

        /// <summary>
        /// Gets or sets the recurring order template billing address id
        /// </summary>
        public Guid BillingAddressId { get; set; }

        /// <summary>
        /// Gets or sets the recurring order template payment method id
        /// </summary>
        public Guid PaymentMethodId { get; set; }

        /// <summary>
        /// Gets or sets the recurring order template shipping provider id
        /// </summary>
        public Guid ShippingProviderId { get; set; }

        /// <summary>
        /// Gets or sets the recurring order template shipping method name
        /// </summary>
        public string ShippingMethodName { get; set; }

        /// <summary>
        /// Gets or sets the url to edit the recurring order line item details
        /// </summary>
        public string EditUrl { get; set; }

        /// <summary>
        /// Gets or sets the url to Recurring Schedule page
        /// </summary>
        public string ScheduleUrl { get; set; }
    }
}
