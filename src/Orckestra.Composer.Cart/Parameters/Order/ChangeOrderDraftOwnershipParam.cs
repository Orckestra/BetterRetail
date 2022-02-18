using System;

namespace Orckestra.Composer.Cart.Parameters.Order
{
    public class ChangeOrderDraftOwnershipParam
    {
        /// <summary>
        /// The id of the requested scope
        /// </summary>
        public string Scope { get; set; }

        /// <summary>
        /// The unique identifier of the customer.
        /// </summary>
        public Guid CustomerId { get; set; }

        public Guid OrderId {get;set;}

        /// <summary>
        /// A value indicating whether pending order draft modifications must be reverted to the original order cart or not.
        /// </summary>
        public bool RevertPendingChanges { get; set; }

        /// <summary>
        /// The culture name in which language the data will be returned.
        /// </summary>
        public string CultureName { get; set; }

    }
}
