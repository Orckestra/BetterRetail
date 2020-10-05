using System;
using System.Globalization;

namespace Orckestra.Composer.Grocery.Parameters
{
    /// <summary>
    /// Parameters container to get default store
    /// </summary>
    public class GetSelectedStoreParam
    {
        /// <summary>
        /// Base URL of an incoming request
        /// </summary>
        public string BaseUrl { get; set; }
        /// <summary>
        /// Culture info for retrieving localized data
        /// </summary>
        public CultureInfo CultureInfo { get; set; }
        /// <summary>
        /// Unique identifier of a customer, who owns a cart
        /// </summary>
        public Guid CustomerId { get; set; }
        /// <summary>
        /// Depending on authentification status can be used a favorite store of a customer
        /// </summary>
        public bool IsAuthenticated { get; set; }
        /// <summary>
        /// If true, and there are no store in cookies or customer preferences, then a store 
        /// can be got from Grocery settings of a website 
        /// </summary>
        public bool TryGetFromDefaultSettings { get; set; }

        /// <summary>
        /// The id of the requested scope
        /// </summary>
        public string Scope { get; set; }
    }
}