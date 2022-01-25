using System.Collections.Generic;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Cart.Parameters
{
    /// <summary>
    /// Parameters to add a list of items to a cart
    /// </summary>
    public class AddLineItemsParam : BaseCartParam
    {
        /// <summary>
        /// List of items to be added to a cart
        /// </summary>
        public List<LineItemInfo> LineItems { get; set; }
    }
}