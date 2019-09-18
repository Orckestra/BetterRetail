using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Orckestra.Composer.Cart.Extensions;
using Orckestra.Composer.Cart.Factory;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Providers.Payment;
using Orckestra.Composer.Cart.Repositories;
using Orckestra.Composer.Cart.Requests;
using Orckestra.Composer.Cart.ViewModels;
using Orckestra.Composer.Enums;
using Orckestra.Composer.Factory;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Requests;
using Orckestra.Composer.Services;
using Orckestra.Composer.Services.Lookup;
using Orckestra.Composer.Utils;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel.Orders;
using Orckestra.Composer.Logging;
using Orckestra.Composer.Cart.Helper;
using Orckestra.Composer.Configuration;

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
            if (paymentRepository == null) { throw new ArgumentNullException("paymentRepository"); }
            if (cartViewModelFactory == null) { throw new ArgumentNullException("cartViewModelFactory"); }
            if (lookupService == null) { throw new ArgumentNullException("lookupService"); }
            if (viewModelMapper == null) { throw new ArgumentNullException("viewModelMapper"); }
            if (paymentProviderFactory == null) { throw new ArgumentNullException("paymentProviderFactory"); }
            if (recurringOrderTemplatesViewService == null) { throw new ArgumentNullException("recurringOrderTemplatesViewService"); }
            if (recurringOrderCartsViewService == null) { throw new ArgumentNullException("recurringOrderCartsViewService"); }

            PaymentRepository = paymentRepository;
            CartViewModelFactory = cartViewModelFactory;
            CartRepository = cartRepository;
            LookupService = lookupService;
            ViewModelMapper = viewModelMapper;
            PaymentProviderFactory = paymentProviderFactory;
            RecurringOrderTemplatesViewService = recurringOrderTemplatesViewService;
            RecurringOrderCartsViewService = recurringOrderCartsViewService;
            RecurringOrdersSettings = recurringOrdersSettings;
        }


        /// <summary>
        /// Gets an enumeration of all available payment providers.
        /// </summary>
        /// <param name="param">Parameters used to retrieve the payment providers.</param>
        /// <returns></returns>
        public virtual Task<IEnumerable<PaymentProviderViewModel>> GetPaymentProvidersAsync(GetPaymentProvidersParam param)
        {
            if (param == null) { throw new ArgumentNullException("param"); }
            if (param.CultureInfo == null) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("CultureInfo"), "param"); }

            var providers = PaymentProviderFactory.ResolveAllProviders();
            var viewModels = new List<PaymentProviderViewModel>();

            foreach (var paymentProvider in providers)
            {
                var vm = MapPaymentProviderViewModel(paymentProvider, param.CultureInfo);
                viewModels.Add(vm);
            }

            return Task.FromResult(viewModels.AsEnumerable());
        }

        protected virtual PaymentProviderViewModel MapPaymentProviderViewModel(IPaymentProvider paymentProvider, CultureInfo cultureInfo)
        {
            var vm = ViewModelMapper.MapTo<PaymentProviderViewModel>(paymentProvider, cultureInfo);
            return vm;
        }

        /// <summary>
        /// Get the Active Payment methods available for a cart.
        /// </summary>
        /// <param name="param">GetPaymentMethodsParam</param>
        /// <returns>A List of PaymentMethodViewModel</returns>
        public virtual async Task<CheckoutPaymentViewModel> GetPaymentMethodsAsync(GetPaymentMethodsParam param)
        {
            if (param == null) { throw new ArgumentNullException("param"); }
            if (string.IsNullOrWhiteSpace(param.CartName)) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("CartName"), "param"); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("Scope"), "param"); }
            if (param.ProviderNames == null) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("ProviderName"), "param"); }

            var paymentMethods = await GetAllPaymentMethodsAsync(param).ConfigureAwait(false);
            var cart = await CartRepository.GetCartAsync(new GetCartParam
            {
                CartName = param.CartName,
                CustomerId = param.CustomerId,
                CultureInfo = param.CultureInfo,
                Scope = param.Scope
            }).ConfigureAwait(false);

            var hasRecurring = false;
            if (RecurringOrdersSettings.Enabled)
            {
                hasRecurring = RecurringOrderCartHelper.IsCartContainsRecurringOrderItems(cart);
            }

            if (hasRecurring)
            {
                var supported = CartConfiguration.SupportedRecurringOrderPaymentMethodTypes;
                paymentMethods = paymentMethods.Where(p => supported.Where(s => s.ToString() == p.PaymentType).Count() > 0).ToList();
            }

            var vm = await MapCheckoutPaymentViewModel(cart, paymentMethods, param.CultureInfo, param.IsAuthenticated);
            return vm;
        }

        protected virtual async Task<List<IPaymentMethodViewModel>> GetAllPaymentMethodsAsync(GetPaymentMethodsParam param)
        {
            var paymentMethods = await PaymentRepository.GetPaymentMethodsAsync(param).ConfigureAwait(false);
            if (paymentMethods == null) { return null; }

            var vm = await MapPaymentMethodsViewModel(paymentMethods, param.CultureInfo, param.CustomerId, param.Scope).ConfigureAwait(false);
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
                    
                    await IsSavedCardUsedInRecurringOrders(methodViewModel, cultureInfo, customerId, scope).ConfigureAwaitWithCulture(false);

                    paymentMethodViewModels.Add(methodViewModel);
                }
            }

            return paymentMethodViewModels.ToList();

        }

        protected virtual async Task<IPaymentMethodViewModel> IsSavedCardUsedInRecurringOrders(IPaymentMethodViewModel methodViewModel,
            CultureInfo cultureInfo,
            Guid customerId,
            string scope)
        {
            if (methodViewModel is SavedCreditCardPaymentMethodViewModel)
            {
                SavedCreditCardPaymentMethodViewModel vm = methodViewModel as SavedCreditCardPaymentMethodViewModel;

                vm.IsUsedInRecurringOrders = await RecurringOrderTemplatesViewService.GetIsPaymentMethodUsedInRecurringOrders(new GetIsPaymentMethodUsedInRecurringOrdersRequest()
                {
                    CultureInfo = cultureInfo,
                    CustomerId = customerId,
                    PaymentMethodId = methodViewModel.Id.ToString(),
                    ScopeId = scope
                }).ConfigureAwaitWithCulture(false);

                return vm;
            }
            return methodViewModel;
        }

        protected virtual async Task<CheckoutPaymentViewModel> MapCheckoutPaymentViewModel(Overture.ServiceModel.Orders.Cart cart, List<IPaymentMethodViewModel> paymentMethodViewModels, CultureInfo cultureInfo, bool isAuthenticated)
        {
            var payment = GetActivePayment(cart);

            if (payment == null) { return null; }

            var paymentId = payment.Id;

            ActivePaymentViewModel activePaymentVm = null;
            if (payment.PaymentMethod != null)
            {

                var provider = PaymentProviderFactory.ResolveProvider(payment.PaymentMethod.PaymentProviderName);

                activePaymentVm = GetActivePaymentViewModel(new GetActivePaymentViewModelParam
                {
                    Cart = cart,
                    CultureInfo = cultureInfo,
                    PaymentProvider = provider,
                    IsAuthenticated = isAuthenticated
                });

                var activePaymentMethodVm =
                    paymentMethodViewModels.FirstOrDefault(
                        pm =>
                            pm.Id == payment.PaymentMethod.Id &&
                            pm.PaymentProviderName == payment.PaymentMethod.PaymentProviderName &&
                            pm.IsValid);

                if (activePaymentMethodVm != null)
                {
                    activePaymentMethodVm.IsSelected = true;
                }

                // If active payment is set to soemthing that doesn't exists anymore
                // Select the first payment method available...
                if (activePaymentVm != null && activePaymentMethodVm == null)
                {
                    activePaymentVm = await UpdateActivePaymentMethodAsync(new UpdatePaymentMethodParam
                    {
                        CartName = cart.Name,
                        CultureInfo = cultureInfo,
                        CustomerId = cart.CustomerId,
                        IsAuthenticated = isAuthenticated,
                        Scope = cart.ScopeId,
                        PaymentId = activePaymentVm.Id,
                        PaymentMethodId = paymentMethodViewModels.First().Id,
                        PaymentProviderName = paymentMethodViewModels.First().PaymentProviderName,
                    }).ConfigureAwait(false);

                    paymentId = activePaymentVm.Id;

                    paymentMethodViewModels.First().IsSelected = true;
                }
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
            if (string.IsNullOrWhiteSpace(param.CartName)) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("CartName"), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.PaymentProviderName)) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("PaymentProviderName"), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("Scope"), nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("CultureInfo"), nameof(param)); }
            if (param.CustomerId == default(Guid)) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("CustomerId"), nameof(param)); }
            if (param.PaymentId == default(Guid)) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("PaymentId"), nameof(param)); }
            if (param.PaymentMethodId == default(Guid)) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("PaymentMethodId"), nameof(param)); }

            var payments = await GetCartPaymentsAsync(param).ConfigureAwait(false);
            var activePayment = payments.GetPayment(param.PaymentId);

            if (activePayment.ShouldInvokePrePaymentSwitch())
            {
                activePayment = await PreparePaymentSwitch(param, activePayment).ConfigureAwait(false);
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

            var isPaymentMethodValid = await ValidatePaymentMethod(validatePaymentMethodParam).ConfigureAwait(false);

            if (!isPaymentMethodValid)
            {
                throw new Exception($"Payment method for provider name /'{param.PaymentProviderName}/' not valid. Credit card has probably expired.");
            }

            Overture.ServiceModel.Orders.Cart cart = await PaymentRepository.UpdatePaymentMethodAsync(param).ConfigureAwait(false);

            var initParam = BuildInitializePaymentParam(cart, param);
            var paymentProvider = ObtainPaymentProvider(param.PaymentProviderName);
            cart = await paymentProvider.InitializePaymentAsync(cart, initParam).ConfigureAwait(false);

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
            Guard.NotNullOrWhiteSpace(param.CartName, nameof(param.CartName));
            Guard.NotNullOrWhiteSpace(param.Scope, nameof(param.Scope));
            Guard.NotNullOrWhiteSpace(param.ProviderName, nameof(param.ProviderName));
            Guard.NotNull(param.CultureInfo, nameof(param.CultureInfo));
            Guard.NotNull(param.CustomerId, nameof(param.CustomerId));
            Guard.NotNull(param.PaymentMethodId, nameof(param.PaymentMethodId));

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

            var paymentMethod = await GetAllPaymentMethodsAsync(getPaymentMethodParam).ConfigureAwait(false);
            if (paymentMethod == null || paymentMethod.All(p => p.Id != param.PaymentMethodId))
            {
                throw new Exception($"Payment method for provider name /'{param.ProviderName}/' not found.");
            }
            return paymentMethod
                .FirstOrDefault(p => p.Id == param.PaymentMethodId)
                .IsValid;
        }

        public virtual async Task<ActivePaymentViewModel> GetActivePayment(GetActivePaymentParam param)
        {
            if (param == null) throw new ArgumentNullException(nameof(param), ArgumentNullMessageFormatter.FormatErrorMessage(nameof(param)));
            if (param.CustomerId == Guid.Empty) throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage(nameof(param.CustomerId)), nameof(param.CustomerId));
            if (param.CultureInfo == null) throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage(nameof(param.CultureInfo)), nameof(param.CultureInfo));
            if (String.IsNullOrWhiteSpace(param.CartName)) throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage(nameof(param.CartName)), nameof(param.CartName));
            if (String.IsNullOrWhiteSpace(param.Scope)) throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage(nameof(param.Scope)), nameof(param.Scope));

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
            if (param == null) throw new ArgumentNullException(nameof(param), "param is required");
            if (param.PaymentMethodId == Guid.Empty) throw new ArgumentException("param.PaymentMethodId is required", nameof(param.PaymentMethodId));

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
            }).ConfigureAwait(false);

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
        protected virtual InitializePaymentParam BuildInitializePaymentParam(Overture.ServiceModel.Orders.Cart cart,
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
            if (param.Cart == null) { return null; }

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
            return cart.Payments == null ? null : cart.Payments.FirstOrDefault(p => !p.IsVoided());
        }

        protected virtual IPaymentProvider ObtainPaymentProvider(string paymentProviderName)
        {
            return PaymentProviderFactory.ResolveProvider(paymentProviderName);
        }
        public virtual async Task<CustomerPaymentMethodListViewModel> GetCustomerPaymentMethodListViewModelAsync(GetCustomerPaymentMethodListViewModelParam param)
        {
            if (param == null) throw new ArgumentNullException(nameof(param), ArgumentNullMessageFormatter.FormatErrorMessage(nameof(param)));

            var tasks = param.ProviderNames.Select(pName => PaymentRepository.GetCustomerPaymentMethodForProviderAsync(new GetCustomerPaymentMethodsForProviderParam
            {
                CustomerId = param.CustomerId,
                ScopeId = param.ScopeId,
                ProviderName = pName
            })).ToArray();

            try
            {
                await Task.WhenAll(tasks).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Log.Warn($"GetCustomerPaymentMethodsRequest failed. {e.ToString()}");
            }

            tasks = tasks.Where(t => !t.IsFaulted).ToArray();
            var paymentMethods = tasks.SelectMany(t => t.Result).ToList();

            var savedCreditCardVm = paymentMethods.Select(s => CartViewModelFactory.MapSavedCreditCard(s, param.CultureInfo)).ToList();
            foreach (var vm in savedCreditCardVm)
            {
                await IsSavedCardUsedInRecurringOrders(vm, param.CultureInfo, param.CustomerId, param.ScopeId).ConfigureAwaitWithCulture(false);
            }

            return new CustomerPaymentMethodListViewModel
            {
                SavedCreditCards = savedCreditCardVm
                //AddWalletUrl
            };
        }

        public virtual async Task<CartViewModel> UpdateRecurringOrderCartPaymentMethodAsync(UpdatePaymentMethodParam param, string baseUrl)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.CartName)) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("CartName"), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.PaymentProviderName)) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("PaymentProviderName"), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("Scope"), nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("CultureInfo"), nameof(param)); }
            if (param.CustomerId == default(Guid)) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("CustomerId"), nameof(param)); }
            if (param.PaymentId == default(Guid)) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("PaymentId"), nameof(param)); }
            if (param.PaymentMethodId == default(Guid)) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("PaymentMethodId"), nameof(param)); }

            var activePayment = await PaymentRepository.GetPaymentAsync(new GetPaymentParam {
                CartName = param.CartName,
                CultureInfo = param.CultureInfo,
                CustomerId = param.CustomerId,
                Scope = param.Scope,
                PaymentId = param.PaymentId
            }).ConfigureAwait(false);
            if (activePayment == null) { return null; }

            if (activePayment.ShouldInvokePrePaymentSwitch())
            {
                activePayment = await PreparePaymentSwitch(param, activePayment).ConfigureAwait(false);
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

            var isPaymentMethodValid = await ValidatePaymentMethod(validatePaymentMethodParam).ConfigureAwait(false);

            if (!isPaymentMethodValid)
            {
                throw new Exception($"Payment method for provider name /'{param.PaymentProviderName}/' not valid. Credit card has probably expired.");
            }

            Overture.ServiceModel.Orders.Cart cart = await PaymentRepository.UpdatePaymentMethodAsync(param).ConfigureAwait(false);

            var initParam = BuildInitializePaymentParam(cart, param);
            var paymentProvider = ObtainPaymentProvider(param.PaymentProviderName);
            cart = await paymentProvider.InitializePaymentAsync(cart, initParam).ConfigureAwait(false);

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
