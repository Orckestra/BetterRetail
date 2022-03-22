using System;
using System.Globalization;

namespace Orckestra.Composer.Cart.Parameters
{
    /// <summary>
    /// Parameters for adding an item to the cart
    /// </summary>
    public class AddLineItemParam : BaseCartParam
    {
        /// <summary>
        /// The ScopeId where to find the cart
        /// Required
        /// </summary>
        public string Scope { get; set; }

        /// <summary>
        /// The culture for returned cart info
        /// Required
        /// </summary>
        public CultureInfo CultureInfo { get; set; }

        /// <summary>
        /// The customer id to who belong the cart
        /// Required
        /// </summary>
        public Guid CustomerId { get; set; }

        /// <summary>
        /// The name of the cart where to add item
        /// Required
        /// </summary>
        public string CartName { get; set; }

        /// <summary>
        /// The product id to add
        /// Required
        /// </summary>
        public string ProductId { get; set; }

        /// <summary>
        /// The variant id
        /// Optionnal if the product doesn't have any.
        /// </summary>
        public string VariantId { get; set; }

        /// <summary>
        /// Quantity to add
        /// Required, must be positive
        /// </summary>
        public double Quantity { get; set; }

        /// <summary>
        /// The Request Base Url
        /// </summary>
        public string BaseUrl { get; set; }

        /// <summary>
        /// The name of the recurring order program frequency
        /// </summary>
        public string RecurringOrderFrequencyName { get; set; }

        /// <summary>
        /// The name of the recurring order program.
        /// </summary>
        public string RecurringOrderProgramName { get; set; }

        public AddLineItemParam Clone()
        {
            var param = (AddLineItemParam)MemberwiseClone();
            return param;
        }
    }
}
