using System;
using System.Threading.Tasks;
using Orckestra.Overture.ServiceModel;

namespace Orckestra.Composer.Repositories
{
    public interface IAddressRepository
    {
        /// <summary>
        /// Gets a single Address based on it's unique identifier
        /// </summary>
        /// <param name="addressId">The unique Id for the address to find</param>
        /// <returns>
        /// The Address matching the requested ID, or null
        /// </returns>
        Task<Address> GetAddressByIdAsync(Guid addressId);
    }
}
