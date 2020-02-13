using System;

namespace Orckestra.Composer.MyAccount.Parameters
{
    /// <summary>
    /// Repository call param to retreive a single Customer based on it's unique identifier
    /// </summary>
    public class GetAddressByIdParam 
    {
        /// <summary>
        /// (Mandatory)
        /// The unique Id of the address to look for
        /// </summary>
        public Guid AddressId { get; set; }
    }
}
