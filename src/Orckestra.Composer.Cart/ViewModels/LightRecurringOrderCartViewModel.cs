using Orckestra.Composer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Cart.ViewModels
{
    public class LightRecurringOrderCartViewModel : BaseViewModel
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
        public List<LightLineItemDetailViewModel> LineItemDetailViewModels { get; set; }


        /// <summary>
        /// The grand total price including all fees, promotions and taxes
        /// </summary>
        [Formatting("General", "PriceFormat")]
        public string Total { get; set; }

        /// <summary>
        /// Indicate if the request to get cart is running.
        /// </summary>
        public bool GettingCart { get; set; }

        /// <summary>
        /// Indicate if the Processed cart containes errors.
        /// </summary>
        public bool HasErrors { get; set; }
        
        /// <summary>
        /// The user is logged in and thus has account and shipping information available.
        /// </summary>
        public bool IsAuthenticated { get; set; }

        /// <summary>
        /// Determines whether or not the full cart view model will load client-side or not.
        /// </summary>
        public bool IsLoading { get; set; }     

        /// <summary>
        /// Url to view the detailed cart view
        /// </summary>
        public string CartDetailUrl { get; set; }

        public LightRecurringOrderCartViewModel()
        {
        }

        /// <summary>
        /// Date used to identify the cart date
        /// </summary>
        public DateTime NextOccurence { get; set; }

        /// <summary>
        ///  Date formatted used to identify the cart date
        /// </summary>
        public string FormatedNextOccurence { get; set; }
    }
}
