using System.Collections.Generic;
using Orckestra.Composer.ViewModels;
using Orckestra.Composer.Cart.ViewModels.Order;

namespace Orckestra.Composer.Cart.ViewModels
{
    public sealed class OrderSummaryViewModel : BaseViewModel
    {
        /// <summary>
        /// The sum price of all lineitems
        /// </summary>
        [Formatting("General", "PriceFormat")]
        public string SubTotal { get; set; }

        /// <summary>
        /// The grand total price including all fees, promotions and taxes
        /// </summary>
        [Formatting("General", "PriceFormat")]
        public string Total { get; set; }

        /// <summary>
        /// The total price of all line items including line item discounts, not including cart discounts.
        /// </summary>
        [Formatting("General", "PriceFormat")]
        public string LineItemsTotal { get; set; }

        /// <summary>
        /// Shipping used in the summary, grouped by taxable and nontaxable shipping. 
        /// Display name is based on the resx L_ShippingBasedOn and L_ShippingBasedOnNonTaxable
        /// </summary>
        public IList<OrderShippingMethodViewModel> Shippings { get; set; }

        /// <summary>
        /// The shipping fee
        /// </summary>
        public string Shipping { get; set; }

        /// <summary>
        /// Indicate if the shipping is taxable 
        /// </summary>
        public bool IsShippingTaxable { get; set; }

        /// <summary>
        /// Indicate if we need to estimate shipping
        /// </summary>
        public bool IsShippingEstimatedOrSelected { get; set; }

        /// <summary>
        /// The total amount of taxes.
        /// </summary>
        [Formatting("General", "PriceFormat")]
        public string TaxTotal { get; set; }

        /// <summary>
        /// The total amount of discount.
        /// </summary>
        [Formatting("General", "PriceFormat")]
        public string DiscountTotal { get; set; }

        /// <summary>
        /// The total amount of discount.
        /// </summary>
        [Formatting("General", "PriceFormat")]
        public string SavingsTotal { get; set; }

        /// <summary>
        /// Indicate if the cart has discount.
        /// </summary>
        public bool HasReward { get; set; }

        /// <summary>
        /// The list of taxes
        /// </summary>
        public IList<TaxViewModel> Taxes { get; set; }

        /// <summary>
        /// The url to target with the checkout button
        /// </summary>
        public string CheckoutUrlTarget { get; set; }

        /// <summary>
        /// The url to target with the edit shopping cart link
        /// </summary>
        public string EditCartUrlTarget { get; set; }

        /// <summary>
        /// The Urls of the checkout steps.
        /// </summary>
        public IList<string>  CheckoutStepUrls { get; set; }

        /// <summary>
        /// The Redirect action info if the customer goes to a step not available yet.
        /// </summary>
        public CheckoutRedirectActionViewModel CheckoutRedirectAction { get; set; }

        /// <summary>
        /// The amount for all shipment additional fees applied to this shipment. 
        /// This value is usually computed during the workflow execution. 
        /// When null, indicates that the value has not been calculated.
        /// </summary>
        [Formatting("General", "PriceFormat")]
        public string ShipmentAdditionalFeeAmount { get; set; }

        /// <summary>
        /// the sum of LineItems additional fees and the sum of shipment additional fees.
        /// </summary>
        [Formatting("General", "PriceFormat")]
        public string AdditionalFeeTotal { get; set; }

        /// <summary>
        /// A summary of lineitems additional fees grouped by name
        /// </summary>
        public IList<AdditionalFeeSummaryViewModel> AdditionalFeeSummaryList { get; set; }

        /// <summary>
        /// A summary of shipment additional fees grouped by name
        /// </summary>
        public IList<AdditionalFeeSummaryViewModel> ShipmentAdditionalFeeSummaryList { get; set; }

        /// <summary>
        /// Gets or sets the list of Discounts viewmodel.
        /// </summary>
        public List<RewardViewModel> Rewards { get; set; }

        public OrderSummaryViewModel()
        {
            Taxes = new List<TaxViewModel>();
            CheckoutStepUrls = new List<string>();
            AdditionalFeeSummaryList = new List<AdditionalFeeSummaryViewModel>();
            ShipmentAdditionalFeeSummaryList = new List<AdditionalFeeSummaryViewModel>();
            Rewards = new List<RewardViewModel>();
        }
    }
}
