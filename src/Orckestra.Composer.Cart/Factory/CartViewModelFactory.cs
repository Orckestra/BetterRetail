using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Orckestra.Composer.Cart.Extensions;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Repositories;
using Orckestra.Composer.Cart.ViewModels;
using Orckestra.Composer.Cart.ViewModels.Order;
using Orckestra.Composer.Country;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Dam;
using Orckestra.Composer.Providers.Localization;
using Orckestra.Composer.Services;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel;
using Orckestra.Overture.ServiceModel.Marketing;
using Orckestra.Overture.ServiceModel.Orders;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;
using Orckestra.Composer.Cart.ViewModels.Order;
using Orckestra.Composer.Providers.Dam;
using Orckestra.Composer.Parameters;

namespace Orckestra.Composer.Cart.Factory
{
    public class CartViewModelFactory : ICartViewModelFactory
    {
        private RetrieveCountryParam _countryParam;
        protected RetrieveCountryParam CountryParam
        {
            get
            {
                return _countryParam ?? (_countryParam = new RetrieveCountryParam
                {
                    CultureInfo = ComposerContext.CultureInfo,
                    IsoCode = ComposerContext.CountryCode
                });
            }
        }

        protected ILocalizationProvider LocalizationProvider { get; private set; }
        protected IViewModelMapper ViewModelMapper { get; private set; }
        protected IFulfillmentMethodRepository FulfillmentMethodRepository { get; private set; }
        protected ICountryService CountryService { get; private set; }
        protected IComposerContext ComposerContext { get; private set; }
        protected ITaxViewModelFactory TaxViewModelFactory { get; private set; }
        protected ILineItemViewModelFactory LineItemViewModelFactory { get; private set; }
        protected IRewardViewModelFactory RewardViewModelFactory { get; private set; }
        protected ICartUrlProvider CartUrlProvider { get; private set; }

        public CartViewModelFactory(
            ILocalizationProvider localizationProvider,
            IViewModelMapper viewModelMapper,
            IFulfillmentMethodRepository fulfillmentMethodRepository,
            ICountryService countryService,
            IComposerContext composerContext,
            ITaxViewModelFactory taxViewModelFactory,
            ILineItemViewModelFactory lineItemViewModelFactory,
            IRewardViewModelFactory rewardViewModelFactory,
            ICartUrlProvider cartUrlProvider)
        {
            LocalizationProvider = localizationProvider ?? throw new ArgumentNullException(nameof(localizationProvider));
            ViewModelMapper = viewModelMapper ?? throw new ArgumentNullException(nameof(viewModelMapper));
            FulfillmentMethodRepository = fulfillmentMethodRepository ?? throw new ArgumentNullException(nameof(fulfillmentMethodRepository));
            CountryService = countryService ?? throw new ArgumentNullException(nameof(countryService));
            ComposerContext = composerContext ?? throw new ArgumentNullException(nameof(composerContext));
            TaxViewModelFactory = taxViewModelFactory ?? throw new ArgumentNullException(nameof(taxViewModelFactory));
            LineItemViewModelFactory = lineItemViewModelFactory ?? throw new ArgumentNullException(nameof(lineItemViewModelFactory));
            RewardViewModelFactory = rewardViewModelFactory ?? throw new ArgumentNullException(nameof(rewardViewModelFactory));
            CartUrlProvider = cartUrlProvider ?? throw new ArgumentNullException(nameof(cartUrlProvider));
        }

