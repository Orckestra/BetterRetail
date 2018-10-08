using System.Collections.Generic;

namespace Orckestra.Composer.Cart.Parameters
{
    public class UpdateCheckoutCartParam
    {
        /// <summary>
        /// The parameters to get the cart.
        /// Required
        /// </summary>
        public GetCartParam GetCartParam { get; set; }

        /// <summary>
        /// The cart update values.
        /// Required
        /// </summary>
        public Dictionary<string, string> UpdateValues { get; set; }

        /// <summary>
        /// The number of the current step.
        /// Required
        /// </summary>
        public int CurrentStep { get; set; }

        /// <summary>
        /// The value indicating if the customer is a guest.
        /// </summary>
        public bool IsGuest { get; set; }
    }
}
