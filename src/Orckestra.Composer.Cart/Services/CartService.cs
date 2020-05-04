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
using Orckestra.Composer.Providers.Dam;
using Orckestra.Composer.Services;
using Orckestra.Composer.Services.Lookup;
using Orckestra.Overture.ServiceModel;
using Orckestra.Overture.ServiceModel.Orders;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;

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
        protected IImageService ImageService { get; private set; }

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
        /// <param name="imageService">The <see cref="IImageService"/></param>
        public CartService(
            ICartRepository cartRepository, 
            IDamProvider damProvider, 
            ICartViewModelFactory cartViewModelFactory,
            ICouponViewService couponViewService,
            ILookupService lookupService, 
            ILineItemService lineItemService,
            IFixCartService fixCartService,
            ICountryService countryService,
            IRegionCodeProvider regionCodeProvider,
            IImageService imageService)
        {
            CartRepository = cartRepository ?? throw new ArgumentNullException(nameof(cartRepository));
            CartViewModelFactory = cartViewModelFactory ?? throw new ArgumentNullException(nameof(cartViewModelFactory));
            CouponViewService = couponViewService ?? throw new ArgumentNullException(nameof(couponViewService));
            LookupService = lookupService ?? throw new ArgumentNullException(nameof(lookupService));
            LineItemService = lineItemService ?? throw new ArgumentNullException(nameof(lineItemService));
            FixCartService = fixCartService ?? throw new ArgumentNullException(nameof(fixCartService));
            CountryService = countryService ?? throw new ArgumentNullException(nameof(fixCartService));
            RegionCodeProvider = regionCodeProvider ?? throw new ArgumentNullException(nameof(regionCodeProvider));
            ImageService = imageService ?? throw new ArgumentNullException(nameof(imageService));
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
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope)), nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.CartName)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.CartName)), nameof(param)); }
            if (param.CustomerId == Guid.Empty) { throw new ArgumentException(GetMessageOfEmpty(nameof(param.CustomerId)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.ProductId)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.ProductId)), nameof(param)); }
            if (param.Quantity < 1) { throw new ArgumentOutOfRangeException(nameof(param), param.Quantity, GetMessageOfZeroNegative(nameof(param.Quantity))); }
            if (string.IsNullOrWhiteSpace(param.BaseUrl)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.BaseUrl)), nameof(param)); }

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
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope)), nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.CartName)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.CartName)), nameof(param)); }
            if (param.LineItemId == Guid.Empty) { throw new ArgumentException(GetMessageOfEmpty(nameof(param.LineItemId)), nameof(param)); }
            if (param.CustomerId == Guid.Empty) { throw new ArgumentException(GetMessageOfEmpty(nameof(param.CustomerId)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.BaseUrl)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.BaseUrl)), nameof(param)); }

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
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.CartName)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.CartName)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.ScopeId)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.ScopeId)), nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.CartName)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.CartName)), nameof(param)); }
            if (param.LineItemId == Guid.Empty) { throw new ArgumentException(GetMessageOfEmpty(nameof(param.LineItemId)), nameof(param)); }
            if (param.CustomerId == Guid.Empty) { throw new ArgumentException(GetMessageOfEmpty(nameof(param.CustomerId)), nameof(param)); }
            if (param.Quantity < 1) { throw new ArgumentOutOfRangeException(nameof(param), param.Quantity, GetMessageOfZeroNegative(nameof(param.Quantity))); }
            if (string.IsNullOrWhiteSpace(param.BaseUrl)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.BaseUrl)), nameof(param)); }

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
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (param.Coupons == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.Coupons)), nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)), nameof(param)); }
            if (param.Shipments == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.Shipments)), nameof(param)); }
            if (param.CustomerId == Guid.Empty) { throw new ArgumentException(GetMessageOfEmpty(nameof(param.CustomerId)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.BaseUrl)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.BaseUrl)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.BillingCurrency)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.BillingCurrency)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.CartName)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.CartName)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.CartType)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.CartType)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Status)){ throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Status)), nameof(param)); }

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
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope)), nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.CartName)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.CartName)), nameof(param)); }
            if (param.CustomerId == Guid.Empty) { throw new ArgumentException(GetMessageOfEmpty(nameof(param.CustomerId)), nameof(param)); }

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
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (param.Cart == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.Cart)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.BaseUrl)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.BaseUrl)), nameof(param)); }

            param.ProductImageInfo = new ProductImageInfo
            {
                ImageUrls = await ImageService.GetImageUrlsAsync(param.Cart.GetLineItems()).ConfigureAwait(false),
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
            if (param == null) { throw new ArgumentNullException(nameof(param)); }

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
            if (param == null) { throw new ArgumentNullException(nameof(param)); }

            ProcessedCart cart = await CartRepository.GetCartAsync(new GetCartParam
            {
                BaseUrl = param.BaseUrl,
                Scope = param.Scope,
                CultureInfo = param.CultureInfo,
                CustomerId = param.CustomerId,
                CartName = param.CartName

            }).ConfigureAwait(false);

            var shipment = cart?.Shipments.FirstOrDefault() ?? throw new InvalidOperationException("No shipment was found in the cart.");

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

            if (shipment.Address.PropertyBag != null)
            {
                shipment.Address.PropertyBag[AddressBookIdPropertyBagKey] = Guid.Empty; // because the updated address will not correspond to any registered address
            }

            shipment.Address.PostalCode = param.PostalCode;
            shipment.Address.CountryCode = country.IsoCode;
            shipment.Address.RegionCode = GetRegionCodeBasedOnPostalCode(param.PostalCode, param.CountryCode);
        }

        public virtual async Task<CartViewModel> UpdateBillingAddressPostalCodeAsync(UpdateBillingAddressPostalCodeParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }

            ProcessedCart cart = await CartRepository.GetCartAsync(new GetCartParam
            {
                BaseUrl = param.BaseUrl,
                Scope = param.Scope,
                CultureInfo = param.CultureInfo,
                CustomerId = param.CustomerId,
                CartName = param.CartName

            }).ConfigureAwait(false);

            var payment = cart.Payments?.Find(x => !x.IsVoided()) 
            ?? throw new InvalidOperationException("There is no valid payment from which we can get or set the billing address");

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
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (param.CustomerId == Guid.Empty) { throw new ArgumentException(GetMessageOfEmpty(nameof(param.CustomerId)), nameof(param)); }
            if (param.PaymentMethodId == Guid.Empty) { throw new ArgumentException(GetMessageOfEmpty(nameof(param.PaymentMethodId)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.PaymentProviderName)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.PaymentProviderName)), nameof(param)); }

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

            if(payment.BillingAddress.PropertyBag != null)
                payment.BillingAddress.PropertyBag[AddressBookIdPropertyBagKey] = Guid.Empty; // because the updated address will not correspond to any registered address
            payment.BillingAddress.PostalCode = param.PostalCode;
            payment.BillingAddress.CountryCode = country.IsoCode;
            payment.BillingAddress.RegionCode = GetRegionCodeBasedOnPostalCode(param.PostalCode, param.CountryCode);
        }

        protected virtual string GetRegionCodeBasedOnPostalCode(string postalCode, string countryCode)
        {
            var region = RegionCodeProvider.GetRegion(postalCode, countryCode) ?? 
                throw new ArgumentException("Cannot resolve a Region based on this Postal Code", "postalCode");
            return region;
        }
    }
}