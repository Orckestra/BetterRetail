using System.Threading.Tasks;
using Orckestra.Composer.MyAccount.Parameters;
using Orckestra.Composer.MyAccount.ViewModels;

namespace Orckestra.Composer.MyAccount.Services
{
    /// <summary>
    /// Service for dealing with Customers Addresses
    /// </summary>
    public interface ICustomerAddressViewService
    {
        /// <summary>
        /// Create a new Address for the given customer
        /// </summary>
        /// <param name="createAddressParam">Service call params <see cref="CreateAddressParam"/></param>
        /// <returns>
        /// The created Address and a status representing a possible cause of errors.
        /// </returns>
        Task<EditAddressViewModel> CreateAddressAsync(CreateAddressParam createAddressParam);

        /// <summary>
        /// Update an Address for the given customer
        /// </summary>
        /// <param name="editAddressParam">Service call params <see cref="EditAddressParam"/></param>
        /// <returns>
        /// The created Address and a status representing a possible cause of errors.
        /// </returns>
        Task<EditAddressViewModel> UpdateAddressAsync(EditAddressParam editAddressParam);

        /// <summary>
        /// Delete an Address for the given customer
        /// </summary>
        /// <param name="param">Service call params <see cref="DeleteAddressParam"/></param>
        /// <returns>
        /// The status representing a possible cause of errors.
        /// </returns>
        Task<DeleteAddressStatusViewModel> DeleteAddressAsync(DeleteAddressParam param);

        /// <summary>
        /// Gets the <see cref="AddressListViewModel"/> to display a list of addresses in MyAddresses Form or Form result
        /// </summary>
        /// <param name="param">Builder params <see cref="GetAddressListViewModelParam"/></param>
        /// <returns></returns>
        Task<AddressListViewModel> GetAddressListViewModelAsync(GetAddressListViewModelParam param);

        /// <summary>
        /// Gets the <see cref="EditAddressViewModel"/> for the creation of an address for a specified customer id.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<EditAddressViewModel> GetCreateAddressViewModelAsync(GetCreateAddressViewModelAsyncParam param);

        /// <summary>
        /// Gets the <see cref="EditAddressViewModel"/> for the edition of a specified address of a specified customer id.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<EditAddressViewModel> GetEditAddressViewModelAsync(GetEditAddressViewModelAsyncParam param);


        /// <summary>
        /// Set default Address for the given customer
        /// </summary>
        /// <param name="param">Service call params <see cref="SetDefaultAddressParam"/></param>
        /// <returns>
        /// The status representing a possible cause of errors.
        /// </returns>
        Task<SetDefaultAddressStatusViewModel> SetDefaultAddressAsync(SetDefaultAddressParam param);

        /// <summary>
        /// Get Default Address for given customer
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<AddressListItemViewModel> GetDefaultAddressViewModelAsync(GetAddressesForCustomerParam param);
    }
}
