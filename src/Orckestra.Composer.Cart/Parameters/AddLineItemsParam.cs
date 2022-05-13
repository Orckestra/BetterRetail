using System;
using System.Collections.Generic;
using System.Globalization;
using Orckestra.Overture.ServiceModel;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Cart.Parameters
{
    /// <summary>
    /// Parameters to add a list of items to a cart
    /// </summary>
    public class AddLineItemsParam : BaseCartParam
    {
        /// <summary>
        /// Name of a cart
        /// Required
        /// </summary>
        public string CartName { get; set; }
        /// <summary>
        /// Culture for a returned cart info
        /// Required
        /// </summary>
        public CultureInfo CultureInfo { get; set; }
        /// <summary>
        /// Customer id of a cart
        /// Required
        /// </summary>
        public Guid CustomerId { get; set; }
        /// <summary>
        /// List of items to be added to a cart
        /// </summary>
        public List<LineItemInfo> LineItems { get; set; }
        /// <summary>
        /// Scope to be used during operation
        /// Required
        /// </summary>
        public string Scope { get; set; }
    }
}