        public virtual CartViewModel CreateCartViewModel(CreateCartViewModelParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)), nameof(param)); }
            if (param.ProductImageInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.ProductImageInfo)), nameof(param)); }
            if (param.ProductImageInfo.ImageUrls == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.ProductImageInfo.ImageUrls)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.BaseUrl)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.BaseUrl)), nameof(param)); }

            var vm = ViewModelMapper.MapTo<CartViewModel>(param.Cart, param.CultureInfo);
            if (vm == null) { return null; }

            vm.OrderSummary = GetOrderSummaryViewModel(param.Cart, param.CultureInfo);
            MapShipmentsAndPayments(param.Cart, param.CultureInfo, param.ProductImageInfo, param.BaseUrl, param.PaymentMethodDisplayNames, vm, param.Cart.Payments);
            MapCustomer(param.Cart.Customer, param.CultureInfo, vm);
            vm.Coupons = GetCouponsViewModel(param.Cart, param.CultureInfo, param.IncludeInvalidCouponsMessages);
            vm.OrderSummary.AdditionalFeeSummaryList = GetAdditionalFeesSummary(vm.LineItemDetailViewModels, param.CultureInfo);

            SetDefaultCountryCode(vm);
            SetDefaultShippingAddressNames(vm);
            SetPostalCodeRegexPattern(vm);
            SetPhoneNumberRegexPattern(vm);

            var getUrlParam = new BaseUrlParameter
            {
                CultureInfo = ComposerContext.CultureInfo
            };

            SetHomepageUrl(vm, getUrlParam);
            SetEditCartUrl(vm, getUrlParam);
            SetCheckoutUrl(vm, getUrlParam);
            SetForgotPasswordPageUrl(vm, getUrlParam);

            // Reverse the items order in the Cart so the last added item will be the first in the list
            if (vm.LineItemDetailViewModels != null)
            {
                vm.LineItemDetailViewModels.Reverse();
            }

            vm.IsAuthenticated = ComposerContext.IsAuthenticated;

            return vm;
        }

        protected virtual void SetForgotPasswordPageUrl(CartViewModel cartViewModel, BaseUrlParameter getUrlParam)
        {
            cartViewModel.ForgotPasswordUrl = CartUrlProvider.GetForgotPasswordPageUrl(getUrlParam);
        }

        protected virtual void SetCheckoutUrl(CartViewModel cartViewModel, BaseUrlParameter getUrlParam)
        {
            if (cartViewModel.OrderSummary != null)
            {
                var checkoutUrl = CartUrlProvider.GetCheckoutPageUrl(getUrlParam);
                cartViewModel.OrderSummary.CheckoutUrlTarget = checkoutUrl;
            }
        }

        protected virtual void SetHomepageUrl(CartViewModel cartViewModel, BaseUrlParameter getUrlParam)
        {

            if (cartViewModel != null)
            {
                var homepageUrl = CartUrlProvider.GetHomepageUrl(getUrlParam);

                cartViewModel.HomepageUrl = homepageUrl;
            }
        }

        protected void SetEditCartUrl(CartViewModel cartViewModel, BaseUrlParameter getUrlParam)
        {
            if (cartViewModel.OrderSummary != null)
            {
                var cartUrl = CartUrlProvider.GetCartUrl(getUrlParam);
                cartViewModel.OrderSummary.EditCartUrlTarget = cartUrl;
            }
        }

        protected virtual void SetDefaultShippingAddressNames(CartViewModel vm)
        {
            if (vm.ShippingAddress != null && vm.Customer != null)
            {
                if (string.IsNullOrWhiteSpace(vm.ShippingAddress.FirstName) &&
                    string.IsNullOrWhiteSpace(vm.ShippingAddress.LastName))
                {
                    vm.ShippingAddress.FirstName = vm.Customer.FirstName;
                    vm.ShippingAddress.LastName = vm.Customer.LastName;
                }
            }
        }

        //TODO: Remove this once we support the notion of countries other than Canada and also have a country picker
        protected virtual void SetDefaultCountryCode(CartViewModel vm)
        {
            if (vm.ShippingAddress != null && string.IsNullOrWhiteSpace(vm.ShippingAddress.CountryCode))
            {
                vm.ShippingAddress.CountryCode = ComposerContext.CountryCode;
            }

            if (vm.Payment?.BillingAddress != null && string.IsNullOrWhiteSpace(vm.Payment.BillingAddress.CountryCode))
            {
                vm.Payment.BillingAddress.CountryCode = ComposerContext.CountryCode;
            }
        }

        protected virtual void SetPhoneNumberRegexPattern(CartViewModel vm)
        {
            if (vm.ShippingAddress != null)
            {
                vm.ShippingAddress.PhoneRegex = CountryService.RetrieveCountryAsync(CountryParam).Result.PhoneRegex;

                if (vm.Payment != null && vm.Payment.BillingAddress != null)
                {
                    vm.Payment.BillingAddress.PhoneRegex = vm.ShippingAddress.PhoneRegex;
                }
            }
        }

        protected virtual void SetPostalCodeRegexPattern(CartViewModel vm)
        {
            var regex = CountryService.RetrieveCountryAsync(CountryParam).Result.PostalCodeRegex;

            if (vm.ShippingAddress == null)
            {
                vm.ShippingAddress = new AddressViewModel();
            }

            vm.ShippingAddress.PostalCodeRegexPattern = regex;

            if (vm.Payment == null)
            {
                vm.Payment = new PaymentViewModel
                {
                    BillingAddress = new BillingAddressViewModel()
                };
            }

            vm.Payment.BillingAddress.PostalCodeRegexPattern = regex;
        }

        /// <summary>
        /// Gets the AdditionalFeeSummaryViewModel.
        /// </summary>
        /// <param name="lineItemDetailViewModels"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        public virtual List<AdditionalFeeSummaryViewModel> GetAdditionalFeesSummary(
            IEnumerable<LineItemDetailViewModel> lineItemDetailViewModels,
            CultureInfo cultureInfo)
        {
            if (lineItemDetailViewModels == null) { return new List<AdditionalFeeSummaryViewModel>(); }

            var dictionary = new Dictionary<(string, bool), decimal>();

            foreach (var el in lineItemDetailViewModels)
            {
                if (el.AdditionalFees == null) { continue; }

                foreach(var l in el.AdditionalFees)
                {
                    if (dictionary.ContainsKey((l.DisplayName, l.Taxable)))
                    {
                        dictionary[(l.DisplayName, l.Taxable)] += l.TotalAmount;
                    }
                    else
                    {
                        dictionary.Add((l.DisplayName, l.Taxable), l.TotalAmount);
                    }
                }
            }

            return dictionary.Select(x => new AdditionalFeeSummaryViewModel
            {
                GroupName = x.Key.Item1,
                Taxable = x.Key.Item2,
                TotalAmount = LocalizationProvider.FormatPrice(x.Value, cultureInfo),
            }).ToList();
        }

        protected virtual string GetShippingPrice(decimal cost, CultureInfo cultureInfo)
        {
            var price = cost == 0
                ? GetFreeShippingPriceLabel(cultureInfo)
                : LocalizationProvider.FormatPrice(cost, cultureInfo);
            return price;
        }

        public virtual IList<OrderShippingMethodViewModel> GetShippingsViewModel(
           Overture.ServiceModel.Orders.Cart cart, CultureInfo cultureInfo)
        {
            var activeShipments = cart.GetActiveShipments();

            var shipments = activeShipments.Any() ?
                activeShipments.Where(x => x.FulfillmentMethod != null) :
                cart.Shipments.Where(x => x.FulfillmentMethod != null); //cancelled orders

            if (!shipments.Any())
            {
                return new List<OrderShippingMethodViewModel>();
            }

            var formatTaxable = LocalizationProvider.GetLocalizedString(new GetLocalizedParam
            {
                Category = "ShoppingCart",
                Key = "L_ShippingBasedOn",
                CultureInfo = cultureInfo
            });

            var formatNonTaxable = LocalizationProvider.GetLocalizedString(new GetLocalizedParam
            {
                Category = "ShoppingCart",
                Key = "L_ShippingBasedOnNonTaxable",
                CultureInfo = cultureInfo
            });

            var shippings = shipments.GroupBy(x => x.IsShippingTaxable()).Select(shippingGroup => new OrderShippingMethodViewModel
            {
                Taxable = shippingGroup.Key,
                Cost = GetShippingPrice(Convert.ToDecimal(shippingGroup.Sum(x => x.FulfillmentMethod.Cost)), cultureInfo),
                DisplayName = shippingGroup.Key ?
                    string.Format(formatTaxable, shippingGroup.FirstOrDefault().Address?.PostalCode) :
                    string.Format(formatNonTaxable, shippingGroup.FirstOrDefault().Address?.PostalCode)
            });

            return shippings.ToList();
        }

        /// <summary>
        /// Gets an OrderSummaryViewModel from a Cart.
        /// </summary>
        /// <param name="cart"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        public virtual OrderSummaryViewModel GetOrderSummaryViewModel(
            Overture.ServiceModel.Orders.Cart cart,
            CultureInfo cultureInfo)
        {
            var orderSummary = ViewModelMapper.MapTo<OrderSummaryViewModel>(cart, cultureInfo);
            var activeShipments = cart.GetActiveShipments();
            orderSummary.Shippings = GetShippingsViewModel(cart, cultureInfo);
            orderSummary.Shipping = GetShippingFee(cart, cultureInfo);
            orderSummary.IsShippingTaxable = activeShipments.FirstOrDefault().IsShippingTaxable(); //used in the cart/checkout
            orderSummary.HasReward = cart.DiscountTotal.HasValue && cart.DiscountTotal.Value > 0;
            orderSummary.CheckoutRedirectAction = GetCheckoutRedirectAction(cart);

            List<Reward> rewards = new List<Reward>();
            List<LineItem> lineItems = new List<LineItem>();
            foreach (var el in activeShipments)
            {
                rewards.AddRange(el.Rewards);
                lineItems.AddRange(el.LineItems);
            }

            orderSummary.Rewards = RewardViewModelFactory.CreateViewModel(rewards, cultureInfo, RewardLevel.FulfillmentMethod, RewardLevel.Shipment).ToList();

            decimal sumAllLineItemsSavings =
                Math.Abs(lineItems.Sum(
                    l => decimal.Multiply(decimal.Subtract(l.CurrentPrice.GetValueOrDefault(0), l.DefaultPrice.GetValueOrDefault(0)), Convert.ToDecimal(l.Quantity))));

            decimal savingsTotal = decimal.Add(cart.DiscountTotal.GetValueOrDefault(0), sumAllLineItemsSavings);
            orderSummary.SavingsTotal = savingsTotal.Equals(0) ? string.Empty : LocalizationProvider.FormatPrice(savingsTotal, cultureInfo);

            return orderSummary;
        }

        protected virtual CheckoutRedirectActionViewModel GetCheckoutRedirectAction(Overture.ServiceModel.Orders.Cart cart)
        {
            var checkoutRedirectAction = new CheckoutRedirectActionViewModel();

            if (cart.PropertyBag != null && cart.PropertyBag.TryGetValue(CartConfiguration.CartPropertyBagLastCheckoutStep, out object lastCheckoutStep))
            {
                checkoutRedirectAction.LastCheckoutStep = (int)lastCheckoutStep;
            }
            var activeShipments = cart.GetActiveShipments();
            //If there is no lineitem in the cart the checkout step is 0 (edit cart page).
            if (!activeShipments.Any() || !activeShipments.SelectMany(x => x.LineItems).Any())
            {
                checkoutRedirectAction.LastCheckoutStep = 0;
                return checkoutRedirectAction;
            }

            //If there is lineitem in the cart lastCheckoutStep can't be 0 but 1 (step 1).
            if (checkoutRedirectAction.LastCheckoutStep == 0)
            {
                checkoutRedirectAction.LastCheckoutStep = 1;
            }

            return checkoutRedirectAction;
        }

        protected virtual void MapShipmentsAndPayments(
            Overture.ServiceModel.Orders.Cart cart,
            CultureInfo cultureInfo,
            ProductImageInfo imageInfo,
            string baseUrl,
            Dictionary<string, string> paymentMethodDisplayNames,
            CartViewModel cartVm,
            List<Payment> payments)
        {
            if (cart.Shipments == null) { return; }

            var shipment = cart.GetActiveShipments().FirstOrDefault();

            if (shipment == null) { return; }

            MapOneShipment(shipment, cultureInfo, imageInfo, baseUrl, cartVm, cart);
            cartVm.Payment = GetPaymentViewModel(payments, shipment, paymentMethodDisplayNames, cultureInfo);
        }

        /// <summary>
        /// Maps a single shipment.
        /// </summary>
        /// <param name="shipment">Shipment to map.</param>
        /// <param name="cultureInfo">Culture Info.</param>
        /// <param name="imageInfo">Information about images</param>
        /// <param name="baseUrl">The request base url</param>
        /// <param name="cartVm">VM in which to map the shipment to.</param>
        /// <param name="cart">Cart being mapped.</param>
        protected virtual void MapOneShipment(
            Shipment shipment,
            CultureInfo cultureInfo,
            ProductImageInfo imageInfo,
            string baseUrl,
            CartViewModel cartVm,
            Overture.ServiceModel.Orders.Cart cart)
        {
            cartVm.CurrentShipmentId = shipment.Id;
            cartVm.Rewards = RewardViewModelFactory.CreateViewModel(shipment.Rewards, cultureInfo, RewardLevel.FulfillmentMethod, RewardLevel.Shipment).ToList();
            cartVm.OrderSummary.Taxes = TaxViewModelFactory.CreateTaxViewModels(shipment.Taxes, cultureInfo).ToList();
            cartVm.ShippingAddress = GetAddressViewModel(shipment.Address, cultureInfo);

            cartVm.PickUpLocationId = shipment.PickUpLocationId;

            cartVm.LineItemDetailViewModels = LineItemViewModelFactory.CreateViewModel(new CreateListOfLineItemDetailViewModelParam
            {
                Cart = cart,
                LineItems = shipment.LineItems,
                CultureInfo = cultureInfo,
                ImageInfo = imageInfo,
                BaseUrl = baseUrl
            }).ToList();

            MapLineItemQuantifiers(cartVm);

            cartVm.OrderSummary.IsShippingEstimatedOrSelected = IsShippingEstimatedOrSelected(shipment);
            cartVm.ShippingMethod = GetShippingMethodViewModel(shipment.FulfillmentMethod, cultureInfo);

            #pragma warning disable 618
            MapShipmentAdditionalFees(shipment, cartVm.OrderSummary, cultureInfo);
            #pragma warning restore 618
        }

        protected virtual void MapCustomer(CustomerSummary customer, CultureInfo cultureInfo, CartViewModel cartVm)
        {
            if (customer == null) { return; }

            var customerViewModel = ViewModelMapper.MapTo<CustomerSummaryViewModel>(customer, cultureInfo);
            cartVm.Customer = customerViewModel;
        }

        /// <summary>
        /// Map the shipment additionnal fees to the orderSummaryViewModel.
        /// </summary>
        /// <param name="shipment"></param>
        /// <param name="viewModel"></param>
        /// <param name="cultureInfo"></param>
        [Obsolete("Use MapShipmentsAdditionalFees instead")]
        public virtual void MapShipmentAdditionalFees(Shipment shipment, OrderSummaryViewModel viewModel, CultureInfo cultureInfo)
        {
            var shipmentAdditionalFees = GetShipmentAdditionalFees(shipment.AdditionalFees, cultureInfo).ToList();
            viewModel.ShipmentAdditionalFeeAmount = shipment.AdditionalFeeAmount.ToString();
            viewModel.ShipmentAdditionalFeeSummaryList = GetShipmentAdditionalFeeSummary(shipmentAdditionalFees, cultureInfo);
        }

        /// <summary>
        /// Map the shipment additionnal fees to the orderSummaryViewModel considering all shipments
        /// </summary>
        /// <param name="shipments"></param>
        /// <param name="viewModel"></param>
        /// <param name="cultureInfo"></param>
        public virtual void MapShipmentsAdditionalFees(IEnumerable<Shipment> shipments, OrderSummaryViewModel viewModel, CultureInfo cultureInfo)
        {
            var allShipmentAdditionalFees = new List<ShipmentAdditionalFee>();
            decimal totalFeeAmount = 0;
            foreach(var el in shipments)
            {
                allShipmentAdditionalFees.AddRange(el.AdditionalFees);
                totalFeeAmount += el.AdditionalFeeAmount ?? 0;
            }
            var shipmentAdditionalFees = GetShipmentAdditionalFees(allShipmentAdditionalFees, cultureInfo).ToList();
            viewModel.ShipmentAdditionalFeeAmount = totalFeeAmount.ToString();
            viewModel.ShipmentAdditionalFeeSummaryList = GetShipmentAdditionalFeeSummary(shipmentAdditionalFees, cultureInfo);
        }

        protected virtual IEnumerable<ShipmentAdditionalFeeViewModel> GetShipmentAdditionalFees(IEnumerable<ShipmentAdditionalFee> additionalFees, CultureInfo cultureInfo)
        {
            return additionalFees.Select(shipmentAdditionalFee => ViewModelMapper.MapTo<ShipmentAdditionalFeeViewModel>(shipmentAdditionalFee, cultureInfo));
        }

        public virtual List<AdditionalFeeSummaryViewModel> GetShipmentAdditionalFeeSummary(
            IEnumerable<ShipmentAdditionalFeeViewModel> shipmentAdditionalFeeViewModels, 
            CultureInfo cultureInfo)
        {
            if (shipmentAdditionalFeeViewModels == null) { return new List<AdditionalFeeSummaryViewModel>(); }

            var dictionary = new Dictionary<(string, bool), decimal>();

            foreach(var el in shipmentAdditionalFeeViewModels)
            {
                if (dictionary.ContainsKey((el.DisplayName, el.Taxable)))
                {
                    dictionary[(el.DisplayName, el.Taxable)] += el.Amount;
                }
                else
                {
                    dictionary.Add((el.DisplayName, el.Taxable), el.Amount);
                }
            }

            return dictionary.Select(x => new AdditionalFeeSummaryViewModel
            {
                GroupName = x.Key.Item1,
                Taxable = x.Key.Item2,
                TotalAmount = LocalizationProvider.FormatPrice(x.Value, cultureInfo),
            }).ToList();
        }

        protected virtual bool IsShippingEstimatedOrSelected(Shipment shipment)
        {
            return shipment?.Address == null ? false : !string.IsNullOrWhiteSpace(shipment.Address.PostalCode);
        }

        /// <summary>
        /// Gets a ShippingMethodViewModel from an Overture FulfillmentMethod object.
        /// </summary>
        /// <param name="fulfillmentMethod"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        public virtual ShippingMethodViewModel GetShippingMethodViewModel(FulfillmentMethod fulfillmentMethod, CultureInfo cultureInfo)
        {
            if (fulfillmentMethod == null) { return null; }

            var shippingMethodViewModel = ViewModelMapper.MapTo<ShippingMethodViewModel>(fulfillmentMethod, cultureInfo);

            if(string.IsNullOrWhiteSpace(shippingMethodViewModel.DisplayName))
            {
                shippingMethodViewModel.DisplayName = shippingMethodViewModel.Name;
            }

            if (fulfillmentMethod.ExpectedDeliveryDate.HasValue) { 
                var totalDays = (int)Math.Ceiling((fulfillmentMethod.ExpectedDeliveryDate.Value - DateTime.UtcNow).TotalDays);
                shippingMethodViewModel.ExpectedDaysBeforeDelivery = totalDays.ToString();
            }

            shippingMethodViewModel.IsShipToStoreType = fulfillmentMethod.FulfillmentMethodType == FulfillmentMethodType.ShipToStore;
            shippingMethodViewModel.FulfillmentMethodTypeString = fulfillmentMethod.FulfillmentMethodType.ToString();

            return shippingMethodViewModel;
        }

        public virtual ShippingMethodTypeViewModel GetShippingMethodTypeViewModel(FulfillmentMethodType fulfillmentMethodType, IList<ShippingMethodViewModel> shippingMethods, CultureInfo cultureInfo)
        {
            shippingMethods = FilterShippingMethods(shippingMethods, fulfillmentMethodType);
            SetDefaultShippingMethod(shippingMethods);

            var fulfillmentMethodTypeString = fulfillmentMethodType.ToString();
            var displayName = LocalizationProvider.GetLocalizedString(new GetLocalizedParam
            {
                Category = "CheckoutProcess",
                Key = $"T_{fulfillmentMethodTypeString}MethodType",
                CultureInfo = cultureInfo
            });

            return new ShippingMethodTypeViewModel
            {
                FulfillmentMethodType = fulfillmentMethodType,
                FulfillmentMethodTypeString = fulfillmentMethodTypeString,
                DisplayName = displayName,
                ShippingMethods = shippingMethods
            };
        }

        protected virtual void SetDefaultShippingMethod(IList<ShippingMethodViewModel> shippingMethods)
        {
            var defaultMethod = shippingMethods.OrderBy(method => method.CostDouble).FirstOrDefault();
            if (defaultMethod != null)
            {
                defaultMethod.IsSelected = true;
            }
        }

        protected virtual IList<ShippingMethodViewModel> FilterShippingMethods(IList<ShippingMethodViewModel> shippingMethods, FulfillmentMethodType fulfillmentMethodType)
        {
            switch (fulfillmentMethodType)
            {
                case FulfillmentMethodType.PickUp: return new List<ShippingMethodViewModel> { shippingMethods.FirstOrDefault() };
                default: return shippingMethods;
            }
        }

        /// <summary>
        /// Gets a PaymentMethodViewModel from an Overture PaymentMethod object.
        /// </summary>
        /// <param name="paymentMethod"></param>
        /// <param name="paymentMethodDisplayNames"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        public virtual IPaymentMethodViewModel GetPaymentMethodViewModel(
            PaymentMethod paymentMethod,
            Dictionary<string, string> paymentMethodDisplayNames,
            CultureInfo cultureInfo)
        {
            if (paymentMethod == null) { return null; }

            paymentMethodDisplayNames.TryGetValue(paymentMethod.Type.ToString(), out string paymentMethodDisplayName);

            if (paymentMethodDisplayName == null) { return null; }

            IPaymentMethodViewModel paymentMethodViewModel;
            switch (paymentMethod.Type)
            {
                case PaymentMethodType.SavedCreditCard:
                    paymentMethodViewModel = MapSavedCreditCard(paymentMethod, cultureInfo);
                    break;

                default:
                    var vm = ViewModelMapper.MapTo<PaymentMethodViewModel>(paymentMethod, cultureInfo);
                    vm.DisplayName = paymentMethodDisplayName;
                    paymentMethodViewModel = vm;
                    break;
            }

            paymentMethodViewModel.PaymentType = paymentMethod.Type.ToString();

            return paymentMethodViewModel;
        }

        public virtual SavedCreditCardPaymentMethodViewModel MapSavedCreditCard(PaymentMethod payment, CultureInfo cultureInfo)
        {            
            var savedCreditCard = ViewModelMapper.MapTo<SavedCreditCardPaymentMethodViewModel>(payment, cultureInfo);

            if (!string.IsNullOrWhiteSpace(savedCreditCard.ExpiryDate))
            {
                var expirationDate = ParseCreditCardExpiryDate(savedCreditCard.ExpiryDate);
                expirationDate = expirationDate.AddDays(DateTime.DaysInMonth(expirationDate.Year, expirationDate.Month) - 1);
                savedCreditCard.IsExpired = expirationDate < DateTime.UtcNow;
            }

            return savedCreditCard;
        }

        protected virtual DateTime ParseCreditCardExpiryDate(string expiryDate)
        {
            var formats = new[]
            {
                "MMyy",
                "MM/yy",
                "MM-yy",
            };

            return DateTime.ParseExact(expiryDate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
        }

        /// <summary>
        /// Map the address of the client
        /// </summary>
        /// <param name="address"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        public virtual AddressViewModel GetAddressViewModel(Address address, CultureInfo cultureInfo)
        {
            if (address == null) { return new AddressViewModel(); }

            var addressViewModel = ViewModelMapper.MapTo<AddressViewModel>(address, cultureInfo);

            if (address.RegionCode != null)
            {
                var regionName = CountryService.RetrieveRegionDisplayNameAsync(new RetrieveRegionDisplayNameParam
                {
                    CultureInfo = cultureInfo,
                    IsoCode = ComposerContext.CountryCode,
                    RegionCode = address.RegionCode
                }).Result;
                addressViewModel.RegionName = regionName;
            }

            if (address.PhoneNumber != null)
            {
                addressViewModel.PhoneNumber = address.PhoneNumber;
                addressViewModel.PhoneNumberFormated = LocalizationProvider.FormatPhoneNumber(address.PhoneNumber, cultureInfo);
            }

            return addressViewModel;
        }

        protected virtual void MapLineItemQuantifiers(CartViewModel cartViewModel)
        {
            cartViewModel.IsCartEmpty = true;

            if (cartViewModel.LineItemDetailViewModels == null) { return; }

            foreach (var lineItemDetailViewModel in cartViewModel.LineItemDetailViewModels)
            {
                cartViewModel.IsCartEmpty = false;
                cartViewModel.LineItemCount += 1;
                cartViewModel.TotalQuantity += (int)lineItemDetailViewModel.Quantity;
                cartViewModel.InvalidLineItemCount += lineItemDetailViewModel.IsValid.GetValueOrDefault(true) ? 0 : 1;
            }
        }

        protected virtual string GetShippingFee(Overture.ServiceModel.Orders.Cart cart, CultureInfo cultureInfo)
        {
            if (!cart.GetActiveShipments().Any())
            {
                return GetFreeShippingPriceLabel(cultureInfo);
            }

            if (cart.FulfillmentCost.HasValue)
            {
                return GetShippingPrice(cart.FulfillmentCost.Value, cultureInfo);
            }

            var shippingMethodParam = new GetShippingMethodsParam
            {
                CartName = cart.Name,
                CultureInfo = cultureInfo,
                CustomerId = cart.CustomerId,
                Scope = cart.ScopeId
            };

            //TODO: Remove the repository and pass the list of fulfillmentMethods in params
            var fulfillmentMethods = FulfillmentMethodRepository.GetCalculatedFulfillmentMethods(shippingMethodParam).Result;

            if (fulfillmentMethods == null || !fulfillmentMethods.Any())
            {
                return GetFreeShippingPriceLabel(cultureInfo);
            }

            var cheapestShippingCost = fulfillmentMethods.Min(s => s.Cost);

            var price = GetShippingPrice((decimal)cheapestShippingCost, cultureInfo);

            return price;
        }

        protected virtual string GetFreeShippingPriceLabel(CultureInfo cultureInfo)
        {
            return LocalizationProvider.GetLocalizedString(new GetLocalizedParam
            {
                Category = "ShoppingCart",
                Key = "L_Free",
                CultureInfo = cultureInfo
            });
        }

        public virtual CouponsViewModel GetCouponsViewModel(
            Overture.ServiceModel.Orders.Cart cart,
            CultureInfo cultureInfo,
            bool includeMessages)
        {
            if (cart?.Coupons?.FirstOrDefault() == null) { return new CouponsViewModel(); }

            var couponsVm = new CouponsViewModel
            {
                ApplicableCoupons = new List<CouponViewModel>(),
                Messages = new List<CartMessageViewModel>()
            };

            var allRewards = GetAllRewards(cart.GetActiveShipments().ToList());

            foreach (var coupon in cart.Coupons)
            {
                if (IsCouponValid(coupon))
                {
                    var reward = allRewards.Find(r => r.PromotionId == coupon.PromotionId);
                    var couponVm = MapCoupon(coupon, reward, cultureInfo);
                    couponsVm.ApplicableCoupons.Add(couponVm);
                }
                else if (includeMessages)
                {
                    var message = GetInvalidCouponMessage(coupon, cultureInfo);
                    couponsVm.Messages.Add(message);
                }
            }
            return couponsVm;
        }
        /// <summary>
        /// Get all rewards applied on line items and shipment into a single list
        /// </summary>
        /// <param name="orderShipment"></param>
        /// <returns></returns>
        protected List<Reward> GetAllRewards(List<Shipment> orderShipment)
        {
            if (orderShipment.Any())
            {
                List<Reward> rewards = new List<Reward>();

                foreach(var el in orderShipment)
                {
                    rewards.AddRange(el.Rewards);
                    if (el.LineItems == null)
                    {
                        continue;
                    }
                    foreach(var l in el.LineItems)
                    {
                        rewards.AddRange(l.Rewards);
                    }
                }
                return rewards;
            }
            return new List<Reward>();
        }

        protected virtual bool IsCouponValid(Coupon coupon)
        {
            return coupon.CouponState == CouponState.Ok;
        }

        protected virtual CouponViewModel MapCoupon(Coupon coupon, Reward reward, CultureInfo cultureInfo)
        {
            var vm = ViewModelMapper.MapTo<CouponViewModel>(coupon, cultureInfo);

            if (reward != null)
            {
                vm.Amount = reward.Amount;
                vm.PromotionName = reward.PromotionName;
            }
            return vm;
        }

        protected virtual CartMessageViewModel GetInvalidCouponMessage(Coupon coupon, CultureInfo cultureInfo)
        {
            var localizedMessageTemplate = LocalizationProvider.GetLocalizedString(new GetLocalizedParam
            {
                Category = "ShoppingCart",
                Key = GetLocalizedKey(coupon.CouponState),
                CultureInfo = cultureInfo
            });

            var localizedMessage = string.Format(localizedMessageTemplate, coupon.CouponCode);

            var message = new CartMessageViewModel
            {
                Message = localizedMessage,
                Level = CartMessageLevels.Error
            };

            return message;
        }

        private static string GetLocalizedKey(CouponState couponState)
        {
            return couponState == CouponState.ValidCouponCannotApply
                ? "F_PromoCodeValidCouponCannotApply"
                : "F_PromoCodeInvalid";
        }

        /// <summary>
        /// Returns the fist payment non voided.
        /// </summary>
        protected virtual PaymentViewModel GetPaymentViewModel(
            List<Payment> payments,
            Shipment shipping,
            Dictionary<string, string> paymentMethodDisplayNames,
            CultureInfo cultureInfo)
        {
            var payment = payments?.Find(x => !x.IsVoided());

            if (payment is null) { return BuildEmptyPaymentViewModel(); }

            IPaymentMethodViewModel paymentMethodViewModel = null;
            if (payment.PaymentMethod != null)
            {
                paymentMethodViewModel = GetPaymentMethodViewModel(payment.PaymentMethod, paymentMethodDisplayNames, cultureInfo);
            }

            var billingAddressViewModel = GetBillingAddressViewModel(payment.BillingAddress, shipping, cultureInfo);

            return new PaymentViewModel
            {
                Id = payment.Id,
                IsLocked = payment.PaymentStatus != PaymentStatus.New && payment.PaymentStatus != PaymentStatus.PendingVerification,
                PaymentMethod = paymentMethodViewModel,
                PaymentStatus = payment.PaymentStatus,
                BillingAddress = billingAddressViewModel
            };
        }

        protected static PaymentViewModel BuildEmptyPaymentViewModel()
        {
            return new PaymentViewModel
            {
                IsLocked = false,
                PaymentStatus = PaymentStatus.New,
                BillingAddress = new BillingAddressViewModel { UseShippingAddress = true }
            };
        }

        protected virtual BillingAddressViewModel GetBillingAddressViewModel(Address billingAddress, Shipment shipping, CultureInfo cultureInfo)
        {
            if (billingAddress == null)
            {
                var useShippingAddress = shipping.FulfillmentMethod == null ? true :
                    shipping.FulfillmentMethod.FulfillmentMethodType == FulfillmentMethodType.Shipping;
                return new BillingAddressViewModel { UseShippingAddress = useShippingAddress };
            }

            var shippingAddress = shipping.Address;
            var billingAddressViewModel = MapBillingAddressViewModel(billingAddress, cultureInfo);
            billingAddressViewModel.UseShippingAddress = shippingAddress != null && AreAddressEqual(shippingAddress, billingAddress);

            return billingAddressViewModel;
        }

        protected virtual BillingAddressViewModel MapBillingAddressViewModel(Address address, CultureInfo cultureInfo)
        {
            if (address == null) { return null; }

            var addressViewModel = ViewModelMapper.MapTo<BillingAddressViewModel>(address, cultureInfo);

            if (!string.IsNullOrWhiteSpace(address.RegionCode))
            {
                var regionName = CountryService.RetrieveRegionDisplayNameAsync(new RetrieveRegionDisplayNameParam
                {
                    CultureInfo = cultureInfo,
                    IsoCode = ComposerContext.CountryCode,
                    RegionCode = address.RegionCode
                }).Result;

                addressViewModel.RegionName = regionName;
            }

            return addressViewModel;
        }

        protected virtual bool AreAddressEqual(Address first, Address second)
        {
            return first.FirstName == second.FirstName
                   && first.LastName == second.LastName
                   && first.Line1 == second.Line1
                   && first.Line2 == second.Line2
                   && first.City == second.City
                   && first.RegionCode == second.RegionCode
                   && first.CountryCode == second.CountryCode
                   && first.PostalCode == second.PostalCode
                   && first.PhoneNumber == second.PhoneNumber;
        }
    }
}
