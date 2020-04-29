using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Orckestra.Composer.Cart.Extensions;
using Orckestra.Composer.Cart.Factory;
using Orckestra.Composer.Cart.Helper;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Providers.Payment;
using Orckestra.Composer.Cart.Repositories;
using Orckestra.Composer.Cart.ViewModels;
using Orckestra.Composer.Configuration;
using Orckestra.Composer.Enums;
using Orckestra.Composer.Logging;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Requests;
using Orckestra.Composer.Services;
using Orckestra.Composer.Services.Lookup;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.Providers;
using Orckestra.Overture.ServiceModel.Orders;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;

namespace Orckestra.Composer.Cart.Services
{
    /// <summary>
    /// Service for dealing with Payment Methods.
    /// </summary>
    public class PaymentViewService : IPaymentViewService
    {
        protected IPaymentRepository PaymentRepository { get; private set; }
        protected ICartViewModelFactory CartViewModelFactory { get; private set; }
        protected ICartRepository CartRepository { get; private set; }
        protected ILookupService LookupService { get; private set; }
        protected IViewModelMapper ViewModelMapper { get; private set; }
        protected IPaymentProviderFactory PaymentProviderFactory { get; private set; }
        protected IRecurringOrderTemplatesViewService RecurringOrderTemplatesViewService { get; private set; }
        protected IRecurringOrderCartsViewService RecurringOrderCartsViewService { get; private set; }
        private static ILog Log = LogProvider.GetCurrentClassLogger();
        protected IRecurringOrdersSettings RecurringOrdersSettings { get; private set; }

        public PaymentViewService(IPaymentRepository paymentRepository, ICartViewModelFactory cartViewModelFactory, ICartRepository cartRepository,
         ILookupService lookupService, IViewModelMapper viewModelMapper, IPaymentProviderFactory paymentProviderFactory,
         IRecurringOrderTemplatesViewService recurringOrderTemplatesViewService,
         IRecurringOrderCartsViewService recurringOrderCartsViewService,
         IRecurringOrdersSettings recurringOrdersSettings
         )
        {
            PaymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
            CartViewModelFactory = cartViewModelFactory ?? throw new ArgumentNullException(nameof(cartViewModelFactory));
            CartRepository = cartRepository ?? throw new ArgumentNullException(nameof(cartRepository));
            LookupService = lookupService ?? throw new ArgumentNullException(nameof(lookupService));
            ViewModelMapper = viewModelMapper ?? throw new ArgumentNullException(nameof(viewModelMapper));
            PaymentProviderFactory = paymentProviderFactory ?? throw new ArgumentNullException(nameof(paymentProviderFactory));
            RecurringOrderTemplatesViewService = recurringOrderTemplatesViewService ?? throw new ArgumentNullException(nameof(recurringOrderTemplatesViewService));
            RecurringOrderCartsViewService = recurringOrderCartsViewService ?? throw new ArgumentNullException(nameof(recurringOrderCartsViewService));
            RecurringOrdersSettings = recurringOrdersSettings ?? throw new ArgumentNullException(nameof(recurringOrdersSettings));
        }

