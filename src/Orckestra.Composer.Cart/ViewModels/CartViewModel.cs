using System;
using System.Collections.Generic;
using Orckestra.Composer.Cart.Helper;
using Orckestra.Composer.Helper;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Cart.ViewModels
{
    /// <summary>
    /// Lightweight CartViewModel with displayable content
    /// </summary>
    public sealed class CartViewModel : BaseViewModel
    {
        /// <summary>
        /// True if the cart contain no lineItems
        /// </summary>
        public bool IsCartEmpty { get; set; }

        /// <summary>
        /// Number of different line items (different products, with disregards the quantity)
        /// </summary>
        public int LineItemCount { get; set; }

        /// <summary>
        /// Number of different invalid line items (different products, with disregards the quantity)
        /// </summary>
        public int InvalidLineItemCount { get; set; }

        /// <summary>
        /// Sum of all item quantites
        /// </summary>
        public int TotalQuantity { get; set; }

        /// <summary>
        /// List of line items (different products)
        /// </summary>
        public List<LineItemDetailViewModel> LineItemDetailViewModels { get; set; }

        /// <summary>
        /// The order summary at the right of the cart where is the total and subtotal
        /// </summary>
        public OrderSummaryViewModel OrderSummary { get; set; }

        /// <summary>
        /// The homepage Url
        /// </summary>
        public string HomepageUrl { get; set; }

        /// <summary>
        /// Displays the discounts on the cart.
        /// </summary>
        /// TODO: Add Ignore mapping here. Reward mapping is too complex.
        public List<RewardViewModel> Rewards { get; set; }

        /// <summary>
        /// Valid coupons applied to the cart.
        /// </summary>
        public CouponsViewModel Coupons { get; set; }

        /// <summary>
        /// The address of the client of the first shipment.
        /// </summary>
        public AddressViewModel ShippingAddress { get; set; }

        /// <summary>
        /// The ShippingMethod of the first shipment.
        /// </summary>
        public ShippingMethodViewModel ShippingMethod { get; set; }

        /// <summary>
        /// The address of the store of the first shipment.
        /// </summary>
        public Guid? PickUpLocationId { get; set; }

        /// <summary>
        /// The Customer info.
        /// </summary>
        public CustomerSummaryViewModel Customer { get; set; }

        /// <summary>
        /// The Overture Id of the first shipment.
        /// </summary>
        public Guid CurrentShipmentId { get; set; }

        /// <summary>
        /// Indicate if the request to get cart is running.
        /// </summary>
        public bool GettingCart { get; set; }

        /// <summary>
        /// Indicate if the Processed cart containes errors.
        /// </summary>
        public bool HasErrors { get; set; }

        /// <summary>
        /// The Payment info.
        /// </summary>
        public PaymentViewModel Payment { get; set; }

        /// <summary>
        /// The user is logged in and thus has account and shipping information available.
        /// </summary>
        public bool IsAuthenticated { get; set; }

        /// <summary>
        /// Determines whether or not the full cart view model will load client-side or not.
        /// </summary>
        public bool IsLoading { get; set; }

        /// <summary>
        /// Indicates if the cart contains recurring lineitems
        /// </summary>
        public bool HasRecurringLineitems {
            get
            {
                return RecurringOrderCartHelper.IsCartContainsRecurringOrderItems(LineItemDetailViewModels);
            }
        }

        public CartViewModel()
        {
            LineItemDetailViewModels = new List<LineItemDetailViewModel>();
            Rewards = new List<RewardViewModel>();
        }
    }
}
