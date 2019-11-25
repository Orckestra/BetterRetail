using System;

namespace Orckestra.Composer.MyAccount.Parameters
{
    /// <summary>
    /// Service call params to Delete an address owned by a Customer
    /// </summary>
    public class DeleteAddressParam
    {
        /// <summary>
        /// (Mandatory)
        /// The unique identifier for the customer to who the address must belong
        /// </summary>
        public Guid CustomerId { get; set; }

        /// <summary>
        /// (Mandatory)
        /// The scope responsible for this request, to which the Customer must belong
        /// </summary>
        public string Scope { get; set; }

        /// <summary>
        /// (Mandatory)
        /// Unique identifier of the address to remove
        /// </summary>
        public Guid AddressId { get; set; }
    }
}
