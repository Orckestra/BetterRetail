using Orckestra.Overture.ServiceModel.Customers;

namespace Orckestra.Composer.MyAccount.Parameters
{
    /// <summary>
    /// Tuple for returning a Customer and a MyAccountStatus from async methods
    /// </summary>
    public class CustomerAndStatus
    {
        /// <summary>
        /// The Customer concerned by the returning process, or null if none available.
        /// </summary>
        public Customer Customer { get; set; }

        /// <summary>
        /// The result status of the returning process
        /// </summary>
        public MyAccountStatus Status { get; set; }
    }
}
