using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Orckestra.Composer.Enums;
using Orckestra.Composer.Cart.Factory;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Repositories;
using Orckestra.Composer.Cart.ViewModels;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Services;
using Orckestra.Composer.Services.Lookup;
using Orckestra.Composer.Utils;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Cart.Services
{
    /// <summary>
    /// Service for building ViewModel relative to Customers
    /// Customers are users with the ability to buy products
    /// </summary>
    public class CustomerPaymentMethodViewService : ICustomerPaymentMethodViewService
    {
        protected IViewModelMapper ViewModelMapper { get; private set; }
        protected ICustomerPaymentMethodRepository CustomerPaymentMethodRepository { get; private set; }
        protected ICultureService CultureService { get; private set; }
        protected ILookupService LookupService { get; private set; }
        protected ILocalizationProvider LocalizationProvider { get; private set; }
        protected ICustomerViewModelFactory CustomerViewModelFactory { get; private set; }

        public CustomerPaymentMethodViewService(
            IViewModelMapper viewModelMapper,
            ICustomerPaymentMethodRepository customerPaymentMethodRepository,
            ICultureService cultureService,
            ILookupService lookupService,
            ILocalizationProvider localizationProvider,
            ICustomerViewModelFactory customerViewModelFactory)
        {
            if (viewModelMapper == null) { throw new ArgumentNullException("viewModelMapper"); }
            if (customerPaymentMethodRepository == null) { throw new ArgumentNullException("customerRepository"); }
            if (cultureService == null) { throw new ArgumentNullException("cultureService"); }
            if (lookupService == null) { throw new ArgumentNullException("lookupService"); }
            if (localizationProvider == null) { throw new ArgumentNullException("localizationProvider"); }
            if (customerViewModelFactory == null) { throw new ArgumentNullException("customerViewModelFactory"); }

            ViewModelMapper = viewModelMapper;
            CustomerPaymentMethodRepository = customerPaymentMethodRepository;
            CultureService = cultureService;
            LookupService = lookupService;
            LocalizationProvider = localizationProvider;
            CustomerViewModelFactory = customerViewModelFactory;
        }

        public virtual async Task<CustomerPaymentViewModel> GetCustomerPaymentMethodsAsync(GetCustomerPaymentMethodsParam param)
        {
            if (param == null) { throw new ArgumentNullException("param"); }           
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("Scope"), "param"); }
            if (param.ProviderNames == null) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("ProviderName"), "param"); }

            var paymentMethods = await GetAllPaymentMethodsAsync(param).ConfigureAwait(false);
            
            var vm = MapCustomerPaymentViewModel(paymentMethods, param.CultureInfo);
            return vm;
        }

        protected virtual async Task<List<ICustomerPaymentMethodViewModel>> GetAllPaymentMethodsAsync(GetCustomerPaymentMethodsParam param)
        {
            var paymentMethods = await CustomerPaymentMethodRepository.GetCustomerPaymentMethodsAsync(param).ConfigureAwait(false);
            if (paymentMethods == null) { return null; }

            var vm = await MapPaymentMethodsViewModel(paymentMethods, param.CultureInfo).ConfigureAwait(false);
            return vm;
        }

        protected virtual async Task<List<ICustomerPaymentMethodViewModel>> MapPaymentMethodsViewModel(IEnumerable<PaymentMethod> paymentMethods, CultureInfo cultureInfo)
        {
            var paymentMethodViewModels = new List<ICustomerPaymentMethodViewModel>();

            foreach (var method in paymentMethods.Where(x => x.Enabled))
            {
                var methodDisplayNames = await LookupService.GetLookupDisplayNamesAsync(new GetLookupDisplayNamesParam
                {
                    CultureInfo = cultureInfo,
                    LookupType = LookupType.Order,
                    LookupName = "PaymentMethodType",
                }).ConfigureAwait(false);

                var methodViewModel = CustomerViewModelFactory.GetPaymentMethodViewModel(method, methodDisplayNames, cultureInfo);
                if (methodViewModel != null)
                {
                    paymentMethodViewModels.Add(methodViewModel);
                }
            }

            return paymentMethodViewModels.ToList();
        }

        protected virtual CustomerPaymentViewModel MapCustomerPaymentViewModel(List<ICustomerPaymentMethodViewModel> customerMethodViewModels, CultureInfo cultureInfo)
        {        
            var vm = new CustomerPaymentViewModel();
            vm.PaymentMethods = customerMethodViewModels;
            vm.IsLoading = true;

            return vm;
        }
    }
}

