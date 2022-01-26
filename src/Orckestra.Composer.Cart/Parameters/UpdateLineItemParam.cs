using System;
using System.Globalization;
using Orckestra.Overture.ServiceModel;

namespace Orckestra.Composer.Cart.Parameters
{
    public class UpdateLineItemParam
    {
        /// <summary>
        /// The Request Base Url
        /// </summary>
        public string BaseUrl { get; set; }

        /// <summary>
        /// The name associated to the requested cart
        /// Required
        /// </summary>
        public string CartName { get; set; }
       
        /// <summary>
        /// The culture name in which language the data will be returned
        /// </summary>
        public CultureInfo CultureInfo { get; set; }
       
        /// <summary>
        /// The unique identifier of the Customer owning the cart
        /// </summary>
        public Guid CustomerId { get; set; }

        /// <summary>
        /// The id of the requested scope
        /// </summary>
        public string ScopeId { get; set; }

        /// <summary>
        /// If the product is to be wrapped as a gift, a message to be written on that
        /// </summary>
        public string GiftMessage { get; set; }

        /// <summary>
        /// Whether or not the item will wrapped in a gift presentation
        /// </summary>
        public bool GiftWrap { get; set; }

        /// <summary>
        /// The unique identifier of the LineItem to update
        /// </summary>
        public Guid LineItemId { get; set; }

        /// <summary>
        /// The property bag containing extended properties for this command
        /// </summary>
        public PropertyBag PropertyBag { get; set; }

        /// <summary>
        /// The number of times this item is bought for this LineItem
        /// </summary>
        public double Quantity { get; set; }

        /// <summary>
        /// The name of the recurring order program frequency
        /// </summary>
        public string RecurringOrderFrequencyName { get; set; }
        
        /// <summary>
        /// The name of the recurring order program.
        /// </summary>
        public string RecurringOrderProgramName { get; set; }
    }
}
