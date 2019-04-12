using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Orckestra.Composer.Cart.Extensions;
using Orckestra.Composer.Cart.Factory;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Repositories;
using Orckestra.Composer.Cart.ViewModels;
using Orckestra.Composer.Enums;
using Orckestra.Composer.Exceptions;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Dam;
using Orckestra.Composer.Repositories;
using Orckestra.Composer.Services;
using Orckestra.Composer.Services.Lookup;
using Orckestra.Composer.Utils;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel;
using Orckestra.Overture.ServiceModel.Customers;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Cart.Services
{
    /// <summary>
    /// Service for dealing with Checkout.
    /// </summary>
    public class CheckoutService : ICheckoutService
    {
        protected const string AddressBookIdPropertyBagKey = "AddressBookId";
        private readonly Dictionary<string, UpdateOperation> _updateOperations;

        protected ICartRepository CartRepository { get; private set; }
        protected ICartService CartService { get; private set; }
        protected IComposerJsonSerializer ComposerJsonSerializer { get; private set; }
        protected ICartViewModelFactory CartViewModelFactory { get; private set; }
        protected ILookupService LookupService { get; private set; }
        protected IAddressRepository AddressRepository { get; private set; }
        protected IShippingMethodViewService ShippingMethodViewService { get; private set; }
        protected IImageService ImageService{ get; private set; }
        protected IFulfillmentMethodRepository FulfillmentMethodRepository { get; private set; }
        protected IViewModelMapper ViewModelMapper { get; private set; }
        protected ILineItemViewModelFactory LineItemViewModelFactory { get; private set; }
        protected IPaymentRepository PaymentRepository { get; private set; }

        /// <summary>
        /// CheckoutService constructor
        /// </summary>
        /// <param name="cartRepository">The repository for accessing cart data</param>
        /// <param name="cartService"></param>
        /// <param name="composerJsonSerializer">The json serializer</param>
        /// <param name="cartViewModelFactory"></param>
        /// <param name="lookupService"></param>
        /// <param name="addressRepository"></param>
        /// <param name="shippingMethodViewService"></param>
        /// <param name="imageService"></param>
        /// <param name="fulfillmentMethodRepository"></param>
        /// <param name="viewModelMapper"></param>
        /// <param name="lineItemViewModelFactory"></param>
        /// <param name="paymentRepository"></param>
        public CheckoutService(
            ICartRepository cartRepository, 
            ICartService cartService,
            IComposerJsonSerializer composerJsonSerializer,
            ICartViewModelFactory cartViewModelFactory,
            ILookupService lookupService,
            IAddressRepository addressRepository,
            IShippingMethodViewService shippingMethodViewService,
            IImageService imageService,
            IFulfillmentMethodRepository fulfillmentMethodRepository,
            IViewModelMapper viewModelMapper,
            ILineItemViewModelFactory lineItemViewModelFactory,
            IPaymentRepository paymentRepository)
        {
            if (cartRepository == null) { throw new ArgumentNullException(ArgumentNullMessageFormatter.FormatErrorMessage(nameof(cartRepository))); }
            if (cartService == null) { throw new ArgumentNullException(ArgumentNullMessageFormatter.FormatErrorMessage(nameof(cartService))); }
            if (composerJsonSerializer == null) { throw new ArgumentNullException(ArgumentNullMessageFormatter.FormatErrorMessage(nameof(ComposerJsonSerializer))); }
            if (cartViewModelFactory == null) { throw new ArgumentNullException(ArgumentNullMessageFormatter.FormatErrorMessage(nameof(cartViewModelFactory))); }
            if (lookupService == null) { throw new ArgumentNullException(ArgumentNullMessageFormatter.FormatErrorMessage(nameof(LookupService))); }
            if (addressRepository == null) { throw new ArgumentNullException(ArgumentNullMessageFormatter.FormatErrorMessage(nameof(addressRepository))); }
            if (shippingMethodViewService == null) { throw new ArgumentNullException(ArgumentNullMessageFormatter.FormatErrorMessage(nameof(shippingMethodViewService))); }
            if (imageService == null) { throw new ArgumentNullException(ArgumentNullMessageFormatter.FormatErrorMessage(nameof(imageService))); }
            if (fulfillmentMethodRepository == null) { throw new ArgumentNullException(ArgumentNullMessageFormatter.FormatErrorMessage(nameof(fulfillmentMethodRepository))); }
            if (viewModelMapper == null) { throw new ArgumentNullException(ArgumentNullMessageFormatter.FormatErrorMessage(nameof(viewModelMapper))); }
            if (lineItemViewModelFactory == null) { throw new ArgumentNullException(ArgumentNullMessageFormatter.FormatErrorMessage(nameof(lineItemViewModelFactory)));}
            if (paymentRepository == null) { throw new ArgumentNullException(ArgumentNullMessageFormatter.FormatErrorMessage(nameof(paymentRepository))); }

            CartRepository = cartRepository;
            CartService = cartService;
            ComposerJsonSerializer = composerJsonSerializer;
            CartViewModelFactory = cartViewModelFactory;
            LookupService = lookupService;
            AddressRepository = addressRepository;
            ShippingMethodViewService = shippingMethodViewService;
            ImageService = imageService;
            FulfillmentMethodRepository = fulfillmentMethodRepository;
            ViewModelMapper = viewModelMapper;
            LineItemViewModelFactory = lineItemViewModelFactory;
            PaymentRepository = paymentRepository;

            _updateOperations = new Dictionary<string, UpdateOperation>();

            RegisterCartUpdateOperation<AddressViewModel>("ShippingAddress", UpdateShippingAddress, 1);
            RegisterCartUpdateOperation<RegisteredShippingAddressViewModel>("ShippingAddressRegistered", UpdateRegisteredShippingAddress, 1);
            RegisterCartUpdateOperation<CustomerSummaryViewModel>("GuestCustomerInfo", UpdateCustomer, 2);
            RegisterCartUpdateOperation<BillingAddressViewModel>("BillingAddress", UpdateBillingAddress, 3);
            RegisterCartUpdateOperation<RegisteredBillingAddressViewModel>("BillingAddressRegistered", UpdateRegisteredBillingAddress, 3);
            RegisterCartUpdateOperation<ShippingMethodViewModel>("ShippingMethod", UpdateShippingMethod, 4);
        }

        public async Task<CompleteCheckoutViewModel> CompleteCheckoutAsync(CompleteCheckoutParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param), "param"); }
            if (param.CultureInfo == null) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("CultureInfo"), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.CartName)) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("CartName"), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("Scope"), nameof(param)); }
            if (param.CustomerId == Guid.Empty) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("CustomerId"), nameof(param)); }

            var order = await CartRepository.CompleteCheckoutAsync(param).ConfigureAwait(false);

            return MapOrderToCompleteCheckoutViewModel(order, param.CultureInfo); 
        }

        protected virtual CompleteCheckoutViewModel MapOrderToCompleteCheckoutViewModel(Overture.ServiceModel.Orders.Order order, CultureInfo cultureInfo)
        {
            if (order == null) { return null; }

            var completeCheckoutViewModel = new CompleteCheckoutViewModel
            {
                OrderNumber = order.OrderNumber,
                CustomerEmail = order.Cart.Customer.Email,
                Affiliation = order.Cart.OrderLocation?.Name,
                Revenu = order.Cart.Total,
                Tax = order.Cart.TaxTotal,
                Shipping = order.Cart.FulfillmentCost,
                ShippingOptions = order.Cart.Shipments?.FirstOrDefault()?.FulfillmentMethod.FulfillmentMethodType.ToString().ToLowerInvariant(),
                BillingCurrency = order.Cart.BillingCurrency,
                Coupons = MapCoupons(order, cultureInfo),
                LineItems = MapCompleteCheckoutLineItems(order, cultureInfo)
            };

            return completeCheckoutViewModel;
        }

        protected virtual List<CouponViewModel> MapCoupons(Overture.ServiceModel.Orders.Order order, CultureInfo cultureInfo)
        {
            if (order?.Cart == null)
            {
                return new List<CouponViewModel>();
            }

            var couponsViewModel = CartViewModelFactory.GetCouponsViewModel(order.Cart, cultureInfo, false);

            return couponsViewModel != null ? couponsViewModel.ApplicableCoupons : new List<CouponViewModel>();
        }

        protected virtual List<CompleteCheckoutLineItemViewModel> MapCompleteCheckoutLineItems(Overture.ServiceModel.Orders.Order order, CultureInfo cultureInfo)
        {
            var lineItems = order.Cart.GetLineItems().Select(li =>
            {
                var viewModel = ViewModelMapper.MapTo<CompleteCheckoutLineItemViewModel>(li, cultureInfo);

                var brandProperty = li.ProductSummary.Brand ?? string.Empty;

                var brand = string.IsNullOrWhiteSpace(brandProperty) ? string.Empty : LookupService.GetLookupDisplayNameAsync(new GetLookupDisplayNameParam
                {
                    Delimiter = ", ",
                    LookupName = "Brand",
                    LookupType = LookupType.Product,
                    Value = li.ProductSummary.Brand,
                    CultureInfo = cultureInfo
                }).Result;
                viewModel.Name = li.ProductSummary.DisplayName;
                viewModel.BrandId = brandProperty;
                viewModel.Brand = brand;
                viewModel.CategoryId = li.ProductSummary.PrimaryParentCategoryId;
                viewModel.KeyVariantAttributesList = LineItemViewModelFactory.GetKeyVariantAttributes(new GetKeyVariantAttributesParam {
                    KvaValues = li.KvaValues,
                    KvaDisplayValues = li.KvaDisplayValues
                }).ToList();

                return viewModel;
            });

            return lineItems.ToList();
        }

        /// <summary>
        /// Update a Cart during the checkout process.
        /// </summary>
        /// <param name="param">UpdateCheckoutCartParam</param>
        public async Task<UpdateCartResultViewModel> UpdateCheckoutCartAsync(UpdateCheckoutCartParam param)
        {
            if (param == null) { throw new ArgumentNullException("param"); }
            if (param.GetCartParam == null) { throw new ArgumentException("param"); }
            if (param.UpdateValues == null) { throw new ArgumentException("UpdateValues"); }

            var cart = await CartRepository.GetCartAsync(param.GetCartParam).ConfigureAwait(false);

            if (cart == null) { throw new InvalidOperationException("No cart found"); }

            SetCustomerType(param.IsGuest, cart);
            UpdateStepInfo(cart, param.CurrentStep + 1);

            var operationsToExecute = GetOperationsToExecute(param.UpdateValues);

            foreach (var operation in operationsToExecute)
            {
                await operation.Invoke(cart);
            }

            var updatedCart = await CartRepository.UpdateCartAsync(UpdateCartParamFactory.Build(cart)).ConfigureAwait(false);

            var createCartViewModelParam = new CreateCartViewModelParam
            {
                Cart = updatedCart,
                CultureInfo = param.GetCartParam.CultureInfo,
                BaseUrl = param.GetCartParam.BaseUrl
            };

            var cartViewModel = await CreateCartViewModelAsync(createCartViewModelParam).ConfigureAwait(false);

            var updateCartResultViewModel = new UpdateCartResultViewModel
            {
                HasErrors = updatedCart.Messages != null && updatedCart.Messages.Any(m => m.Severity == ExecutionMessageSeverity.Error || m.Severity == ExecutionMessageSeverity.Failure),
                Cart = cartViewModel
            };

            return updateCartResultViewModel;
        }

        /// <summary>
        /// Obtains all the Update Operations to execute in the right order.
        /// </summary>
        /// <param name="updateValues">UpdateValues received from the request.</param>
        /// <returns></returns>
        protected virtual IEnumerable<Func<Overture.ServiceModel.Orders.Cart, Task>> GetOperationsToExecute(Dictionary<string, string> updateValues)
        {
            var operationsToExecute = from op in updateValues
                                      where _updateOperations.ContainsKey(op.Key)
                                      let updateOp = _updateOperations[op.Key]
                                      orderby updateOp.Order
                                      select new Func<Overture.ServiceModel.Orders.Cart, Task>(c => updateOp.ExecuteAsync(c, op.Value));

            return operationsToExecute;
        }

        private static void SetCustomerType(bool isGuest, Overture.ServiceModel.Orders.Cart cart)
        {
            if (cart.Customer == null)
            {
                cart.Customer = new CustomerSummary();
            }

            if (!isGuest)
            {
                cart.Customer = new CustomerSummary();
            }

            cart.Customer.Type = isGuest ? CustomerType.Guest : CustomerType.Registered;
        }

        protected virtual async Task<CartViewModel> CreateCartViewModelAsync(CreateCartViewModelParam param)
        {
            if (param == null) { throw new ArgumentNullException("param"); }
            if (param.Cart == null) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("Cart"), "param"); }
            if (param.BaseUrl == null) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("BaseUrl"), "param"); }

            param.ProductImageInfo = new ProductImageInfo
            {
                ImageUrls = await ImageService.GetImageUrlsAsync(param.Cart.GetLineItems()).ConfigureAwait(false)
            };

            var methodDisplayNames = await LookupService.GetLookupDisplayNamesAsync(new GetLookupDisplayNamesParam
            {
                CultureInfo = param.CultureInfo,
                LookupType = LookupType.Order,
                LookupName = "PaymentMethodType",
            }).ConfigureAwait(false);

            param.PaymentMethodDisplayNames = methodDisplayNames;

            var vm = CartViewModelFactory.CreateCartViewModel(param);
            return vm;
        }

        /// <summary>
        /// Registers a new update operation for the checkout process.
        /// </summary>
        /// <param name="operationName">The key of the ViewModel to update</param>
        /// <param name="updateAction">The action to apply on a Cart from a ViewModel</param>
        /// <param name="order">Order in which the operation must be performed.</param>
        public void RegisterCartUpdateOperation<TViewModel>(string operationName, Func<Overture.ServiceModel.Orders.Cart, TViewModel, Task> updateAction, int? order)
        {
            if (string.IsNullOrEmpty(operationName)) { throw new ArgumentNullException("operationName"); }
            if (updateAction == null) { throw new ArgumentNullException("updateAction"); }

            var operation = new Func<Overture.ServiceModel.Orders.Cart, object, Task>((cart, viewModel) => updateAction(cart, ParseJson<TViewModel>(viewModel)));
            _updateOperations[operationName] = order == null ? new UpdateOperation(operation) : new UpdateOperation(order.Value, operation);
        }

        private TViewModel ParseJson<TViewModel>(object viewModel)
        {
            var jsonValue = viewModel as string;
            if (jsonValue == null)
            {
                throw new InvalidDataException("Value must be a string");
            }

            var deserializedJson = ComposerJsonSerializer.Deserialize<TViewModel>(jsonValue);
            ValidateViewModel(deserializedJson);

            return deserializedJson;
        }

        protected virtual void ValidateViewModel<TViewModel>(TViewModel viewModel)
        {
            //Include Meta-data in validation maybe?
            var context = new ValidationContext(viewModel);
            var errors = new List<ValidationResult>();

            if (!Validator.TryValidateObject(viewModel, context, errors))
            {
                var eVm = errors.Select(e => new ErrorViewModel
                {
                    ErrorCode = e.ErrorMessage,
                    ErrorMessage = e.MemberNames.Aggregate(string.Empty, (s, s1) => s + "; " + s1)
                });

                throw new ComposerException(eVm.ToList());
            }
        }

        protected virtual void UpdateStepInfo(Overture.ServiceModel.Orders.Cart cart, int lastCompletedCheckoutStep)
        {
            if (cart.PropertyBag == null)
            {
                cart.PropertyBag = new PropertyBag();
            }

            if (cart.PropertyBag.ContainsKey(CartConfiguration.CartPropertyBagLastCheckoutStep))
            {
                cart.PropertyBag[CartConfiguration.CartPropertyBagLastCheckoutStep] = lastCompletedCheckoutStep;
            }
            else
            {
                cart.PropertyBag.Add(CartConfiguration.CartPropertyBagLastCheckoutStep, lastCompletedCheckoutStep);
            }
        }

        protected virtual Task UpdateCustomer(Overture.ServiceModel.Orders.Cart cart, CustomerSummaryViewModel customerSummaryViewModel)
        {
            if (customerSummaryViewModel == null)
            {
                return Task.FromResult(0);
            }

            if (cart.Customer == null)
            {
                cart.Customer = new CustomerSummary();
            }

            cart.Customer.LastName = null;
            cart.Customer.FirstName = null;
            cart.Customer.MiddleName = null;
            cart.Customer.Phone = null;
            cart.Customer.Email = customerSummaryViewModel.Email;

            return Task.FromResult(0);
        }

        protected virtual async Task UpdateShippingAddress(Overture.ServiceModel.Orders.Cart cart, AddressViewModel addressViewModel)
        {
            if (addressViewModel == null)
            {
                return;
            }

            var shipment = cart.Shipments.FirstOrDefault();

            if (shipment == null)
            {
                return;
            }

            var newAddress = CreateAddressFromViewModel(addressViewModel);

            var isShippingChanged = shipment.Address == null || !IsEqual(shipment.Address, newAddress);

            shipment.Address = newAddress;

            await ShippingMethodViewService.EstimateShippingAsync(new EstimateShippingParam
            {
                Cart = cart,
                CultureInfo = CultureInfo.GetCultureInfo(cart.CultureName), //TODO: Fix me
                ForceUpdate = isShippingChanged

            }).ConfigureAwait(false);
        }



        protected virtual async Task UpdateShippingMethod(Overture.ServiceModel.Orders.Cart cart, ShippingMethodViewModel shippingMethodViewModel)
        {
            if (shippingMethodViewModel == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(shippingMethodViewModel.Name))
            {
                return;
            }

            var shipment = cart.Shipments.FirstOrDefault();
            if (shipment == null)
            {
                return;
            }

            shipment.FulfillmentMethod = await GetFulfillmentMethodAsync(cart, shippingMethodViewModel);
        }

        private async Task<FulfillmentMethod> GetFulfillmentMethodAsync(Overture.ServiceModel.Orders.Cart cart, ShippingMethodViewModel shippingMethodViewModel)
        {
            var param = new GetShippingMethodsParam
            {
                CartName = cart.Name,
                CustomerId = cart.CustomerId,
                CultureInfo = new CultureInfo(cart.CultureName),
                Scope = cart.ScopeId
            };

            var fulfillmentMethods = await FulfillmentMethodRepository.GetCalculatedFulfillmentMethods(param);

            var fulfillmentMethod = fulfillmentMethods.FirstOrDefault(method =>
                method.Name == shippingMethodViewModel.Name &&
                method.ShippingProviderId == shippingMethodViewModel.ShippingProviderId);

            if (fulfillmentMethod == null)
            {
                throw new ComposerException(new ErrorViewModel
                {
                    ErrorCode = "",
                    ErrorMessage = "Unable to find any shipment provider matching the provided parameters."
                });
            }

            return fulfillmentMethod;
        }

        protected virtual Task UpdateBillingAddress(Overture.ServiceModel.Orders.Cart cart, BillingAddressViewModel billingAddressViewModel)
        {
            if (billingAddressViewModel == null)
            {
                return Task.FromResult(0);
            }

            var payment = GetPayment(cart);

            if (payment == null)
            {
                return Task.FromResult(0);
            }

            if (billingAddressViewModel.UseShippingAddress)
            {
                var shipment = cart.Shipments.FirstOrDefault();

                if (shipment?.Address == null)
                {
                    return Task.FromResult(0);
                }

                payment.BillingAddress = shipment.Address.Clone();
            }
            else
            {
                payment.BillingAddress = CreateAddressFromViewModel(billingAddressViewModel);
            }

            if (cart.Customer.Type == CustomerType.Guest)
            {
                cart.Customer.FirstName = payment.BillingAddress.FirstName;
                cart.Customer.LastName = payment.BillingAddress.LastName;
                cart.Customer.Phone = payment.BillingAddress.PhoneNumber;
            }

            return Task.FromResult(0);
        }

        protected virtual async Task UpdateRegisteredBillingAddress(Overture.ServiceModel.Orders.Cart cart, RegisteredBillingAddressViewModel registeredBillingAddressViewModel)
        {
            if (registeredBillingAddressViewModel == null)
            {
                return;
            }

            var payment = cart.Payments.FirstOrDefault();

            if (payment == null)
            {
                return;
            }

            if (registeredBillingAddressViewModel.UseShippingAddress)
            {
                var shipment = cart.Shipments.FirstOrDefault();

                if (shipment?.Address == null) { return; }

                var isBillingChanged = payment.BillingAddress == null || !IsEqual(payment.BillingAddress, shipment.Address);

                if (isBillingChanged)
                {
                    payment.BillingAddress = shipment.Address.Clone();
                    payment.BillingAddress.Id = Guid.Empty;
                }
            }
            else
            {
                if (registeredBillingAddressViewModel.BillingAddressId == Guid.Empty)
                {
                    return;
                }

                var newAddress = await AddressRepository.GetAddressByIdAsync(registeredBillingAddressViewModel.BillingAddressId).ConfigureAwait(false);
                var isBillingChanged = payment.BillingAddress == null || !IsEqual(payment.BillingAddress, newAddress);

                if (isBillingChanged)
                {
                    payment.BillingAddress = newAddress;
                    payment.BillingAddress.PropertyBag[AddressBookIdPropertyBagKey] = newAddress.Id;
                    payment.BillingAddress.Id = Guid.Empty;
                }
            }
        }

        protected virtual async Task UpdateRegisteredShippingAddress(Overture.ServiceModel.Orders.Cart cart, RegisteredShippingAddressViewModel registeredShippingAddressViewModel)
        {
            if (registeredShippingAddressViewModel == null)
            {
                return;
            }

            if (registeredShippingAddressViewModel.ShippingAddressId == Guid.Empty)
            {
                return;
            }

            var shipment = cart.Shipments.FirstOrDefault();

            if (shipment == null)
            {
                return;
            }

            var newAddress = await AddressRepository.GetAddressByIdAsync(registeredShippingAddressViewModel.ShippingAddressId).ConfigureAwait(false);
            var isShippingChanged = shipment.Address == null || !IsEqual(shipment.Address, newAddress);

            if (isShippingChanged)
            {
                shipment.Address = newAddress;
                shipment.Address.PropertyBag[AddressBookIdPropertyBagKey] = newAddress.Id;
                shipment.Address.Id = Guid.Empty;
            }

            //In the case the user didn't do estimate shipping before
            await ShippingMethodViewService.EstimateShippingAsync(new EstimateShippingParam
            {
                Cart = cart,
                CultureInfo = CultureInfo.GetCultureInfo(cart.CultureName), //TODO: Fix me
                ForceUpdate = isShippingChanged

            }).ConfigureAwait(false);
        }

        protected virtual bool IsEqual(Address firstAddress, Address secondAddress)
        {
            return firstAddress.AddressName == secondAddress.AddressName &&
                   firstAddress.City == secondAddress.City &&
                   firstAddress.CountryCode == secondAddress.CountryCode &&
                   firstAddress.FirstName == secondAddress.FirstName &&
                   firstAddress.LastName == secondAddress.LastName &&
                   firstAddress.Line1 == secondAddress.Line1 &&
                   firstAddress.Line2 == secondAddress.Line2 &&
                   firstAddress.PhoneNumber == secondAddress.PhoneNumber &&
                   firstAddress.PostalCode == secondAddress.PostalCode &&
                   firstAddress.RegionCode == secondAddress.RegionCode;
        }

        protected virtual Address CreateAddressFromViewModel(AddressViewModel addressViewModel)
        {
            var address = new Address
            {
                AddressName = addressViewModel.AddressName,
                City = addressViewModel.City,
                CountryCode = addressViewModel.CountryCode,
                FirstName = addressViewModel.FirstName,
                LastName = addressViewModel.LastName,
                Line1 = addressViewModel.Line1,
                Line2 = addressViewModel.Line2,
                PhoneNumber = addressViewModel.PhoneNumber,
                PostalCode = addressViewModel.PostalCode,
                RegionCode = addressViewModel.RegionCode
            };

            return address;
        }

        protected virtual Address CreateAddressFromViewModel(BillingAddressViewModel addressViewModel)
        {
            var address = new Address
            {
                AddressName = addressViewModel.AddressName,
                City = addressViewModel.City,
                CountryCode = addressViewModel.CountryCode,
                FirstName = addressViewModel.FirstName,
                LastName = addressViewModel.LastName,
                Line1 = addressViewModel.Line1,
                Line2 = addressViewModel.Line2,
                PhoneNumber = addressViewModel.PhoneNumber,
                PostalCode = addressViewModel.PostalCode,
                RegionCode = addressViewModel.RegionCode
            };

            return address;
        }

        protected virtual Payment GetPayment(Overture.ServiceModel.Orders.Cart cart)
        {
            var payments = cart.Payments;

            if (payments == null || payments.All(payment => payment.IsVoided()))
            {
                throw new InvalidOperationException("No valid payment was found in the cart.");
            }

            return payments.First(payment => !payment.IsVoided());
        }
    }
}
