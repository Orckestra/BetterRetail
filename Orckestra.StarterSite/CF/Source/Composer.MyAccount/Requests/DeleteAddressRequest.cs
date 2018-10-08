using System.Runtime.Serialization;

namespace Orckestra.Composer.MyAccount.Requests
{
    /// <summary>
    /// WebApi Request to Delete one of the address for the currently logged user
    /// </summary>
    [DataContract]
    public class DeleteAddressRequest
    {
        /// <summary>
        /// (Mandatory)
        /// The unique identifier for the address to delete
        /// </summary>
        [DataMember(IsRequired = true)]
        public string AddressId { get; set; }

        /// <summary>
        /// (Optional)
        /// ReturnUrl to be used on client side to redirect after deleting the address
        /// </summary>
        [DataMember(IsRequired = false)]
        public string ReturnUrl { get; set; }
    }
}