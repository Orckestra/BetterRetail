using System;

namespace Orckestra.Composer.Cart.Parameters
{

    /// <summary>
    /// Parameters for deleting an item to the cart
    /// </summary>
    public class RemoveLineItemParam : BaseCartParam
    {
        /// <summary>
        /// The Request Base Url
        /// </summary>
        public string BaseUrl { get; set; }

        /// <summary>
        /// The unique identifier of the LineItem to be removed
        /// Required
        /// </summary>
        public Guid LineItemId { get; set; }


        public RemoveLineItemParam Clone()
        {
            var param = (RemoveLineItemParam)MemberwiseClone();
            return param;
        }
    }
}

