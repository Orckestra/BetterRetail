using Orckestra.Overture.ServiceModel.Requests.RecurringOrders;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Cart.Parameters
{
    public class CreateTemplateGroupedShippingAddressParam
    {
        /// <summary>
        /// Cart to map.
        /// </summary>
        public ListOfRecurringOrderLineItems ListOfRecurringOrderLineItems { get; set; }

        /// <summary>
        /// Culture Info for the ViewModel.
        /// </summary>
        public CultureInfo CultureInfo { get; set; }

        /// <summary>
        /// Product Image information
        /// </summary>
        public ProductImageInfo ProductImageInfo { get; set; }

        /// <summary>
        /// The Request Base Url
        /// </summary>
        public string BaseUrl { get; set; }

        /// <summary>
        /// The Payment Methods display name.
        /// </summary>
        public Dictionary<string, string> PaymentMethodDisplayNames { get; set; }

        public Guid CustomerId { get; set; }
        public string ScopeId { get; set; }
    }
}
