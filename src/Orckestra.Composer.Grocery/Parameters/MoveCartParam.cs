using Orckestra.Overture.ServiceModel.Orders;
using System;
using System.Globalization;
using StoreServiceModel = Orckestra.Overture.ServiceModel.Customers.Stores.Store;

namespace Orckestra.Composer.Grocery.Parameters
{
	/// <summary>
	/// Container to pass cart moving parameters
	/// </summary>
	public class MoveCartParam
	{
		/// <summary>
		/// Culture info to be used for returning data
		/// </summary>
		public CultureInfo CultureInfo { get; set; }
		/// <summary>
		/// Id of a customer, who owns a cart
		/// </summary>
		public Guid CustomerId { get; set; }
        /// Store of a new cart line items location
        /// </summary>
        public StoreServiceModel NewStore { get; set; }
        /// <summary>
        /// From which scope to move line items
        /// </summary>
        public string ScopeFrom { get; set; }
		/// <summary>
		/// To which scope to move line items
		/// </summary>
		public string ScopeTo { get; set; }
		/// <summary>
		/// Inventory location ID of destination place
		/// </summary>
		public string InventoryLocationId { get; set; }

        /// <summary>
        /// Fulfillment method type need to set up for the new Cart
        /// </summary>
        public FulfillmentMethodType? FulfillementMethodType { get; set; }
	}
}
