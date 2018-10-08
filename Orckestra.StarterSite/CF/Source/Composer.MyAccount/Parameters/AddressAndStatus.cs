using Orckestra.Overture.ServiceModel;

namespace Orckestra.Composer.MyAccount.Parameters
{
    /// <summary>
    /// Tuple for returning a Customer Address and a MyAccountStatus from async methods
    /// </summary>
    public class AddressAndStatus
    {
        /// <summary>
        /// The Address concerned by the returning process, or null if none available.
        /// </summary>
        public Address Address { get; set; }

        /// <summary>
        /// The result status of the returning process
        /// </summary>
        public MyAccountStatus Status { get; set; }
    }
}
