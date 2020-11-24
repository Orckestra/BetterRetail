using System;
using System.Globalization;

namespace Orckestra.Composer.Grocery.Parameters
{
    /// <summary>
    /// Parameters container to set default store
    /// </summary>
    public class SetSelectedStoreParam
    {
        /// <summary>
        /// Culture info for retrieving localized data
        /// </summary>
        public CultureInfo CultureInfo { get; set; }

        public string BaseUrl { get; set; }
        /// <summary>
        /// Unique identifier of a customer, who owns a cart
        /// </summary>
        public Guid CustomerId { get; set; }
        /// <summary>
        /// Depending on authentication status can be used a favorite store of a customer
        /// </summary>
        public bool IsAuthenticated { get; set; }
        /// <summary>
        /// Unique GUID of a store
        /// </summary>
        public Guid StoreId { get; set; }
        /// <summary>
        /// Determines if need to update Preferred store for the customer 
        /// </summary>
        public bool UpdatePreferredStore { get; set; } = true;
    }
}