        /// <summary>
        /// Gets an enumeration of all available payment providers.
        /// </summary>
        /// <param name="param">Parameters used to retrieve the payment providers.</param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<PaymentProviderViewModel>> GetPaymentProvidersAsync(GetPaymentProvidersParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)), nameof(param)); }
            if (param.Scope == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.Scope)), nameof(param)); }

            var providers = PaymentProviderFactory.ResolveAllProviders();
            providers = await FilterAvailablePaymentProviders(param, providers).ConfigureAwait(false);

            var viewModels = new List<PaymentProviderViewModel>();

            foreach (var paymentProvider in providers)
            {
                var vm = MapPaymentProviderViewModel(paymentProvider, param.CultureInfo);
                viewModels.Add(vm);
            }

            return viewModels;
        }

        protected virtual async Task<IEnumerable<IPaymentProvider>> FilterAvailablePaymentProviders(GetPaymentProvidersParam param, IEnumerable<IPaymentProvider> providers)
        {
            var availablePaymentProvidersTask = PaymentRepository.GetPaymentProviders(param.Scope).ConfigureAwait(false);
            var availableProvidersTask = PaymentRepository.GetProviders(param.Scope, ProviderType.Payment);

            var availablePaymentProviders = await availablePaymentProvidersTask;
            var availableProviders = await availableProvidersTask;

            var availableProvidersDic = new Dictionary<string, Overture.ServiceModel.Providers.Provider>(StringComparer.OrdinalIgnoreCase);
            foreach (var el in availableProviders)
            {
                if (!el.IsActive || availableProvidersDic.ContainsKey(el.ImplementationTypeName)) { continue; }

                availableProvidersDic.Add(el.ImplementationTypeName, el);
            }

            var availablePaymentProvidersDic = new Dictionary<Guid, Overture.ServiceModel.Providers.PaymentProviderInfo>();
            foreach (var el in availablePaymentProviders)
            {
                if (availablePaymentProvidersDic.ContainsKey(el.Id)) { continue; } 
                
                availablePaymentProvidersDic.Add(el.Id, el);
            }

            var result = new List<IPaymentProvider>();

            foreach (var provider in providers)
            {
                availableProvidersDic.TryGetValue(provider.ProviderType, out var availableProvider);
                if (availableProvider == null) { continue; }

                availablePaymentProvidersDic.TryGetValue(availableProvider.Id, out var availablePaymentProvider);
                if (availablePaymentProvider == null) { continue; }

                var supportedCultureIds = availablePaymentProvider.SupportedCultureIds.Split(',').Select(c => c.Trim());

                if (!supportedCultureIds.Any(c => c.Equals(param.CultureInfo.Name, StringComparison.OrdinalIgnoreCase))) { continue; }

                result.Add(provider);
            }

            return result;
        }

        protected virtual PaymentProviderViewModel MapPaymentProviderViewModel(IPaymentProvider paymentProvider, CultureInfo cultureInfo)
        {
            return ViewModelMapper.MapTo<PaymentProviderViewModel>(paymentProvider, cultureInfo);
        }

        /// <summary>
        /// Get the Active Payment methods available for a cart.
        /// </summary>
        /// <param name="param">GetPaymentMethodsParam</param>
        /// <returns>A List of PaymentMethodViewModel</returns>
        public virtual async Task<SingleCheckoutPaymentViewModel> GetSingleCheckoutPaymentAsync(GetPaymentMethodsParam param)
        {
            var providers = await GetPaymentProvidersAsync(new GetPaymentProvidersParam
            {
                Scope = param.Scope,
                CultureInfo = param.CultureInfo
            }).ConfigureAwait(false);

            param.ProviderNames = providers.Select(p => p.ProviderName).ToList();

            var results = await GetPaymentMethodsAsync(param);

            if (results.ActivePaymentViewModel == null)
            {
                results.ActivePaymentViewModel = new ActivePaymentViewModel()
                {
                    Id = results.PaymentId ?? default
                };
            }

            return new SingleCheckoutPaymentViewModel()
            {
                PaymentMethods = results.PaymentMethods,
                PaymentProviders = providers.ToList(),
                ActivePaymentViewModel = results.ActivePaymentViewModel
            };
        }

        /// <summary>
        /// Get the Active Payment methods available for a cart.
        /// </summary>
        /// <param name="param">GetPaymentMethodsParam</param>
        /// <returns>A List of PaymentMethodViewModel</returns>
        public virtual async Task<CheckoutPaymentViewModel> GetPaymentMethodsAsync(GetPaymentMethodsParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.CartName)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.CartName)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope)), nameof(param)); }
            if (param.ProviderNames == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.ProviderNames)), nameof(param)); }

            var paymentMethods = await GetAllPaymentMethodsAsync(param).ConfigureAwait(false);
            var cart = await CartRepository.GetCartAsync(new GetCartParam
            {
                CartName = param.CartName,
                CustomerId = param.CustomerId,
                CultureInfo = param.CultureInfo,
                Scope = param.Scope
            });

            var hasRecurring = false;
            if (RecurringOrdersSettings.Enabled)
            {
                hasRecurring = RecurringOrderCartHelper.IsCartContainsRecurringOrderItems(cart);
            }

            if (hasRecurring)
            {
                var supported = CartConfiguration.SupportedRecurringOrderPaymentMethodTypes;
                paymentMethods = paymentMethods.Where(p => supported.Any(s => s.ToString() == p.PaymentType)).ToList();
            }

            var vm = await MapCheckoutPaymentViewModel(cart, paymentMethods, param.CultureInfo, param.IsAuthenticated);
            return vm;
        }

        protected virtual async Task<List<IPaymentMethodViewModel>> GetAllPaymentMethodsAsync(GetPaymentMethodsParam param)
        {
            var paymentMethods = await PaymentRepository.GetPaymentMethodsAsync(param).ConfigureAwait(false);
            if (paymentMethods == null) { return null; }

            var vm = await MapPaymentMethodsViewModel(paymentMethods, param.CultureInfo, param.CustomerId, param.Scope);
            return vm;
        }

        protected virtual async Task<List<IPaymentMethodViewModel>> MapPaymentMethodsViewModel(IEnumerable<PaymentMethod> paymentMethods,
            CultureInfo cultureInfo,
            Guid customerId,
            string scope)
        {
            var paymentMethodViewModels = new List<IPaymentMethodViewModel>();

            foreach (var method in paymentMethods.Where(x => x.Enabled))
            {
                var methodDisplayNames = await LookupService.GetLookupDisplayNamesAsync(new GetLookupDisplayNamesParam
                {
                    CultureInfo = cultureInfo,
                    LookupType = LookupType.Order,
                    LookupName = "PaymentMethodType",
                }).ConfigureAwait(false);

                var methodViewModel = CartViewModelFactory.GetPaymentMethodViewModel(method, methodDisplayNames, cultureInfo);
                if (methodViewModel != null)
                {
                    var paymentProvider = ObtainPaymentProvider(methodViewModel.PaymentProviderName);
                    methodViewModel.PaymentProviderType = paymentProvider?.ProviderType;

                    await IsSavedCardUsedInRecurringOrders(methodViewModel, cultureInfo, customerId, scope);

                    IsCreditCardPaymentMethod(methodViewModel);

                    paymentMethodViewModels.Add(methodViewModel);
                }
            }

            return paymentMethodViewModels;
        }

        protected virtual void IsCreditCardPaymentMethod(IPaymentMethodViewModel paymentMethod)
        {
            paymentMethod.IsCreditCardPaymentMethod = paymentMethod.PaymentProviderType == "MonerisCanadaPaymentProvider";
        }

        protected virtual async Task<IPaymentMethodViewModel> IsSavedCardUsedInRecurringOrders(
            IPaymentMethodViewModel methodViewModel,
            CultureInfo cultureInfo,
            Guid customerId,
            string scope)
        {
            if (methodViewModel is SavedCreditCardPaymentMethodViewModel vm)
            {
                vm.IsUsedInRecurringOrders = await RecurringOrderTemplatesViewService.GetIsPaymentMethodUsedInRecurringOrders(new GetIsPaymentMethodUsedInRecurringOrdersRequest
                {
                    CultureInfo = cultureInfo,
                    CustomerId = customerId,
                    PaymentMethodId = methodViewModel.Id.ToString(),
                    ScopeId = scope
                }).ConfigureAwait(false);

                return vm;
            }
            return methodViewModel;
        }

        protected virtual async Task<CheckoutPaymentViewModel> MapCheckoutPaymentViewModel(
            Overture.ServiceModel.Orders.Cart cart, 
            List<IPaymentMethodViewModel> paymentMethodViewModels, 
            CultureInfo cultureInfo, 
            bool isAuthenticated)
        {
            var payment = GetActivePayment(cart);

            if (payment == null) { return null; }

            var paymentId = payment.Id;

            IPaymentMethodViewModel activePaymentMethodVm = null;
            ActivePaymentViewModel activePaymentVm = null;

            if (payment.PaymentMethod != null)
            {
                activePaymentMethodVm = paymentMethodViewModels.FirstOrDefault(pm =>
                    pm.Id == payment.PaymentMethod.Id &&
                    pm.PaymentProviderName == payment.PaymentMethod.PaymentProviderName &&
                    pm.IsValid
                );
            }

            if (activePaymentMethodVm != null)
            {
                var provider = PaymentProviderFactory.ResolveProvider(payment.PaymentMethod.PaymentProviderName);

                activePaymentVm = GetActivePaymentViewModel(new GetActivePaymentViewModelParam
                {
                    Cart = cart,
                    CultureInfo = cultureInfo,
                    PaymentProvider = provider,
                    IsAuthenticated = isAuthenticated
                });
            }
            else
            {
                // If active payment is set to soemthing that doesn't exists anymore
                // Select the first payment method available...
                var validPaymentMethods = paymentMethodViewModels.Where(pm => pm.IsValid);
                activePaymentMethodVm = validPaymentMethods.FirstOrDefault(pm => pm.Default) ?? validPaymentMethods.FirstOrDefault();

                activePaymentVm = await UpdateActivePaymentMethodAsync(new UpdatePaymentMethodParam
                {
                    CartName = cart.Name,
                    CultureInfo = cultureInfo,
                    CustomerId = cart.CustomerId,
                    IsAuthenticated = isAuthenticated,
                    Scope = cart.ScopeId,
                    PaymentId = paymentId,
                    PaymentMethodId = activePaymentMethodVm.Id,
                    PaymentProviderName = activePaymentMethodVm.PaymentProviderName,
                }).ConfigureAwait(false);

                if (activePaymentVm != null)
                {
                    paymentId = activePaymentVm.Id;
                }
            }

            if (activePaymentMethodVm != null)
            {
                activePaymentMethodVm.IsSelected = true;
            }

            var vm = new CheckoutPaymentViewModel
            {
                PaymentId = paymentId,
                PaymentMethods = paymentMethodViewModels,
                ActivePaymentViewModel = activePaymentVm
            };

            return vm;
        }

        /// <summary>
        /// Updates the payment method for a cart and initializes it.
        /// </summary>
        /// <returns>ViewModel representing the initialized Active Payment.</returns>
        public virtual async Task<CheckoutPaymentViewModel> UpdatePaymentMethodAsync(UpdatePaymentMethodParam param)
        {
            await UpdateActivePaymentMethodAsync(param).ConfigureAwait(false);

            var vm = await GetPaymentMethodsAsync(new GetPaymentMethodsParam
            {
                CartName = param.CartName,
                CultureInfo = param.CultureInfo,
                CustomerId = param.CustomerId,
                IsAuthenticated = param.IsAuthenticated,
                ProviderNames = param.ProviderNames,
                Scope = param.Scope,
            }).ConfigureAwait(false);

            return vm;
        }

        public virtual async Task<ActivePaymentViewModel> UpdateActivePaymentMethodAsync(UpdatePaymentMethodParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.CartName)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.CartName)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.PaymentProviderName)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.PaymentProviderName)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope)), nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)), nameof(param)); }
            if (param.CustomerId == Guid.Empty) { throw new ArgumentException(GetMessageOfEmpty(nameof(param.CustomerId)), nameof(param)); }
            if (param.PaymentId == Guid.Empty) { throw new ArgumentException(GetMessageOfEmpty(nameof(param.PaymentId)), nameof(param)); }
            if (param.PaymentMethodId == Guid.Empty) { throw new ArgumentException(GetMessageOfEmpty(nameof(param.PaymentMethodId)), nameof(param)); }

            var payments = await GetCartPaymentsAsync(param).ConfigureAwait(false);
            var activePayment = payments.GetPayment(param.PaymentId);

            if (activePayment.ShouldInvokePrePaymentSwitch())
            {
                activePayment = await PreparePaymentSwitch(param, activePayment);
                param.PaymentId = activePayment.Id;
            }

            var validatePaymentMethodParam = new ValidatePaymentMethodParam()
            {
                CartName = param.CartName,
                CultureInfo = param.CultureInfo,
                CustomerId = param.CustomerId,
                IsAuthenticated = param.IsAuthenticated,
                ProviderName = param.PaymentProviderName,
                Scope = param.Scope,
                PaymentMethodId = param.PaymentMethodId
            };

            var isPaymentMethodValid = await ValidatePaymentMethod(validatePaymentMethodParam);

            if (!isPaymentMethodValid)
            {
                throw new Exception($"Payment method for provider name /'{param.PaymentProviderName}/' not valid. Credit card has probably expired.");
            }

            Overture.ServiceModel.Orders.Cart cart = await PaymentRepository.UpdatePaymentMethodAsync(param);

            var initParam = BuildInitializePaymentParam(cart, param);
            var paymentProvider = ObtainPaymentProvider(param.PaymentProviderName);
            cart = await paymentProvider.InitializePaymentAsync(cart, initParam);

            var activePaymentVm = GetActivePaymentViewModel(new GetActivePaymentViewModelParam
            {
                Cart = cart,
                CultureInfo = param.CultureInfo,
                PaymentProvider = paymentProvider,
                IsAuthenticated = param.IsAuthenticated
            });

            return activePaymentVm;
        }

        protected virtual async Task<bool> ValidatePaymentMethod(ValidatePaymentMethodParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.CartName)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.CartName)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.ProviderName)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.ProviderName)), nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)), nameof(param)); }
            if (param.CustomerId == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.CustomerId)), nameof(param)); }
            if (param.PaymentMethodId == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.PaymentMethodId)), nameof(param)); }

            var getPaymentMethodParam = new GetPaymentMethodsParam()
            {
                CartName = param.CartName,
                Scope = param.Scope,
                CultureInfo = param.CultureInfo,
                CustomerId = param.CustomerId,
                IsAuthenticated = param.IsAuthenticated,
                ProviderNames = new List<string>()
                {
                    param.ProviderName
                }
            };

            var paymentMethods = await GetAllPaymentMethodsAsync(getPaymentMethodParam).ConfigureAwait(false);
            var paymentMethod = paymentMethods?.Find(x => x.Id == param.PaymentMethodId);
            return paymentMethod?.IsValid ?? throw new Exception($"Payment method for provider name /'{param.ProviderName}/' not found."); ;
        }

        public virtual async Task<ActivePaymentViewModel> GetActivePayment(GetActivePaymentParam param)
        {
            if (param == null) throw new ArgumentNullException(nameof(param));
            if (param.CustomerId == Guid.Empty) throw new ArgumentException(GetMessageOfEmpty(nameof(param.CustomerId)), nameof(param));
            if (param.CultureInfo == null) throw new ArgumentNullException(GetMessageOfNull(nameof(param.CultureInfo)), nameof(param));
            if (string.IsNullOrWhiteSpace(param.CartName)) throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.CartName)), nameof(param));
            if (string.IsNullOrWhiteSpace(param.Scope)) throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope)), nameof(param));

            var cart = await CartRepository.GetCartAsync(new GetCartParam
            {
                CartName = param.CartName,
                CustomerId = param.CustomerId,
                CultureInfo = param.CultureInfo,
                Scope = param.Scope
            });

            var payment = GetActivePayment(cart);

            if (payment == null) return null;

            return GetActivePaymentViewModel(new GetActivePaymentViewModelParam
            {
                CultureInfo = param.CultureInfo,
                Cart = cart,
                PaymentProvider = PaymentProviderFactory.ResolveProvider(payment.PaymentMethod.PaymentProviderName),
                IsAuthenticated = param.IsAuthenticated
            });
        }

        public virtual Task RemovePaymentMethodAsync(RemovePaymentMethodParam param)
        {
            if (param == null) throw new ArgumentNullException(nameof(param));
            if (param.PaymentMethodId == Guid.Empty) throw new ArgumentException(GetMessageOfEmpty(nameof(param.PaymentMethodId)), nameof(param));
            return PaymentRepository.RemovePaymentMethodAsync(param);
        }

        protected virtual async Task<Payment> PreparePaymentSwitch(UpdatePaymentMethodParam param, Payment activePayment)
        {
            //TODO: Remove for now, but we should void (Bug with Moneris when payment is PendingVerification).
            await PaymentRepository.RemovePaymentAsync(new VoidOrRemovePaymentParam
            {
                CartName = param.CartName,
                CultureInfo = param.CultureInfo,
                CustomerId = param.CustomerId,
                PaymentId = activePayment.Id,
                Scope = param.Scope
            }).ConfigureAwait(false);

            var cart = await CartRepository.AddPaymentAsync(new AddPaymentParam
            {
                BillingAddress = activePayment.BillingAddress.Clone(),
                CartName = param.CartName,
                CultureInfo = param.CultureInfo,
                CustomerId = param.CustomerId,
                Scope = param.Scope
            });

            var newPayment = GetActivePayment(cart);
            return newPayment;
        }

        protected virtual Task<List<Payment>> GetCartPaymentsAsync(UpdatePaymentMethodParam param)
        {
            return PaymentRepository.GetCartPaymentsAsync(new GetCartPaymentsParam
            {
                CartName = param.CartName,
                CultureInfo = param.CultureInfo,
                CustomerId = param.CustomerId,
                Scope = param.Scope
            });
        }

        /// <summary>
        /// Builds an instance of <see cref="InitializePaymentParam"/> that will be used to make the request to the repository.
        /// </summary>
        /// <param name="cart">Cart previously retrieved from the UpdatePaymentMethodAsync call.</param>
        /// <param name="updatePaymentMethodParam">Parameters passed to the UpdatePaymentMethodAsync method.</param>
        /// <returns>Instance of the <see cref="InitializePaymentParam"/> that will be used to make the call.</returns>
        /// <remarks>It may be useful to override this method to augment the request with AdditionalData or Options for the request.</remarks>
        protected virtual InitializePaymentParam BuildInitializePaymentParam(
            Overture.ServiceModel.Orders.Cart cart,
            UpdatePaymentMethodParam updatePaymentMethodParam)
        {
            var param = new InitializePaymentParam
            {
                CartName = updatePaymentMethodParam.CartName,
                CultureInfo = updatePaymentMethodParam.CultureInfo,
                CustomerId = updatePaymentMethodParam.CustomerId,
                PaymentId = updatePaymentMethodParam.PaymentId,
                Scope = updatePaymentMethodParam.Scope,
                PaymentType = updatePaymentMethodParam.PaymentType
            };

            return param;
        }

        /// <summary>
        /// Gets the ActivePaymentViewModel based on parameters.
        /// </summary>
        /// <param name="param">Parameter used to retrieve the ViewModel.</param>
        /// <returns></returns>
        protected virtual ActivePaymentViewModel GetActivePaymentViewModel(GetActivePaymentViewModelParam param)
        {
            if (param?.Cart == null) { return null; }

            var payment = GetActivePayment(param.Cart);
            var vm = ViewModelMapper.MapTo<ActivePaymentViewModel>(payment, param.CultureInfo);

            vm.ShouldCapturePayment = param.PaymentProvider.ShouldCapturePayment(payment);

            if (vm.ShouldCapturePayment)
            {
                vm.CapturePaymentUrl = param.PaymentProvider.GetCapturePaymentUrl(payment, param.CultureInfo);
            }

            if (payment.PaymentMethod != null)
            {
                vm.PaymentMethodType = payment.PaymentMethod.Type.ToString();
            }

            vm.ProviderName = param.PaymentProvider.ProviderName;
            vm.ProviderType = param.PaymentProvider.ProviderType;

            // TODO: Update when multiple payment provider available please
            // Quick hack, since we only support saving the payment method only 
            // for Moneris currently (when the user is authenticated) 
            vm.CanSavePaymentMethod = vm.ProviderType == MonerisCanadaPaymentProvider.MonerisProviderTypeName && param.IsAuthenticated;

            // Recurring orders must have a saved credit card to work.
            vm.MustSavePaymentMethod = vm.CanSavePaymentMethod && RecurringOrdersSettings.Enabled && RecurringOrderCartHelper.IsCartContainsRecurringOrderItems(param.Cart);

            param.PaymentProvider.AugmentViewModel(vm, payment);

            return vm;
        }

        protected virtual Payment GetActivePayment(Overture.ServiceModel.Orders.Cart cart)
        {
            return cart.Payments?.Find(p => !p.IsVoided());
        }

        protected virtual IPaymentProvider ObtainPaymentProvider(string paymentProviderName)
        {
            return PaymentProviderFactory.ResolveProvider(paymentProviderName);
        }
        public virtual async Task<CustomerPaymentMethodListViewModel> GetCustomerPaymentMethodListViewModelAsync(GetCustomerPaymentMethodListViewModelParam param)
        {
            if (param == null) throw new ArgumentNullException(nameof(param));

            var tasks = param.ProviderNames.Select(pName => PaymentRepository.GetCustomerPaymentMethodForProviderAsync(new GetCustomerPaymentMethodsForProviderParam
            {
                CustomerId = param.CustomerId,
                ScopeId = param.ScopeId,
                ProviderName = pName
            }));

            try
            {
                await Task.WhenAll(tasks).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Log.Warn($"GetCustomerPaymentMethodsRequest failed. {e.ToString()}");
            }

            var savedCreditCardVm = tasks
                .Where(t => !t.IsFaulted)
                .SelectMany(t => t.Result)
                .Select(s => CartViewModelFactory.MapSavedCreditCard(s, param.CultureInfo)).ToList();

            List<Task> taskList = new List<Task>();
            foreach (var vm in savedCreditCardVm)
            {
                taskList.Add(IsSavedCardUsedInRecurringOrders(vm, param.CultureInfo, param.CustomerId, param.ScopeId));
            }

            await Task.WhenAll(taskList).ConfigureAwait(false);

            return new CustomerPaymentMethodListViewModel
            {
                SavedCreditCards = savedCreditCardVm
                //AddWalletUrl
            };
        }

        public virtual async Task<CartViewModel> UpdateRecurringOrderCartPaymentMethodAsync(UpdatePaymentMethodParam param, string baseUrl)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.CartName)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.CartName)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.PaymentProviderName)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.PaymentProviderName)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope)), nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)), nameof(param)); }
            if (param.CustomerId == Guid.Empty) { throw new ArgumentException(GetMessageOfEmpty(nameof(param.CustomerId)), nameof(param)); }
            if (param.PaymentId == Guid.Empty) { throw new ArgumentException(GetMessageOfEmpty(nameof(param.PaymentId)), nameof(param)); }
            if (param.PaymentMethodId == Guid.Empty) { throw new ArgumentException(GetMessageOfEmpty(nameof(param.PaymentMethodId)), nameof(param)); }

            var activePayment = await PaymentRepository.GetPaymentAsync(new GetPaymentParam
            {
                CartName = param.CartName,
                CultureInfo = param.CultureInfo,
                CustomerId = param.CustomerId,
                Scope = param.Scope,
                PaymentId = param.PaymentId
            }).ConfigureAwait(false);

            if (activePayment == null) { return null; }

            if (activePayment.ShouldInvokePrePaymentSwitch())
            {
                activePayment = await PreparePaymentSwitch(param, activePayment);
                param.PaymentId = activePayment.Id;
            }

            var validatePaymentMethodParam = new ValidatePaymentMethodParam()
            {
                CartName = param.CartName,
                CultureInfo = param.CultureInfo,
                CustomerId = param.CustomerId,
                IsAuthenticated = param.IsAuthenticated,
                ProviderName = param.PaymentProviderName,
                Scope = param.Scope,
                PaymentMethodId = param.PaymentMethodId
            };

            var isPaymentMethodValid = await ValidatePaymentMethod(validatePaymentMethodParam);

            if (!isPaymentMethodValid)
            {
                throw new Exception($"Payment method for provider name /'{param.PaymentProviderName}/' not valid. Credit card has probably expired.");
            }

            Overture.ServiceModel.Orders.Cart cart = await PaymentRepository.UpdatePaymentMethodAsync(param);

            var initParam = BuildInitializePaymentParam(cart, param);
            var paymentProvider = ObtainPaymentProvider(param.PaymentProviderName);
            cart = await paymentProvider.InitializePaymentAsync(cart, initParam);

            var vm = await RecurringOrderCartsViewService.CreateCartViewModelAsync(new CreateRecurringOrderCartViewModelParam
            {
                Cart = cart,
                CultureInfo = param.CultureInfo,
                IncludeInvalidCouponsMessages = false,
                BaseUrl = baseUrl
            });

            return vm;
        }
    }
}