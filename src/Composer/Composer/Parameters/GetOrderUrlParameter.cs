using System.Globalization;

namespace Orckestra.Composer.Parameters
{
    public class GetOrderUrlParameter: BaseUrlParameter
    {

        /// <summary>
        /// Gets or sets the order identifier.
        /// </summary>
        /// <value>
        /// The order identifier.
        /// </value>
        public string OrderId { get; set; }
    }
}