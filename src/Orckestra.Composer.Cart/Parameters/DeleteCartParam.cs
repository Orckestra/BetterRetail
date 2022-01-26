using System;
using System.Globalization;

namespace Orckestra.Composer.Cart.Parameters
{
    /// <summary>
    /// Parameters to delete a cart
    /// </summary>
    public class DeleteCartParam
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
        /// Scope to be used during operation
        /// Required
        /// </summary>
        public string Scope { get; set; }
    }
}