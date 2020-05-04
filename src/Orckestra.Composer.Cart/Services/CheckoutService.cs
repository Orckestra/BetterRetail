using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Orckestra.Composer.Cart.Extensions;
using Orckestra.Composer.Cart.Factory;
using Orckestra.Composer.Cart.Factory.Order;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Parameters.Order;
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
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel;
using Orckestra.Overture.ServiceModel.Customers;
using Orckestra.Overture.ServiceModel.Orders;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;
using Orckestra.Composer.Store.Repositories;

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
        protected IOrderDetailsViewModelFactory OrderDetailsViewModelFactory { get; private set; }
        protected virtual IOrderUrlProvider OrderUrlProvider { get; private set; }
        protected ICartService CartService { get; private set; }
        protected IComposerJsonSerializer ComposerJsonSerializer { get; private set; }
        protected ICartViewModelFactory CartViewModelFactory { get; private set; }
        protected ILookupService LookupService { get; private set; }
        protected IAddressRepository AddressRepository { get; private set; }
        protected IShippingMethodViewService ShippingMethodViewService { get; private set; }
        protected IImageService ImageService { get; private set; }
        protected IFulfillmentMethodRepository FulfillmentMethodRepository { get; private set; }
        protected IViewModelMapper ViewModelMapper { get; private set; }
        protected ILineItemViewModelFactory LineItemViewModelFactory { get; private set; }
        protected IPaymentRepository PaymentRepository { get; private set; }
        protected IStoreRepository StoreRepository { get; private set; }
        protected IInventoryLocationProvider InventoryLocationProvider { get; private set; }


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
            IPaymentRepository paymentRepository,
            IOrderDetailsViewModelFactory orderDetailsViewModelFactory,
            IOrderUrlProvider orderUrlProvider,
            IStoreRepository storeRepository,
            IInventoryLocationProvider inventoryLocationProvider)
        {
            CartRepository = cartRepository ?? throw new ArgumentNullException(nameof(cartRepository));
            CartService = cartService ?? throw new ArgumentNullException(nameof(cartService));
            ComposerJsonSerializer = composerJsonSerializer ?? throw new ArgumentNullException(nameof(composerJsonSerializer));
            CartViewModelFactory = cartViewModelFactory ?? throw new ArgumentNullException(nameof(cartViewModelFactory));
            LookupService = lookupService ?? throw new ArgumentNullException(nameof(lookupService));
            AddressRepository = addressRepository ?? throw new ArgumentNullException(nameof(addressRepository));
            ShippingMethodViewService = shippingMethodViewService ?? throw new ArgumentNullException(nameof(shippingMethodViewService));
            ImageService = imageService ?? throw new ArgumentNullException(nameof(imageService));
            FulfillmentMethodRepository = fulfillmentMethodRepository ?? throw new ArgumentNullException(nameof(fulfillmentMethodRepository));
            ViewModelMapper = viewModelMapper ?? throw new ArgumentNullException(nameof(viewModelMapper));
            LineItemViewModelFactory = lineItemViewModelFactory ?? throw new ArgumentNullException(nameof(lineItemViewModelFactory));
            PaymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
            OrderDetailsViewModelFactory = orderDetailsViewModelFactory ?? throw new ArgumentNullException(nameof(orderDetailsViewModelFactory));
            OrderUrlProvider = orderUrlProvider ?? throw new ArgumentNullException(nameof(orderUrlProvider));
            StoreRepository = storeRepository ?? throw new ArgumentNullException(nameof(storeRepository));
            InventoryLocationProvider = inventoryLocationProvider ?? throw new ArgumentNullException(nameof(inventoryLocationProvider));

            _updateOperations = new Dictionary<string, UpdateOperation>();

            RegisterCartUpdateOperation<AddressViewModel>("ShippingAddress", UpdateShippingAddress, 1);
            RegisterCartUpdateOperation<RegisteredShippingAddressViewModel>("ShippingAddressRegistered", UpdateRegisteredShippingAddress, 1);
            RegisterCartUpdateOperation<PickUpAddressViewModel>("PickUpAddress", UpdatePickUpAddress, 1);
            RegisterCartUpdateOperation<CustomerSummaryViewModel>("GuestCustomerInfo", UpdateCustomer, 2);
            RegisterCartUpdateOperation<BillingAddressViewModel>("BillingAddress", UpdateBillingAddress, 3);
            RegisterCartUpdateOperation<RegisteredBillingAddressViewModel>("BillingAddressRegistered", UpdateRegisteredBillingAddress, 3);
            RegisterCartUpdateOperation<ShippingMethodViewModel>("ShippingMethod", UpdateShippingMethod, 4);
        }

        public virtual async Task<CompleteCheckoutViewModel> CompleteCheckoutAsync(CompleteCheckoutParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.CartName)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.CartName)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope)), nameof(param)); }
            if (param.CustomerId == Guid.Empty) { throw new ArgumentException(GetMessageOfEmpty(nameof(param.CustomerId)), nameof(param)); }

            var order = await CartRepository.CompleteCheckoutAsync(param).ConfigureAwait(false);

            return await MapOrderToCompleteCheckoutViewModel(order, param).ConfigureAwait(false); 
        }

        protected virtual async Task<CompleteCheckoutViewModel> MapOrderToCompleteCheckoutViewModel(Overture.ServiceModel.Orders.Order order,
            CompleteCheckoutParam param)
        {
            if (order == null) { return null; }

            var orderStatuses = await LookupService.GetLookupDisplayNamesAsync(new GetLookupDisplayNamesParam
            {
                CultureInfo = param.CultureInfo,
                LookupType = LookupType.Order,
                LookupName = "OrderStatus",
            }).ConfigureAwait(false);

            var productImageInfo = new ProductImageInfo
            {
                ImageUrls = await ImageService.GetImageUrlsAsync(order.Cart.GetLineItems()).ConfigureAwait(false)
            };

            var getVmOrderParam = new CreateOrderDetailViewModelParam { 
                Order = order,
                CultureInfo = param.CultureInfo,
                OrderStatuses = orderStatuses,
                OrderDetailBaseUrl = OrderUrlProvider.GetOrderDetailsBaseUrl(param.CultureInfo),
                BaseUrl = param.BaseUrl,
                ProductImageInfo = productImageInfo,
            };

            var orderViewModel = OrderDetailsViewModelFactory.CreateLightViewModel(getVmOrderParam);

            var completeCheckoutViewModel = new CompleteCheckoutViewModel
            {
                OrderNumber = order.OrderNumber,
                Order = orderViewModel,
                CustomerEmail = order.Cart.Customer.Email,
                CustomerFirstName = order.Cart.Customer.FirstName,
                CustomerLastName = order.Cart.Customer.LastName,
                Affiliation = order.Cart.OrderLocation?.Name,
                Revenu = order.Cart.Total,
                Tax = order.Cart.TaxTotal,
                Shipping = order.Cart.FulfillmentCost,
                ShippingOptions = order.Cart.Shipments?.FirstOrDefault()?.FulfillmentMethod.FulfillmentMethodType.ToString().ToLowerInvariant(),
                BillingCurrency = order.Cart.BillingCurrency,
                Coupons = MapCoupons(order, param.CultureInfo),
                LineItems = orderViewModel?.Shipments.FirstOrDefault()?.LineItems
            };

            return completeCheckoutViewModel;
        }

        protected virtual List<CouponViewModel> MapCoupons(Overture.ServiceModel.Orders.Order order, CultureInfo cultureInfo)
        {
            if (order?.Cart == null) { return new List<CouponViewModel>(); }

            var couponsViewModel = CartViewModelFactory.GetCouponsViewModel(order.Cart, cultureInfo, false);

            return couponsViewModel != null ? couponsViewModel.ApplicableCoupons : new List<CouponViewModel>();
        }

        /// <summary>
        /// Update a Cart during the checkout process.
        /// </summary>
        /// <param name="param">UpdateCheckoutCartParam</param>
        public virtual async Task<UpdateCartResultViewModel> UpdateCheckoutCartAsync(UpdateCheckoutCartParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (param.GetCartParam == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.GetCartParam)), nameof(param)); }
            if (param.UpdateValues == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.UpdateValues)), nameof(param)); }

            var cart = await CartRepository.GetCartAsync(param.GetCartParam).ConfigureAwait(false);

            if (cart == null) { throw new InvalidOperationException("No cart found"); }

            SetCustomerType(param.IsGuest, cart);
            UpdateStepInfo(cart, param.CurrentStep + 1);

            var operationsToExecute = GetOperationsToExecute(param.UpdateValues);

            foreach (var operation in operationsToExecute)
            {
                await operation.Invoke(cart);
            }

            var updatedCart = await CartRepository.UpdateCartAsync(UpdateCartParamFactory.Build(cart));

            var createCartViewModelParam = new CreateCartViewModelParam
            {
                Cart = updatedCart,
                CultureInfo = param.GetCartParam.CultureInfo,
                BaseUrl = param.GetCartParam.BaseUrl
            };

            var cartViewModel = await CreateCartViewModelAsync(createCartViewModelParam);

            var updateCartResultViewModel = new UpdateCartResultViewModel
            {
                HasErrors = updatedCart.Messages != null && updatedCart.Messages
                .Any(m => m.Severity == ExecutionMessageSeverity.Error || m.Severity == ExecutionMessageSeverity.Failure),
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
            if (cart.Customer == null || !isGuest) { cart.Customer = new CustomerSummary(); }
            cart.Customer.Type = isGuest ? CustomerType.Guest : CustomerType.Registered;
        }

        protected virtual async Task<CartViewModel> CreateCartViewModelAsync(CreateCartViewModelParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (param.Cart == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.Cart)), nameof(param)); }
            if (param.BaseUrl == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.BaseUrl)), nameof(param)); }

            param.ProductImageInfo = new ProductImageInfo
            {
                ImageUrls = await ImageService.GetImageUrlsAsync(param.Cart.GetLineItems()).ConfigureAwait(false)
            };

            var methodDisplayNames = await LookupService.GetLookupDisplayNamesAsync(new GetLookupDisplayNamesParam
            {
                CultureInfo = param.CultureInfo,
                LookupType = LookupType.Order,
                LookupName = "PaymentMethodType",
            });

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
        public virtual void RegisterCartUpdateOperation<TViewModel>(string operationName, Func<Overture.ServiceModel.Orders.Cart, TViewModel, Task> updateAction, int? order)
        {
            if (string.IsNullOrEmpty(operationName)) { throw new ArgumentException(GetMessageOfNullEmpty(), nameof(operationName)); }
            if (updateAction == null) { throw new ArgumentNullException(nameof(updateAction)); }

            var operation = new Func<Overture.ServiceModel.Orders.Cart, object, Task>((cart, viewModel) => updateAction(cart, ParseJson<TViewModel>(viewModel)));
            _updateOperations[operationName] = order == null ? new UpdateOperation(operation) : new UpdateOperation(order.Value, operation);
        }

        protected TViewModel ParseJson<TViewModel>(object viewModel)
        {
            var jsonValue = viewModel as string ?? throw new InvalidDataException("Value must be a string");

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
            if (cart.PropertyBag == null) { cart.PropertyBag = new PropertyBag(); }

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
            if (customerSummaryViewModel == null) { return Task.FromResult(0); }

            if (cart.Customer == null) { cart.Customer = new CustomerSummary(); }

            cart.Customer.LastName = customerSummaryViewModel.LastName;
            cart.Customer.FirstName = customerSummaryViewModel.FirstName;
            cart.Customer.MiddleName = customerSummaryViewModel.MiddleName;
            cart.Customer.Phone = customerSummaryViewModel.Phone;
            cart.Customer.Email = customerSummaryViewModel.Email;

            return Task.FromResult(0);
        }

        protected virtual async Task UpdateShippingAddress(Overture.ServiceModel.Orders.Cart cart, AddressViewModel addressViewModel)
        {
            if (addressViewModel == null) { return; }

            var shipment = cart.Shipments.FirstOrDefault();

            if (shipment == null) { return; }

            var newAddress = CreateAddressFromViewModel(addressViewModel);

            var isShippingChanged = shipment.Address == null || !IsEqual(shipment.Address, newAddress);

            shipment.Address = newAddress;

            if (shipment.FulfillmentMethod == null)
            {
                await ShippingMethodViewService.EstimateShippingAsync(new EstimateShippingParam
                {
                    Cart = cart,
                    CultureInfo = CultureInfo.GetCultureInfo(cart.CultureName), //TODO: Fix me
                    ForceUpdate = isShippingChanged

                }).ConfigureAwait(false);
            }
        }

        protected virtual async Task UpdateShippingMethod(Overture.ServiceModel.Orders.Cart cart, ShippingMethodViewModel shippingMethodViewModel)
        {
            if (string.IsNullOrEmpty(shippingMethodViewModel?.Name)) { return; }

            var shipment = cart.Shipments.FirstOrDefault();

            if (shipment == null) { return; }

            if (shipment.PickUpLocationId.HasValue)
            {
                shipment.PickUpLocationId = null;
                shipment.Address = null;
                var fulfillmentLocation = await InventoryLocationProvider.GetFulfillmentLocationAsync(new GetFulfillmentLocationParam
                {
                    Scope = cart.ScopeId
                }).ConfigureAwait(false);
                shipment.FulfillmentLocationId = fulfillmentLocation.Id;
            }

            shipment.FulfillmentMethod = await GetFulfillmentMethodAsync(cart, shippingMethodViewModel);
        }

        protected virtual async Task<FulfillmentMethod> GetFulfillmentMethodAsync(Overture.ServiceModel.Orders.Cart cart, ShippingMethodViewModel shippingMethodViewModel)
        {
            var param = new GetShippingMethodsParam
            {
                CartName = cart.Name,
                CustomerId = cart.CustomerId,
                CultureInfo = new CultureInfo(cart.CultureName),
                Scope = cart.ScopeId
            };

            var fulfillmentMethods = await FulfillmentMethodRepository.GetCalculatedFulfillmentMethods(param);

            var fulfillmentMethod = fulfillmentMethods.Find(method =>
                method.Name == shippingMethodViewModel.Name &&
                method.ShippingProviderId == shippingMethodViewModel.ShippingProviderId);

            if (fulfillmentMethod == null)
            {
                throw new ComposerException(new ErrorViewModel
                {
                    ErrorCode = string.Empty,
                    ErrorMessage = "Unable to find any shipment provider matching the provided parameters."
                });
            }

            return fulfillmentMethod;
        }

        protected virtual Task UpdateBillingAddress(Overture.ServiceModel.Orders.Cart cart, BillingAddressViewModel billingAddressViewModel)
        {
            if (billingAddressViewModel == null) { return Task.FromResult(0); }

            var payment = GetPayment(cart);

            if (payment == null) { return Task.FromResult(0); }

            if (billingAddressViewModel.UseShippingAddress)
            {
                var shipment = cart.Shipments.FirstOrDefault();

                if (shipment?.Address == null) { return Task.FromResult(0); }

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
            if (registeredBillingAddressViewModel == null) { return; }

            var payment = cart.Payments.FirstOrDefault();

            if (payment == null) { return; }

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
                if (registeredBillingAddressViewModel.BillingAddressId == Guid.Empty) { return; }

                var newAddress = await AddressRepository.GetAddressByIdAsync(registeredBillingAddressViewModel.BillingAddressId).ConfigureAwait(false);

                if(newAddress == null)
                {
                    return;
                }

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
            if (registeredShippingAddressViewModel == null || registeredShippingAddressViewModel.ShippingAddressId == Guid.Empty) { return; }

            var shipment = cart.Shipments.FirstOrDefault();

            if (shipment == null) { return; }

            var newAddress = await AddressRepository.GetAddressByIdAsync(registeredShippingAddressViewModel.ShippingAddressId).ConfigureAwait(false);
            var isShippingChanged = shipment.Address == null || !IsEqual(shipment.Address, newAddress);

            if (isShippingChanged)
            {
                shipment.Address = newAddress;
                shipment.Address.PropertyBag[AddressBookIdPropertyBagKey] = newAddress.Id;
                shipment.Address.Id = Guid.Empty;
            }

            //In the case the user didn't do estimate shipping before
            if (shipment.FulfillmentMethod == null)
            {
                await ShippingMethodViewService.EstimateShippingAsync(new EstimateShippingParam
                {
                    Cart = cart,
                    CultureInfo = CultureInfo.GetCultureInfo(cart.CultureName), //TODO: Fix me
                    ForceUpdate = isShippingChanged

                }).ConfigureAwait(false);
            }
        }

        protected virtual async Task UpdatePickUpAddress(Overture.ServiceModel.Orders.Cart cart, PickUpAddressViewModel pickUpAddressViewModel)
        {
            if (pickUpAddressViewModel == null || pickUpAddressViewModel.PickUpLocationId == Guid.Empty)
            {
                return;
            }

            var shipment = cart.Shipments.FirstOrDefault();

            if (shipment == null)
            {
                return;
            }

            shipment.PickUpLocationId = pickUpAddressViewModel.PickUpLocationId;
            shipment.FulfillmentLocationId = pickUpAddressViewModel.PickUpLocationId;

            var store = await StoreRepository.GetStoreAsync(new Store.Parameters.GetStoreParam
            {
                Id = pickUpAddressViewModel.PickUpLocationId,
                CultureInfo = new CultureInfo(cart.CultureName),
                Scope = cart.ScopeId
            }).ConfigureAwait(false);
            
            var storeAddress = store.FulfillmentLocation.Addresses.FirstOrDefault();

            var address = new Address
            {
                AddressName = store.Name,
                City = storeAddress.City,
                CountryCode = storeAddress.CountryCode,
                FirstName = cart.Customer.FirstName,
                LastName = cart.Customer.LastName,
                Line1 = storeAddress.Line1,
                Line2 = storeAddress.Line2,
                PhoneNumber = storeAddress.PhoneNumber,
                PostalCode = storeAddress.PostalCode,
                RegionCode = storeAddress.RegionCode,
                PropertyBag = storeAddress.PropertyBag
            };
            shipment.Address = address;
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
                RegionCode = addressViewModel.RegionCode,
                PropertyBag = new PropertyBag()
            };

            if (addressViewModel.Bag != null)
            {
                address.PropertyBag = new PropertyBag(addressViewModel.Bag);
            }

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
            return cart.Payments.Find(x => !x.IsVoided()) ?? throw new InvalidOperationException("No valid payment was found in the cart.");
        }
    }
}