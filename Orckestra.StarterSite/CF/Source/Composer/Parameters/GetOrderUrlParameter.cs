using System.Globalization;

namespace Orckestra.Composer.Parameters
{
    public class GetOrderUrlParameter
    {
        /// <summary>
        /// Gets or sets the culture information.
        /// </summary>
        /// <value>
        /// The culture information.
        /// </value>
        public CultureInfo CultureInfo { get; set; }

        /// <summary>
        /// Gets or sets the order identifier.
        /// </summary>
        /// <value>
        /// The order identifier.
        /// </value>
        public string OrderId { get; set; }
    }
}