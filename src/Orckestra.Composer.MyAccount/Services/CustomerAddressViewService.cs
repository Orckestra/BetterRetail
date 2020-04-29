using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Orckestra.Composer.Cart.Services;
using Orckestra.Composer.Country;
using Orckestra.Composer.MyAccount.Parameters;
using Orckestra.Composer.MyAccount.Repositories;
using Orckestra.Composer.MyAccount.Requests;
using Orckestra.Composer.MyAccount.ViewModels;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Repositories;
using Orckestra.Composer.Services;
using Orckestra.Composer.Utils;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel;
using Orckestra.Overture.ServiceModel.Customers;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;

namespace Orckestra.Composer.MyAccount.Services
{
    /// <summary>
    /// Service for dealing with Addresses
    /// </summary>
    public class CustomerAddressViewService : ICustomerAddressViewService
    {
        protected ICustomerRepository CustomerRepository { get; private set; }
        protected ICustomerAddressRepository CustomerAddressRepository { get; private set; }
        protected IAddressRepository AddressRepository { get; private set; }
        protected IViewModelMapper ViewModelMapper { get; private set; }
        protected IMyAccountUrlProvider MyAccountUrlProvider { get; private set; }
        protected IComposerContext ComposerContext { get; private set; }
        protected ICountryService CountryService { get; private set; }
        protected IRecurringOrderCartsViewService RecurringOrderCartsViewService { get; private set; }

        public CustomerAddressViewService(
            IViewModelMapper viewModelMapper,
            ICustomerRepository customerRepository,
            ICustomerAddressRepository customerAddressRepository,
            IAddressRepository addressRepository,
            IMyAccountUrlProvider myAccountUrlProvider,
            ICountryService countryService,
            IComposerContext composerContext,
            IRecurringOrderCartsViewService recurringOrderCartsViewService)
        {
            ViewModelMapper = viewModelMapper ?? throw new ArgumentNullException(nameof(viewModelMapper));
            CustomerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
            CustomerAddressRepository = customerAddressRepository ?? throw new ArgumentNullException(nameof(customerAddressRepository));
            AddressRepository = addressRepository ?? throw new ArgumentNullException(nameof(addressRepository));
            MyAccountUrlProvider = myAccountUrlProvider ?? throw new ArgumentNullException(nameof(myAccountUrlProvider));
            CountryService = countryService ?? throw new ArgumentNullException(nameof(countryService));
            ComposerContext = composerContext ?? throw new ArgumentNullException(nameof(composerContext));
            RecurringOrderCartsViewService = recurringOrderCartsViewService ?? throw new ArgumentNullException(nameof(recurringOrderCartsViewService));
        }

