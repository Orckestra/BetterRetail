using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Orckestra.Composer.Cart.Extensions;
using Orckestra.Composer.Cart.Factory;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Repositories;
using Orckestra.Composer.Cart.ViewModels;
using Orckestra.Composer.Country;
using Orckestra.Composer.Enums;
using Orckestra.Composer.Extensions;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Services.Lookup;
using Orckestra.Composer.Utils;
using Orckestra.Overture.ServiceModel;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Cart.Services
{
    /// <summary>
    /// Service for dealing with Cart.
    /// </summary>
    public class CartService : ICartService
    {
        protected const string AddressBookIdPropertyBagKey = "AddressBookId";

        protected ICartRepository CartRepository { get; private set; }
        protected ICartViewModelFactory CartViewModelFactory { get; private set; }
        protected ICouponViewService CouponViewService { get; private set; }
        protected ILookupService LookupService { get; private set; }
        protected ILineItemService LineItemService { get; private set; }
        protected IFixCartService FixCartService { get; private set; }
        protected ICountryService CountryService { get; private set; }
        protected IRegionCodeProvider RegionCodeProvider { get; private set; }

        /// <summary>
        /// CartService constructor
        /// </summary>
        /// <param name="cartRepository">The repository for accessing cart data</param>
        /// <param name="damProvider">The provider for providing images</param>
        /// <param name="cartViewModelFactory">Factory creating a <see cref="CartViewModel"/>.</param>
        /// <param name="couponViewService">The <see cref="ICouponViewService"/>.</param>
        /// <param name="lookupService">The <see cref="ILookupService"/>.</param>
        /// <param name="lineItemService">The <see cref="ILineItemService"/>.</param>
        /// <param name="fixCartService">The <see cref="IFixCartService"/>.</param>
        /// <param name="countryService">The <see cref="ICountryService"/></param>
        /// <param name="regionCodeProvider">The <see cref="IRegionCodeProvider"/></param>
        public CartService(
            ICartRepository cartRepository, 
            IDamProvider damProvider, 
            ICartViewModelFactory cartViewModelFactory,
            ICouponViewService couponViewService,
            ILookupService lookupService, 
            ILineItemService lineItemService,
            IFixCartService fixCartService,
            ICountryService countryService,
            IRegionCodeProvider regionCodeProvider)
        {
            if (cartRepository == null) { throw new ArgumentNullException("cartRepository"); }
            if (damProvider == null) { throw new ArgumentNullException("damProvider"); }
            if (cartViewModelFactory == null) { throw new ArgumentNullException("cartViewModelFactory"); }
            if (couponViewService == null) { throw new ArgumentNullException("couponViewService"); }
            if (lookupService == null) { throw new ArgumentNullException("lookupService"); }
            if (lineItemService == null) { throw new ArgumentNullException("lineItemService"); }
            if (fixCartService == null) { throw new ArgumentNullException("fixCartService"); }
            if (countryService == null) { throw new ArgumentNullException("fixCartService"); }
            if (regionCodeProvider == null) { throw new ArgumentNullException("regionCodeProvider"); }

            CartRepository = cartRepository;
            CartViewModelFactory = cartViewModelFactory;
            CouponViewService = couponViewService;
            LookupService = lookupService;
            LineItemService = lineItemService;
            FixCartService = fixCartService;
            CountryService = countryService;
            RegionCodeProvider = regionCodeProvider;
        }

        /// <summary>
        /// Add line item to the cart.
        /// 
        /// CartName will be created if needed
        /// CustomerId (guest customers) will be created if needed
        /// If the (product,variant) is already in the cart, the quantity will be increased; 
        /// otherwise a new line will be added
        /// 
        /// param.VariantId is optional
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <returns>The Lightweight CartViewModel</returns>
        public async virtual Task<CartViewModel> AddLineItemAsync(AddLineItemParam param)
        {
            if (param == null) { throw new ArgumentNullException("param", "param is required"); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException("param.Scope is required", "param"); }
            if (param.CultureInfo == null) { throw new ArgumentException("param.CultureInfo", "param"); }
            if (string.IsNullOrWhiteSpace(param.CartName)) { throw new ArgumentException("param.CartName is required", "param"); }
            if (param.CustomerId == Guid.Empty) { throw new ArgumentException("param.CustomerId is required", "param"); }
            if (string.IsNullOrWhiteSpace(param.ProductId)) { throw new ArgumentException("param.ProductId is required", "param"); }
            if (param.Quantity <= 0) { throw new ArgumentException("param.Quantity is required", "param"); }
            if (string.IsNullOrWhiteSpace(param.BaseUrl)) { throw new ArgumentException("param.BaseUrl is required", "param"); }

            var cart = await CartRepository.AddLineItemAsync(param).ConfigureAwait(false);

            var fixedCart = await FixCartService.FixCartAsync(new FixCartParam
            {
                Cart = cart
            }).ConfigureAwait(false);

            var vmParam = new CreateCartViewModelParam
            {
                Cart = fixedCart,
                CultureInfo = param.CultureInfo,
                IncludeInvalidCouponsMessages = false,
                BaseUrl = param.BaseUrl
            };

            var viewModel = await CreateCartViewModelAsync(vmParam).ConfigureAwait(false);

            return viewModel;
        }

        /// <summary>
        /// Remove line item to the cart.
        /// </summary>
        /// <param name="param"></param>
        /// <returns>The Lightweight CartViewModel</returns>
        public async virtual Task<CartViewModel> RemoveLineItemAsync(RemoveLineItemParam param)
        {
            if (param == null) { throw new ArgumentNullException("param", "param is required"); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException("param.Scope is required", "param"); }
            if (param.CultureInfo == null) { throw new ArgumentException("param.CultureInfo is required", "param"); }
            if (string.IsNullOrWhiteSpace(param.CartName)) { throw new ArgumentException("param.CartName is required", "param"); }
            if (param.LineItemId == Guid.Empty) { throw new ArgumentException("param.LineItemId is required", "param"); }
            if (param.CustomerId == Guid.Empty) { throw new ArgumentException("param.CustomerId is required", "param"); }
            if (string.IsNullOrWhiteSpace(param.BaseUrl)) { throw new ArgumentException("param.BaseUrl is required", "param"); }

            var cart = await CartRepository.RemoveLineItemAsync(param).ConfigureAwait(false);

            await CartRepository.RemoveCouponsAsync(new RemoveCouponsParam
            {
                CartName = param.CartName,
                CouponCodes = CouponViewService.GetInvalidCouponsCode(cart.Coupons).ToList(),
                CustomerId = param.CustomerId,
                Scope = param.Scope,
            }).ConfigureAwait(false);

            var vmParam = new CreateCartViewModelParam
            {
                Cart = cart,
                CultureInfo = param.CultureInfo,
                IncludeInvalidCouponsMessages = true,
                BaseUrl = param.BaseUrl
            };

            var viewModel = await CreateCartViewModelAsync(vmParam).ConfigureAwait(false);

            return viewModel;
        }

        /// <summary>
        /// Update a line item in the cart.
        /// </summary>
        /// <param name="param"></param>
        /// <returns>The Lightweight CartViewModel</returns>
        public async virtual Task<CartViewModel> UpdateLineItemAsync(UpdateLineItemParam param)
        {
            if (param == null) { throw new ArgumentNullException("param", "param is required"); }
            if (string.IsNullOrWhiteSpace(param.CartName)) { throw new ArgumentException("param.CartName is required", "param"); }
            if (string.IsNullOrWhiteSpace(param.ScopeId)) { throw new ArgumentException("param.ScopeId is required", "param"); }
            if (param.CultureInfo == null) { throw new ArgumentException("param.CultureInfo is required", "param"); }
            if (string.IsNullOrWhiteSpace(param.CartName)) { throw new ArgumentException("param.CartName is required", "param"); }
            if (param.LineItemId == Guid.Empty) { throw new ArgumentException("param.LineItemId is required", "param"); }
            if (param.CustomerId == Guid.Empty) { throw new ArgumentException("param.CustomerId is required", "param"); }
            if (param.Quantity < 1) { throw new ArgumentException("param.Quantity must be greater than 0", "param"); }
            if (string.IsNullOrWhiteSpace(param.BaseUrl)) { throw new ArgumentException("param.BaseUrl is required", "param"); }

            var cart = await CartRepository.UpdateLineItemAsync(param).ConfigureAwait(false);

            await CartRepository.RemoveCouponsAsync(new RemoveCouponsParam
            {
                CartName = param.CartName,
                CouponCodes = CouponViewService.GetInvalidCouponsCode(cart.Coupons).ToList(),
                CustomerId = param.CustomerId,
                Scope = param.ScopeId
            }).ConfigureAwait(false);

            var vmParam = new CreateCartViewModelParam
            {
                Cart = cart,
                CultureInfo = param.CultureInfo,
                IncludeInvalidCouponsMessages = true,
                BaseUrl = param.BaseUrl
            };

            var viewModel = await CreateCartViewModelAsync(vmParam).ConfigureAwait(false);
            
            return viewModel;
        }

        /// <summary>
        /// Update the Cart
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual async Task<CartViewModel> UpdateCartAsync(UpdateCartViewModelParam param)
        {
            if (param == null) { throw new ArgumentNullException("param", "param"); }
            if (param.Coupons == null) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("Coupons"), "param"); }
            if (param.CultureInfo == null) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("CultureInfo"), "param"); }
            if (param.Shipments == null) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("Shipments"), "param"); }
            if (param.CustomerId == Guid.Empty) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("CustomerId"), "param"); }
            if (string.IsNullOrWhiteSpace(param.BaseUrl)) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("BaseUrl"), "param"); }
            if (string.IsNullOrWhiteSpace(param.BillingCurrency)) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("BillingCurrency"), "param"); }
            if (string.IsNullOrWhiteSpace(param.CartName)) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("CartName"), "param"); }
            if (string.IsNullOrWhiteSpace(param.CartType)) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("CartType"), "param"); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("Scope"), "param"); }
            if (string.IsNullOrWhiteSpace(param.Status)){ throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("Status"), "param"); }

            var updatedCart = await CartRepository.UpdateCartAsync(param).ConfigureAwait(false);

            await CartRepository.RemoveCouponsAsync(new RemoveCouponsParam
            {
                CartName = param.CartName,
                CouponCodes = CouponViewService.GetInvalidCouponsCode(param.Coupons).ToList(),
                CustomerId = param.CustomerId,
                Scope = param.Scope
            }).ConfigureAwait(false);

            var vmParam = new CreateCartViewModelParam
            {
                Cart = updatedCart,
                CultureInfo = param.CultureInfo,
                IncludeInvalidCouponsMessages = true,
                BaseUrl = param.BaseUrl
            };

            var viewModel = await CreateCartViewModelAsync(vmParam).ConfigureAwait(false);
            
            return viewModel;
        }

        /// <summary>
        /// Retrieve a cart
        /// </summary>
        /// <param name="param"></param>
        /// <returns>The Lightweight CartViewModel</returns>
        public virtual async Task<CartViewModel> GetCartViewModelAsync(GetCartParam param)
        {
            if (param == null) { throw new ArgumentNullException("param", "param is required"); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException("param.Scope is required", "param"); }
            if (param.CultureInfo == null) { throw new ArgumentException("param.CultureInfo is required", "param"); }
            if (string.IsNullOrWhiteSpace(param.CartName)) { throw new ArgumentException("param.CartName is required", "param"); }
            if (param.CustomerId == Guid.Empty) { throw new ArgumentException("param.CustomerId is required", "param"); }

            var cart = await CartRepository.GetCartAsync(param).ConfigureAwait(false);            

            await CartRepository.RemoveCouponsAsync(new RemoveCouponsParam
            {
                CartName = param.CartName,
                CustomerId = param.CustomerId,
                Scope = param.Scope,
                CouponCodes = CouponViewService.GetInvalidCouponsCode(cart.Coupons).ToList()
            }).ConfigureAwait(false);

            var vmParam = new CreateCartViewModelParam
            {
                Cart = cart,
                CultureInfo = param.CultureInfo,
                IncludeInvalidCouponsMessages = false,
                BaseUrl = param.BaseUrl
            };

            var viewModel = await CreateCartViewModelAsync(vmParam).ConfigureAwait(false);

            return viewModel;
        }

        //TODO: Would it be possible to add cache here too without too much problem?
        //TODO: Because it is called 3 times when you retrieve the Cart
        protected virtual async Task<CartViewModel> CreateCartViewModelAsync(CreateCartViewModelParam param)
        {
            if (param == null) { throw new ArgumentNullException("param"); }
            if (param.Cart == null) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("Cart"), "param"); }
            if (string.IsNullOrWhiteSpace(param.BaseUrl)) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("BaseUrl"), "param"); }

            param.ProductImageInfo = new ProductImageInfo
            {
                ImageUrls = await LineItemService.GetImageUrlsAsync(param.Cart.GetLineItems()).ConfigureAwait(false),
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

        public virtual async Task<CartViewModel> RemoveInvalidLineItemsAsync(RemoveInvalidLineItemsParam param)
        {
            if (param == null) { throw new ArgumentNullException("param"); }

            var cart = await CartRepository.GetCartAsync(new GetCartParam
            {
                CartName = param.CartName,
                CultureInfo = param.CultureInfo,
                CustomerId = param.CustomerId,
                ExecuteWorkflow = param.ExecuteWorkflow,
                Scope = param.Scope,
                WorkflowToExecute = param.WorkflowToExecute
            }).ConfigureAwait(false);

            var invalidLineItems = LineItemService.GetInvalidLineItems(cart);

            if (invalidLineItems.Any())
            {
                var p = new RemoveLineItemsParam
                {
                    CartName = param.CartName,
                    CultureInfo = param.CultureInfo,
                    CustomerId = param.CustomerId,
                    Scope = param.Scope,
                    LineItems = invalidLineItems.Select(li => new RemoveLineItemsParam.LineItemDescriptor
                    {
                        Id = li.Id,
                        ProductId = li.ProductId,
                        VariantId = li.VariantId
                    }).ToList()
                };

                cart = await CartRepository.RemoveLineItemsAsync(p).ConfigureAwait(false);
            }

            var vmParam = new CreateCartViewModelParam
            {
                Cart = cart,
                CultureInfo = param.CultureInfo,
                BaseUrl = param.BaseUrl
            };

            var viewModel = await CreateCartViewModelAsync(vmParam).ConfigureAwait(false);

            return viewModel;
        }

        public virtual async Task<CartViewModel> UpdateShippingAddressPostalCodeAsync(UpdateShippingAddressPostalCodeParam param)
        {
            if (param == null) { throw new ArgumentNullException("param"); }

            ProcessedCart cart = await CartRepository.GetCartAsync(new GetCartParam
            {
                BaseUrl = param.BaseUrl,
                Scope = param.Scope,
                CultureInfo = param.CultureInfo,
                CustomerId = param.CustomerId,
                CartName = param.CartName

            }).ConfigureAwait(false);

            if (cart.Shipments == null || !cart.Shipments.Any())
            {
                throw new InvalidOperationException("No shipment was found in the cart.");
            }

            Shipment shipment = cart.Shipments.First();

            await MapShippingAddressPostalCodeToShipmentAsync(param, shipment);

            return await UpdateCartAsync(new UpdateCartViewModelParam
            {
                BaseUrl = param.BaseUrl,
                BillingCurrency = cart.BillingCurrency,
                CartName = cart.Name,
                CartType = cart.CartType,
                Coupons = cart.Coupons,
                CultureInfo = param.CultureInfo,
                Customer = cart.Customer,
                CustomerId = cart.CustomerId,
                OrderLocation = cart.OrderLocation,
                Payments = cart.Payments,
                PropertyBag = cart.PropertyBag,
                Scope = cart.ScopeId,
                Shipments = cart.Shipments,
                Status = cart.Status

            }).ConfigureAwait(false);
        }

        protected virtual async Task MapShippingAddressPostalCodeToShipmentAsync(UpdateShippingAddressPostalCodeParam param, Shipment shipment)
        {
            var country = await CountryService.RetrieveCountryAsync(new RetrieveCountryParam
            {
                CultureInfo = param.CultureInfo,
                IsoCode = param.CountryCode

            }).ConfigureAwait(false);

            country.Validate(param.PostalCode);
            
            if (shipment.Address == null)
            {
                shipment.Address = new Address { PropertyBag = new PropertyBag() };
            }

            shipment.Address.PropertyBag[AddressBookIdPropertyBagKey] = Guid.Empty; // because the updated address will not correspond to any registered address
            shipment.Address.PostalCode = param.PostalCode;
            shipment.Address.CountryCode = country.IsoCode;
            shipment.Address.RegionCode = GetRegionCodeBasedOnPostalCode(param.PostalCode, param.CountryCode);
        }

        public virtual async Task<CartViewModel> UpdateBillingAddressPostalCodeAsync(UpdateBillingAddressPostalCodeParam param)
        {
            if (param == null) { throw new ArgumentNullException("param"); }

            ProcessedCart cart = await CartRepository.GetCartAsync(new GetCartParam
            {
                BaseUrl = param.BaseUrl,
                Scope = param.Scope,
                CultureInfo = param.CultureInfo,
                CustomerId = param.CustomerId,
                CartName = param.CartName

            }).ConfigureAwait(false);

            if (cart.Payments == null || cart.Payments.Any(x => x.IsVoided()))
            {
                throw new InvalidOperationException("There is no valid payment from which we can get or set the billing address");
            }

            var payment = cart.Payments.First(x => !x.IsVoided());
            if (payment.BillingAddress == null)
            {
                payment.BillingAddress = new Address() { PropertyBag = new PropertyBag() };
            }

            await MapBillingAddressPostalCodeToPaymentAsync(param, payment);
            
            return await UpdateCartAsync(new UpdateCartViewModelParam
            {
                BaseUrl = param.BaseUrl,
                BillingCurrency = cart.BillingCurrency,
                CartName = cart.Name,
                CartType = cart.CartType,
                Coupons = cart.Coupons,
                CultureInfo = param.CultureInfo,
                Customer = cart.Customer,
                CustomerId = cart.CustomerId,
                OrderLocation = cart.OrderLocation,
                Payments = cart.Payments,
                PropertyBag = cart.PropertyBag,
                Scope = cart.ScopeId,
                Shipments = cart.Shipments,
                Status = cart.Status

            }).ConfigureAwait(false);
        }

        public virtual async Task<IPaymentMethodViewModel> SetDefaultCustomerPaymentMethod(SetDefaultCustomerPaymentMethodParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param), ArgumentNullMessageFormatter.FormatErrorMessage(nameof(param))); }
            if (param.CustomerId == Guid.Empty) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage(nameof(param.CustomerId)), nameof(param.CustomerId)); }
            if (param.PaymentMethodId == Guid.Empty) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage(nameof(param.PaymentMethodId)), nameof(param.PaymentMethodId)); }
            if (String.IsNullOrWhiteSpace(param.PaymentProviderName)) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage(nameof(param.PaymentProviderName)), nameof(param.PaymentProviderName)); }

            var paymentMethod = await CartRepository.SetDefaultCustomerPaymentMethod(param).ConfigureAwait(false);

            return await MapPaymentMethodToViewModel(paymentMethod, param.Culture);
        }

        protected virtual async Task<IPaymentMethodViewModel> MapPaymentMethodToViewModel(PaymentMethod paymentMethod, CultureInfo culture)
        {
            var methodDisplayNames = await LookupService.GetLookupDisplayNamesAsync(new GetLookupDisplayNamesParam
            {
                CultureInfo = culture,
                LookupType = LookupType.Order,
                LookupName = "PaymentMethodType",
            }).ConfigureAwait(false);

            return CartViewModelFactory.GetPaymentMethodViewModel(paymentMethod, methodDisplayNames, culture);
        }

        protected virtual async Task MapBillingAddressPostalCodeToPaymentAsync(UpdateBillingAddressPostalCodeParam param, Payment payment)
        {
            var country = await CountryService.RetrieveCountryAsync(new RetrieveCountryParam
            {
                CultureInfo = param.CultureInfo,
                IsoCode = param.CountryCode

            }).ConfigureAwait(false);

            country.Validate(param.PostalCode);

            payment.BillingAddress.PropertyBag[AddressBookIdPropertyBagKey] = Guid.Empty; // because the updated address will not correspond to any registered address
            payment.BillingAddress.PostalCode = param.PostalCode;
            payment.BillingAddress.CountryCode = country.IsoCode;
            payment.BillingAddress.RegionCode = GetRegionCodeBasedOnPostalCode(param.PostalCode, param.CountryCode);
        }

        protected virtual string GetRegionCodeBasedOnPostalCode(string postalCode, string countryCode)
        {
            var region = RegionCodeProvider.GetRegion(postalCode, countryCode);
            if (region == null)
            {
                throw new ArgumentException("Cannot resolve a Region based on this Postal Code", "postalCode");
            }

            return region;
        }
    }
}
