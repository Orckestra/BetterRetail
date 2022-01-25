using Orckestra.Overture.ServiceModel.Orders;
using System.Collections.Generic;
using System.Globalization;

namespace Orckestra.Composer.Cart.Parameters
{
    public class GetRecurringOrderCartsViewModelFromCartsParam
    {
        /// <summary>
        /// List of processed cart
        /// Required
        /// </summary>
        public List<ProcessedCart> Carts { get; set; }

        /// <summary>
        /// The Request Base Url
        /// </summary>
        public string BaseUrl { get; set; }
        /// <summary>
        /// The culture info in which language the data will be returned
        /// Optional
        /// </summary>
        public CultureInfo CultureInfo { get; set; }
    }
}