        /// <summary>
        /// Create a new Address for the given customer
        /// </summary>
        /// <param name="createAddressParam">Service call params <see cref="CreateAddressParam"/></param>
        /// <returns>
        /// The created Address and a status representing a possible cause of errors.
        /// </returns>
        public virtual async Task<EditAddressViewModel> CreateAddressAsync(CreateAddressParam createAddressParam)
        {
            if (createAddressParam == null) { throw new ArgumentNullException(nameof(createAddressParam)); }
            if (createAddressParam.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(createAddressParam.CultureInfo)), nameof(createAddressParam)); }
            if (createAddressParam.CustomerId == Guid.Empty) { throw new ArgumentException(GetMessageOfEmpty(nameof(createAddressParam.CustomerId)), nameof(createAddressParam)); }
            if (createAddressParam.EditAddress == null) { throw new ArgumentException(GetMessageOfNull(nameof(createAddressParam.EditAddress)), nameof(createAddressParam)); }
            if (string.IsNullOrWhiteSpace(createAddressParam.Scope)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(createAddressParam.Scope)), nameof(createAddressParam)); }

            var createdAddress = await CustomerAddressRepository
                .CreateAddressAsync(createAddressParam.CustomerId,
                    ConvertToAddress(new Address(), createAddressParam.EditAddress), createAddressParam.Scope)
                .ConfigureAwait(false);

            if (createAddressParam.EditAddress.IsPreferredShipping || createAddressParam.EditAddress.IsPreferredBilling)
            {
                await SetDefaultAddressAsync(new SetDefaultAddressParam
                {
                    AddressId = createdAddress.Id,
                    CustomerId = createAddressParam.CustomerId,
                    Scope = createAddressParam.Scope

                }).ConfigureAwait(false);
            }

            return await GetEditAddressViewModel(new GetEditAddressViewModelParam
            {
                Address = createdAddress,
                Status = MyAccountStatus.Success,
                ReturnUrl = createAddressParam.ReturnUrl,
                IsUpdating = false,
                CultureInfo = createAddressParam.CultureInfo
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Update an Address for the given customer
        /// </summary>
        /// <param name="editAddressParam">Service call params <see cref="EditAddressParam"/></param>
        /// <returns>
        /// The created Address and a status representing a possible cause of errors.
        /// </returns>
        public virtual async Task<EditAddressViewModel> UpdateAddressAsync(EditAddressParam editAddressParam)
        {
            if (editAddressParam == null) { throw new ArgumentNullException(nameof(editAddressParam)); }
            if (editAddressParam.EditAddress == null) { throw new ArgumentException(GetMessageOfNull(nameof(editAddressParam.EditAddress)), nameof(editAddressParam)); }
            if (editAddressParam.AddressId == Guid.Empty) { throw new ArgumentException(GetMessageOfEmpty(nameof(editAddressParam.AddressId)), nameof(editAddressParam)); }
            if (editAddressParam.CustomerId == Guid.Empty) { throw new ArgumentException(GetMessageOfEmpty(nameof(editAddressParam.CustomerId)), nameof(editAddressParam)); }
            if (editAddressParam.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(editAddressParam.CultureInfo)), nameof(editAddressParam)); }
            if (string.IsNullOrWhiteSpace(editAddressParam.Scope)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(editAddressParam.Scope)), nameof(editAddressParam)); }

            var baseAddress = await AddressRepository.GetAddressByIdAsync(editAddressParam.AddressId).ConfigureAwait(false);

            if (baseAddress == null) { return null; }

            if (!await EnsureAddressBelongsToCustomer(editAddressParam.CustomerId, editAddressParam.Scope, baseAddress.Id).ConfigureAwait(false)) { return null; }

            var updatedAddress = await CustomerAddressRepository
                .UpdateAddressAsync(editAddressParam.CustomerId,
                    ConvertToAddress(baseAddress, editAddressParam.EditAddress))
                    .ConfigureAwait(false);

            if (editAddressParam.EditAddress.IsPreferredShipping || editAddressParam.EditAddress.IsPreferredBilling)
            {
                await SetDefaultAddressAsync(new SetDefaultAddressParam
                {
                    AddressId = editAddressParam.AddressId,
                    CustomerId = editAddressParam.CustomerId,
                    Scope = editAddressParam.Scope

                }).ConfigureAwait(false);
            }

            //Update recurring carts with this address Id
            await RecurringOrderCartsViewService.UpdateRecurringOrderCartsAddressesAsync(new Cart.Parameters.UpdateRecurringOrderCartsAddressesParam
            {
                BaseUrl = "Empty", //Dont need but can't allow empty
                CultureInfo = editAddressParam.CultureInfo,
                CustomerId = editAddressParam.CustomerId,
                ScopeId = editAddressParam.Scope,
                AddressId = updatedAddress.Id,
            }).ConfigureAwait(false);

            return await GetEditAddressViewModel(new GetEditAddressViewModelParam
            {
                Address = updatedAddress,
                Status = MyAccountStatus.Success,
                ReturnUrl = editAddressParam.ReturnUrl,
                IsUpdating = false,
                CultureInfo = editAddressParam.CultureInfo
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete an Address for the given customer
        /// </summary>
        /// <param name="param">Service call params <see cref="DeleteAddressParam"/></param>
        /// <returns>
        /// The status representing a possible cause of errors.
        /// </returns>
        public virtual async Task<DeleteAddressStatusViewModel> DeleteAddressAsync(DeleteAddressParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (param.CustomerId == Guid.Empty) { throw new ArgumentException(GetMessageOfEmpty(nameof(param.CustomerId)), nameof(param)); }
            if (param.AddressId == Guid.Empty) { throw new ArgumentException(GetMessageOfEmpty(nameof(param.AddressId)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope)), nameof(param)); }

            if (!await EnsureAddressBelongsToCustomer(param.CustomerId, param.Scope, param.AddressId).ConfigureAwait(false)) { return null; }

            await CustomerAddressRepository.DeleteAddressAsync(param.AddressId).ConfigureAwait(false);

            return new DeleteAddressStatusViewModel();
        }

        /// <summary>
        /// Gets the <see cref="AddressListViewModel"/> to display a list of addresses in MyAddresses Form or Form result
        /// </summary>
        /// <param name="param">Builder params <see cref="GetAddressListViewModelParam"/></param>
        /// <returns></returns>
        public virtual async Task<AddressListViewModel> GetAddressListViewModelAsync(GetAddressListViewModelParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)), nameof(param)); }
            if (param.CustomerId == Guid.Empty) { throw new ArgumentException(GetMessageOfEmpty(nameof(param.CustomerId)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.CountryCode)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.CountryCode)), nameof(param)); }

            var customer = await CustomerRepository.GetCustomerByIdAsync(new GetCustomerByIdParam
            {
                CultureInfo = param.CultureInfo,
                CustomerId = param.CustomerId,
                Scope = param.Scope,
                IncludeAddresses = true,
            }).ConfigureAwait(false);

            IList<Address> addresses = customer.Addresses 
                ?? await CustomerAddressRepository.GetCustomerAddressesAsync(customer.Id, param.Scope).ConfigureAwait(false);

            var regions = await CountryService.RetrieveRegionsAsync(new RetrieveCountryParam
            {
                IsoCode = param.CountryCode,
                CultureInfo = param.CultureInfo,
            }).ConfigureAwait(false);

            var viewModel = GetAddressListViewModel(param, customer, addresses, regions);

            return viewModel;
        }

        /// <summary>
        /// Gets the <see cref="AddressListViewModel"/> to display a list of addresses in MyAddresses Form or Form result
        /// </summary>
        /// <param name="param"></param>
        /// <param name="customer"></param>
        /// <param name="addresses"></param>
        /// <param name="regions"></param>
        /// <returns></returns>
        protected virtual AddressListViewModel GetAddressListViewModel(
            GetAddressListViewModelParam param,
            Customer customer,
            IEnumerable<Address> addresses,
            IEnumerable<RegionViewModel> regions)
        {
            var viewModel = ViewModelMapper.MapTo<AddressListViewModel>(customer, param.CultureInfo);

            viewModel.AddAddressUrl = param.AddAddressUrl;

            viewModel.Addresses = addresses.Select(address =>
            {
                var addressVm = ViewModelMapper.MapTo<AddressListItemViewModel>(address, param.CultureInfo);
                if (!string.IsNullOrWhiteSpace(param.EditAddressBaseUrl))
                {
                    var editUrlWithParam = UrlFormatter.AppendQueryString(param.EditAddressBaseUrl,
                        new NameValueCollection
                        {
                             {"addressId", address.Id.ToString()}
                        });
                    addressVm.UpdateAddressUrl = editUrlWithParam;
                }

                var region = regions.FirstOrDefault(x => x.IsoCode == address.RegionCode);
                addressVm.RegionName = region != null ? region.Name : string.Empty;

                return addressVm;
            }).ToList();

            return viewModel;
        }

        /// <summary>
        /// Gets the <see cref="EditAddressViewModel"/> for the edition of a specified address of a specified customer id.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual async Task<EditAddressViewModel> GetEditAddressViewModelAsync(GetEditAddressViewModelAsyncParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)), nameof(param)); }
            if (param.CustomerId == Guid.Empty) { throw new ArgumentException(GetMessageOfEmpty(nameof(param.CustomerId)), nameof(param)); }
            if (param.AddressId == Guid.Empty) { throw new ArgumentException(GetMessageOfEmpty(nameof(param.AddressId)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope)), nameof(param)); }

            if (!await EnsureAddressBelongsToCustomer(param.CustomerId, param.Scope, param.AddressId).ConfigureAwait(false)) { return null; }

            var address = await AddressRepository.GetAddressByIdAsync(param.AddressId);

            return await GetEditAddressViewModel(new GetEditAddressViewModelParam
            {
                Address = address,
                IsUpdating = true,
                CultureInfo = param.CultureInfo
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the <see cref="EditAddressViewModel"/> for the creation of an address for a specified customer id.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual async Task<EditAddressViewModel> GetCreateAddressViewModelAsync(GetCreateAddressViewModelAsyncParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)), nameof(param)); }
            if (param.CustomerId == Guid.Empty) { throw new ArgumentException(GetMessageOfEmpty(nameof(param.CustomerId)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.CountryCode)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.CountryCode)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope)), nameof(param)); }

            var customer = await CustomerRepository.GetCustomerByIdAsync(new GetCustomerByIdParam
            {
                CustomerId = param.CustomerId,
                CultureInfo = param.CultureInfo,
                Scope = param.Scope
            }).ConfigureAwait(false);

            var newAddressWithDefaultValues = new Address
            {
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                PhoneNumber = customer.PhoneNumber,
                PhoneExtension = customer.PhoneExtension,
                Email = customer.Email,
                CountryCode = param.CountryCode
            };

            return await GetEditAddressViewModel(new GetEditAddressViewModelParam
            {
                Address = newAddressWithDefaultValues,
                IsUpdating = false,
                CultureInfo = param.CultureInfo
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Set default Address for the given customer
        /// </summary>
        /// <param name="param">Service call params <see cref="SetDefaultAddressParam"/></param>
        /// <returns>
        /// The status representing a possible cause of errors.
        /// </returns>
        public virtual async Task<SetDefaultAddressStatusViewModel> SetDefaultAddressAsync(SetDefaultAddressParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (param.CustomerId == Guid.Empty) { throw new ArgumentException(GetMessageOfEmpty(nameof(param.CustomerId)), nameof(param)); }
            if (param.AddressId == Guid.Empty) { throw new ArgumentException(GetMessageOfEmpty(nameof(param.AddressId)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope)), nameof(param)); }

            var address = await AddressRepository.GetAddressByIdAsync(param.AddressId).ConfigureAwait(false);

            if (address == null) { return null; }

            if (!await EnsureAddressBelongsToCustomer(param.CustomerId, param.Scope, address.Id).ConfigureAwait(false)) { return null; }

            address.IsPreferredShipping = true;
            address.IsPreferredBilling = true;

            await CustomerAddressRepository.UpdateAddressAsync(param.CustomerId, address).ConfigureAwait(false);

            return new SetDefaultAddressStatusViewModel();
        }

        /// <summary>
        /// Get Default Address for given customer
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual async Task<AddressListItemViewModel> GetDefaultAddressViewModelAsync(GetAddressesForCustomerParam param)
        {
            var addresses = await CustomerAddressRepository.GetCustomerAddressesAsync(
                param.CustomerId, param.Scope).ConfigureAwait(false);

            if (addresses == null) { return null; }

            var addressVm = new AddressListItemViewModel();
            var defaultAddress = addresses.Find(a => a.IsPreferredShipping);
            if (defaultAddress != null)
            {
                addressVm = ViewModelMapper.MapTo<AddressListItemViewModel>(defaultAddress, param.CultureInfo);
            }

            return addressVm;
        }

        protected virtual async Task UpdateAddressAsync(Guid customerId, Guid otherAddressId, Guid addressId)
        {
            if (otherAddressId == addressId) { return; }

            var otherAddress = await AddressRepository.GetAddressByIdAsync(otherAddressId).ConfigureAwait(false);

            if (otherAddress != null && otherAddress.IsPreferredShipping)
            {
                otherAddress.IsPreferredShipping = false;
                await CustomerAddressRepository.UpdateAddressAsync(customerId, otherAddress).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Gets the <see cref="EditAddressViewModel"/> to display the edit address From or Form result
        /// </summary>
        /// <param name="param">Builder params <see cref="GetEditAddressViewModelParam"/></param>
        /// <returns></returns>
        protected virtual async Task<EditAddressViewModel> GetEditAddressViewModel(GetEditAddressViewModelParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)), nameof(param)); }

            var countryParam = new RetrieveCountryParam
            {
                CultureInfo = param.CultureInfo ?? ComposerContext.CultureInfo,
                IsoCode = ComposerContext.CountryCode
            };

            var viewModel = ViewModelMapper.MapTo<EditAddressViewModel>(param.Address, param.CultureInfo) ?? new EditAddressViewModel();

            viewModel.Status = param.Status.HasValue ? param.Status.Value.ToString() : string.Empty;
            viewModel.ReturnUrl = param.ReturnUrl;
            viewModel.IsUpdating = param.Address != null && param.IsUpdating;

            var country = await CountryService.RetrieveCountryAsync(countryParam).ConfigureAwait(false);
            if (country != null)
            {
                viewModel.PostalCodeRegex = country.PostalCodeRegex;
                viewModel.PhoneRegex = country.PhoneRegex;
            }

            return viewModel;
        }

        /// <summary>
        /// Convert a request to <see cref="Address"/>, including mandatory, optional and additionnal properties
        /// </summary>
        /// <param name="baseAddress">
        /// The fully populated address to update, be warned that incomplete update form will wipe data,
        /// it is best practice to load from repository before saving. The base adresse is the one loaded from the repository
        /// </param>
        /// <param name="request"></param>
        /// <returns></returns>
        protected virtual Address ConvertToAddress(Address baseAddress, EditAddressRequest request)
        {
            baseAddress.AddressName = request.AddressName;
            baseAddress.City = request.City;
            baseAddress.CountryCode = request.CountryCode;
            baseAddress.FirstName = request.FirstName;
            baseAddress.LastName = request.LastName;
            baseAddress.Line1 = request.Line1;
            baseAddress.Line2 = request.Line2;
            baseAddress.PhoneNumber = request.PhoneNumber;
            baseAddress.PostalCode = request.PostalCode;
            baseAddress.RegionCode = request.RegionCode;
            baseAddress.IsPreferredShipping = request.IsPreferredShipping;
            baseAddress.IsPreferredBilling = request.IsPreferredBilling;

            baseAddress.PropertyBag = baseAddress.PropertyBag ?? new PropertyBag();

            foreach (var additionnalProperty in request.AdditionnalProperties)
            {
                baseAddress.PropertyBag[additionnalProperty.Key] = additionnalProperty.Value;
            }

            return baseAddress;
        }

        //TODO is there any better way to ensure that only the Customer (or Company Owner) can edit this guy's information
        protected virtual async Task<bool> EnsureAddressBelongsToCustomer(Guid customerId, string scope, Guid addressId)
        {
            var customer = await CustomerRepository.GetCustomerByIdAsync(new GetCustomerByIdParam
            {
                CustomerId = customerId,
                CultureInfo = CultureInfo.InvariantCulture,
                Scope = scope
            }).ConfigureAwait(false);

            var hasId = customer.AddressIds.Contains(addressId);
            return hasId;
        }
    }
}