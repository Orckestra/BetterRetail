using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Orckestra.Composer.Enums;
using Orckestra.Composer.MyAccount.Parameters;
using Orckestra.Composer.MyAccount.Repositories;
using Orckestra.Composer.MyAccount.ViewModels;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Localization;
using Orckestra.Composer.Services;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel.Customers;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;

namespace Orckestra.Composer.MyAccount.Services
{
    /// <summary>
    /// Service for building ViewModel relative to Customers
    /// Customers are users with the ability to buy products
    /// </summary>
    public class CustomerViewService : ICustomerViewService
    {
        protected IViewModelMapper ViewModelMapper { get; private set; }
        protected ICustomerRepository CustomerRepository { get; private set; }
        protected ICultureService CultureService { get; private set; }
        protected ILocalizationProvider LocalizationProvider { get; private set; }

        public CustomerViewService(
            IViewModelMapper viewModelMapper,
            ICustomerRepository customerRepository,
            ICultureService cultureService,
            ILocalizationProvider localizationProvider)
        {
            ViewModelMapper = viewModelMapper ?? throw new ArgumentNullException(nameof(viewModelMapper));
            CustomerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
            CultureService = cultureService ?? throw new ArgumentNullException(nameof(cultureService));
            LocalizationProvider = localizationProvider ?? throw new ArgumentNullException(nameof(localizationProvider));
        }

        /// <summary>
        /// Gets the <see cref="AccountHeaderViewModel"/> to greet a given Customer.
        /// </summary>
        /// <param name="param">Builder params <see cref="GetAccountHeaderViewModelParam"/></param>
        /// <returns></returns>
        public async virtual Task<AccountHeaderViewModel> GetAccountHeaderViewModelAsync(GetAccountHeaderViewModelParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)), nameof(param)); }
            if (param.CustomerId == Guid.Empty) { throw new ArgumentException(GetMessageOfEmpty(nameof(param.CustomerId)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope)), nameof(param)); }

            var customer = await CustomerRepository.GetCustomerByIdAsync(new GetCustomerByIdParam
            {
                CultureInfo = param.CultureInfo,
                CustomerId = param.CustomerId,
                Scope = param.Scope
            }).ConfigureAwait(false);

            var accountHeaderViewModel = ViewModelMapper.MapTo<AccountHeaderViewModel>(customer, param.CultureInfo);

            return accountHeaderViewModel;
        }

        public virtual async Task<AccountStatusViewModel> GetAccountStatusViewModelAsync(GetAccountStatusViewModelParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)), nameof(param)); }
            if (param.CustomerId == Guid.Empty) { throw new ArgumentException(GetMessageOfEmpty(nameof(param.CustomerId)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope)), nameof(param)); }

            var customer = await CustomerRepository.GetCustomerByIdAsync(new GetCustomerByIdParam
            {
                CultureInfo = param.CultureInfo,
                CustomerId = param.CustomerId,
                Scope = param.Scope
            }).ConfigureAwait(false);

            if (customer == null)
            {
                return null;
            }

            var vm = new AccountStatusViewModel
            {
                Status  = GetAccountStatus(customer.AccountStatus)
            };

            return vm;
        }

        /// <summary>
        /// Gets the <see cref="UpdateAccountViewModel"/> to display the UpdateAccount Form or Form result
        /// </summary>
        /// <param name="param">Builder params <see cref="GetUpdateAccountViewModelParam"/></param>
        /// <returns></returns>
        public async virtual Task<UpdateAccountViewModel> GetUpdateAccountViewModelAsync(GetUpdateAccountViewModelParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)), nameof(param)); }
            if (param.CustomerId == Guid.Empty) { throw new ArgumentException(GetMessageOfEmpty(nameof(param.CustomerId)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope)), nameof(param)); }

            var customer = await CustomerRepository.GetCustomerByIdAsync(new GetCustomerByIdParam
            {
                CultureInfo = param.CultureInfo,
                CustomerId = param.CustomerId,
                Scope = param.Scope
            }).ConfigureAwait(false);

            return GetUpdateAccountViewModel(param, customer);
        }

        /// <summary>
        /// Update the account informations.
        /// </summary>
        /// <param name="param">Builder params <see cref="UpdateAccountParam"/></param>
        /// <returns></returns>
        public async virtual Task<UpdateAccountViewModel> UpdateAccountAsync(UpdateAccountParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)), nameof(param)); }
            if (param.CustomerId == Guid.Empty) { throw new ArgumentException(GetMessageOfEmpty(nameof(param.CustomerId)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Email)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Email)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.FirstName)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.FirstName)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.LastName)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.LastName)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.PreferredLanguage)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.PreferredLanguage)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope)), nameof(param)); }

            var customer = await CustomerRepository.GetCustomerByIdAsync(new GetCustomerByIdParam
            {
                Scope = param.Scope,
                CustomerId = param.CustomerId,
                CultureInfo = param.CultureInfo
            }).ConfigureAwait(false);

            if (customer == null) { return null; }

            UpdateCustomerInfo(customer, param);

            var updateUserParam = new UpdateUserParam
            {
                Customer = customer,
                Scope = param.Scope
            };

            var updatedCustomer = await CustomerRepository.UpdateUserAsync(updateUserParam).ConfigureAwait(false);

            var getUpdateAccountViewModelParam = new GetUpdateAccountViewModelParam
            {
                Status = MyAccountStatus.Success,
                CultureInfo = param.CultureInfo,
                ReturnUrl = param.ReturnUrl
            };

            return GetUpdateAccountViewModel(getUpdateAccountViewModelParam, updatedCustomer);
        }

        protected virtual void UpdateCustomerInfo(Customer customer, UpdateAccountParam updateParam)
        {
            customer.Email = updateParam.Email;
            customer.FirstName = updateParam.FirstName;
            customer.LastName = updateParam.LastName;
            customer.Language = updateParam.PreferredLanguage;
        }

        protected virtual UpdateAccountViewModel GetUpdateAccountViewModel(GetUpdateAccountViewModelParam param, Customer customer)
        {
            var viewModel = ViewModelMapper.MapTo<UpdateAccountViewModel>(customer, param.CultureInfo);

            viewModel.Status = param.Status.HasValue ? param.Status.Value.ToString("G") : string.Empty;
            viewModel.ReturnUrl = param.ReturnUrl;
            viewModel.Languages = GetPreferredLanguageViewModel(param.CultureInfo, customer.Language);

            return viewModel;
        }

        protected virtual List<PreferredLanguageViewModel> GetPreferredLanguageViewModel(CultureInfo currentCulture, string customerLanguage)
        {
            var allcultures = CultureService.GetAllSupportedCultures();

            return (from culture in allcultures
                    let displayName = LocalizationProvider.GetLocalizedString(new GetLocalizedParam
                    {
                        CultureInfo = currentCulture,
                        Category = "General",
                        Key = culture.DisplayName
                    })
                    select new PreferredLanguageViewModel
                    {
                        DisplayName = displayName,
                        IsoCode = culture.Name,
                        IsSelected = customerLanguage == culture.Name
                    }).ToList();
        }

        protected virtual AccountStatusEnum GetAccountStatus(AccountStatus status)
        {
            switch (status)
            {
                case AccountStatus.Active:
                    return AccountStatusEnum.Active;
                case AccountStatus.Inactive:
                    return AccountStatusEnum.Inactive;
                case AccountStatus.RequiresApproval:
                    return AccountStatusEnum.RequiresApproval;
                default:
                    return AccountStatusEnum.Unspecified;
            }
        }
    }
}