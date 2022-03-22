using System.Globalization;

namespace Orckestra.Composer.Grocery.Parameters
{
    public class SetOrderFulfillmentParam
    {
        /// <summary>
        /// Gets or sets the order number
        /// </summary>
        public string OrderNumber { get; set; }

        /// <summary>
        /// Gets or sets the scope.
        /// </summary>
        /// <value>
        /// The scope.
        /// </value>
        public string Scope { get; set; }

        /// <summary>
        /// The culture information.
        /// </summary>
        public CultureInfo CultureInfo { get; set; }

         /// <summary>
        /// The base Url.
        /// </summary>
        public string BaseUrl { get; set; }
    }
}
