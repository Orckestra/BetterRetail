using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orckestra.Composer.MyAccount.Parameters;
using Orckestra.Overture.ServiceModel;

namespace Orckestra.Composer.MyAccount.Repositories
{
    /// <summary>
    /// Repository for interacting with Customer's Addresses
    /// </summary>
    public interface ICustomerAddressRepository
    {
        /// <summary>
        /// Gets a list of all Addresses bound to a Customer account
        /// </summary>
        /// <param name="customerId">The customer's unique identifier</param>
        /// <param name="scopeId">The scope to which those address belong</param>
        /// <returns>
        /// A list of all Addresses bound to a Customer account
        /// </returns>
        Task<List<Address>> GetCustomerAddressesAsync(Guid customerId, string scopeId);

        /// <summary>
        /// Create a new Address for the given customer
        /// </summary>
        /// <param name="customerId">The customerId owner of this new address</param>
        /// <param name="scope">The scope owner of this new address</param>
        /// <param name="address">The address as we want to create it. Not that the address.ID field will be ignored</param>
        /// <returns>
        /// The created Address.
        /// </returns>
        Task<Address> CreateAddressAsync(Guid customerId, Address address, string scope);

        /// <summary>
        /// Update an Address for the given customer
        /// </summary>
        /// <param name="customerId">The customerId owner of this new address</param>
        /// <param name="address">The address to update</param>
        /// <returns>
        /// The updated Address.
        /// </returns>
        Task<Address> UpdateAddressAsync(Guid customerId, Address address);

        /// <summary>
        /// Delete an Address for the given customer
        /// </summary>
        /// <param name="addressId">The unique Id for the address to delete</param>
        Task DeleteAddressAsync(Guid addressId);
    }
}
