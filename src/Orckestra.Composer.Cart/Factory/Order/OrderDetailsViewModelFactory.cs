using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using Orckestra.Composer.Cart.Extensions;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Parameters.Order;
using Orckestra.Composer.Cart.ViewModels;
using Orckestra.Composer.Cart.ViewModels.Order;
using Orckestra.Composer.Country;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Localization;
using Orckestra.Composer.Utils;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel.Orders;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;

namespace Orckestra.Composer.Cart.Factory.Order
{
    public class OrderDetailsViewModelFactory : IOrderDetailsViewModelFactory
    {
        private readonly string _orderStatus = "OrderStatus";
        private readonly string _shipmentStatus = "ShipmentStatus";
        private readonly string _orderStatusCanceled = "Canceled";

        protected virtual ILocalizationProvider LocalizationProvider { get; private set; }
        protected virtual IViewModelMapper ViewModelMapper { get; private set; }
        protected virtual ICountryService CountryService { get; private set; }
        protected virtual IProductUrlProvider ProductUrlProvider { get; private set; }
        protected virtual ICartViewModelFactory CartViewModelFactory { get; private set; }
        protected virtual IPaymentProviderFactory PaymentProviderFactory { get; private set; }
        protected virtual IShippingTrackingProviderFactory ShippingTrackingProviderFactory { get; private set; }
        protected virtual ITaxViewModelFactory TaxViewModelFactory { get; private set; }
        protected virtual ILineItemViewModelFactory LineItemViewModelFactory { get; private set; }
        protected virtual IRewardViewModelFactory RewardViewModelFactory { get; private set; }

        public OrderDetailsViewModelFactory(
            ILocalizationProvider localizationProvider,
            IViewModelMapper viewModelMapper,
            ICountryService countryService,
            IProductUrlProvider productUrlProvider,
            ICartViewModelFactory cartViewModelFactory,
            IPaymentProviderFactory paymentProviderFactory,
            IShippingTrackingProviderFactory shippingTrackingProviderFactory,
            ITaxViewModelFactory taxViewModelFactory,
            ILineItemViewModelFactory lineItemViewModelFactory,
            IRewardViewModelFactory rewardViewModelFactory)
        {
            LocalizationProvider = localizationProvider ?? throw new ArgumentNullException(nameof(localizationProvider));
            ViewModelMapper = viewModelMapper ?? throw new ArgumentNullException(nameof(viewModelMapper));
            CountryService = countryService ?? throw new ArgumentNullException(nameof(countryService));
            ProductUrlProvider = productUrlProvider ?? throw new ArgumentNullException(nameof(productUrlProvider));
            CartViewModelFactory = cartViewModelFactory ?? throw new ArgumentNullException(nameof(cartViewModelFactory));
            PaymentProviderFactory = paymentProviderFactory ?? throw new ArgumentNullException(nameof(paymentProviderFactory));
            ShippingTrackingProviderFactory = shippingTrackingProviderFactory ?? throw new ArgumentNullException(nameof(shippingTrackingProviderFactory));
            TaxViewModelFactory = taxViewModelFactory ?? throw new ArgumentNullException(nameof(taxViewModelFactory));
            LineItemViewModelFactory = lineItemViewModelFactory ?? throw new ArgumentNullException(nameof(lineItemViewModelFactory));
            RewardViewModelFactory = rewardViewModelFactory ?? throw new ArgumentNullException(nameof(rewardViewModelFactory));
        }

        /// <summary>
        /// Creates an OrderDetailViewModel, containing all the informations about the order, the shipments and lineitems.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual OrderDetailViewModel CreateViewModel(CreateOrderDetailViewModelParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)), nameof(param)); }
            if (param.Order == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.Order)), nameof(param)); }
            if (param.OrderStatuses == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.OrderStatuses)), nameof(param)); }
            if (param.ShipmentStatuses == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.ShipmentStatuses)), nameof(param)); }
            if (param.OrderChanges == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.OrderChanges)), nameof(param)); }
            if (param.ProductImageInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.ProductImageInfo)), nameof(param)); }
            if (param.ProductImageInfo.ImageUrls == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.ProductImageInfo.ImageUrls)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.BaseUrl)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.BaseUrl)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.CountryCode)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.CountryCode)), nameof(param)); }

            var viewModel = new OrderDetailViewModel();
            var shipments = GetActiveShipments(param.Order);

            #pragma warning disable 618
            viewModel.OrderInfos = GetOrderInfosViewModel(param);
            viewModel.History = GetOrderChangesViewModel(param.OrderChanges, param.CultureInfo, _orderStatus);
            viewModel.BillingAddress = GetBillingAddressViewModel(param);
            viewModel.ShippingAddress = GetShippingAddressViewModel(param);
            viewModel.Shipments = GetShipmentViewModels(param);
            viewModel.ShippingMethod = GetShippingMethodViewModel(param);
            viewModel.Payments = GetPaymentViewModels(param);
            viewModel.OrderSummary = CartViewModelFactory.GetOrderSummaryViewModel(param.Order.Cart, param.CultureInfo);
            viewModel.OrderSummary.Taxes = TaxViewModelFactory.CreateTaxViewModels(shipments.SelectMany(s => s.Taxes).ToList(), param.CultureInfo).ToList();
            MapAdditionalFees(viewModel, param);
            #pragma warning restore 618

            // Reverse the items order in the Cart so the last added item will be the first in the list
            if (viewModel.Shipments != null && viewModel.Shipments.Any())
            {
                viewModel.Shipments.ForEach(x => x.LineItems.Reverse());
            }

            return viewModel;
        }

        public virtual LightOrderDetailViewModel CreateLightViewModel(CreateOrderDetailViewModelParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(nameof(param.CultureInfo)); }
            if (param.Order == null) { throw new ArgumentException(nameof(param.Order)); }
            if (param.OrderStatuses == null) { throw new ArgumentException(nameof(param.OrderStatuses)); }
            if (param.ProductImageInfo == null) { throw new ArgumentException(nameof(param.ProductImageInfo)); }

            var viewModel = new LightOrderDetailViewModel();
            viewModel.OrderInfos = GetOrderInfosViewModel(param);
            viewModel.Shipments = GetShipmentViewModels(param);

            var orderDetailUrl = UrlFormatter.AppendQueryString(param.OrderDetailBaseUrl, new NameValueCollection
                {
                    {"id", param.Order.OrderNumber}
                });

            viewModel.Url = orderDetailUrl;

            return viewModel;
        }

        protected virtual OrderDetailInfoViewModel GetOrderInfosViewModel(CreateOrderDetailViewModelParam param)
        {
            if (param.Order.Cart.Total == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.Order.Cart.Total)), nameof(param)); }

            var orderInfos = ViewModelMapper.MapTo<OrderDetailInfoViewModel>(param.Order, param.CultureInfo);

            orderInfos.OrderStatus = param.OrderStatuses[param.Order.OrderStatus];
            orderInfos.OrderStatusRaw = param.Order.OrderStatus;
            orderInfos.BillingCurrency = param.Order.Cart.BillingCurrency;
            orderInfos.PricePaid = LocalizationProvider.FormatPrice((decimal)param.Order.Cart.Total, param.CultureInfo);

            return orderInfos;
        }

        protected virtual List<OrderChangeViewModel> GetOrderChangesViewModel(IEnumerable<OrderHistoryItem> orderChanges, CultureInfo cultureInfo, params string[] historyCategories)
        {
            var history = new List<OrderChangeViewModel>();
            var hashSet = new HashSet<string>(historyCategories, StringComparer.InvariantCultureIgnoreCase);

            foreach (var change in orderChanges)
            {
                if (hashSet.Contains(change.Category))
                {
                    var changeVm = MapOrderChangeViewModel(change, cultureInfo);
                    history.Add(changeVm);
                }
            }
            return history;
        }

        [Obsolete("This function does not support multiple shipments. Use GetAddressViewModel instead")]
        protected virtual AddressViewModel GetShippingAddressViewModel(CreateOrderDetailViewModelParam param)
        {
            Shipment shipment = GetActiveShipments(param.Order).FirstOrDefault();

            if (shipment == null) { return new AddressViewModel(); }

            // ReSharper disable once PossibleNullReferenceException (The address can be null we will create one)
            return CartViewModelFactory.GetAddressViewModel(shipment.Address, param.CultureInfo);
        }

        [Obsolete("This function does not support multiple shipments. Use GetAddressViewModel instead")]
        protected virtual AddressViewModel GetBillingAddressViewModel(CreateOrderDetailViewModelParam param)
        {
            var validPayment = param.Order.Cart.Payments.Where(x => !x.IsVoided()).FirstOrDefault();
            var payment = validPayment ?? param.Order.Cart.Payments.FirstOrDefault();
            return payment == null ? null : CartViewModelFactory.GetAddressViewModel(payment.BillingAddress, param.CultureInfo);
        }

        protected virtual List<OrderShipmentDetailViewModel> GetShipmentViewModels(CreateOrderDetailViewModelParam param)
        {
            var shipmentVms = new List<OrderShipmentDetailViewModel>();

            foreach (var shipment in GetActiveShipments(param.Order))
            {
                var shipmentVm = GetOrderShipmentDetailViewModel(shipment, param);
                shipmentVms.Add(shipmentVm);
            }

            return shipmentVms;
        }

        protected virtual OrderChangeViewModel MapOrderChangeViewModel(OrderHistoryItem orderHistoryItem, CultureInfo cultureInfo)
        {
            var orderChangeVm = ViewModelMapper.MapTo<OrderChangeViewModel>(orderHistoryItem, cultureInfo);

            return orderChangeVm;
        }

        protected virtual OrderShipmentDetailViewModel GetOrderShipmentDetailViewModel(Shipment shipment, CreateOrderDetailViewModelParam param)
        {
            var shipmentVm = new OrderShipmentDetailViewModel();

            var index = param.Order.Cart.Shipments.IndexOf(shipment);

            if (index >= 0)
            {
                shipmentVm.Index = (index + 1).ToString();
            }

            if (shipment.FulfillmentScheduledTimeBeginDate.HasValue)
            {
                shipmentVm.ScheduledShipDate = 
                    LocalizationHelper.LocalizedFormat("General", "ShortDateFormat", shipment.FulfillmentScheduledTimeBeginDate.Value, param.CultureInfo);
            }

            shipmentVm.LineItems = LineItemViewModelFactory.CreateViewModel(new CreateListOfLineItemDetailViewModelParam
            {
                Cart = param.Order.Cart,
                LineItems = shipment.LineItems,
                CultureInfo = param.CultureInfo,
                ImageInfo = param.ProductImageInfo,
                BaseUrl = param.BaseUrl
            }).ToList();

            if (param.ShipmentsNotes != null && param.ShipmentsNotes.ContainsKey(shipment.Id))
            {
                var notes = param.ShipmentsNotes[shipment.Id];
                shipmentVm.Comments = notes;
            }

            shipmentVm.Rewards = RewardViewModelFactory.CreateViewModel(shipment.Rewards, param.CultureInfo, RewardLevel.FulfillmentMethod, RewardLevel.Shipment).ToList();

            shipmentVm.ShippingAddress = CartViewModelFactory.GetAddressViewModel(shipment.Address, param.CultureInfo);

            var fulfillmentMethodName = shipment.FulfillmentMethod.Name;
            var shippingTrackingProvider = ShippingTrackingProviderFactory.ResolveProvider(fulfillmentMethodName);
            shipmentVm.TrackingInfo = shippingTrackingProvider.GetTrackingInfoViewModel(shipment, param.CultureInfo);
            shipmentVm.History = GetOrderChangesViewModel(param.OrderChanges, param.CultureInfo, _shipmentStatus);
            if (!string.IsNullOrWhiteSpace(shipment.Status))
            {
                shipmentVm.ShipmentStatusName = shipment.Status;
                if (param.ShipmentStatuses.TryGetValue(shipment.Status, out string shipmentStatusLookup))
                {
                    shipmentVm.ShipmentStatus = shipmentStatusLookup;
                }

                string shipmentStatusDate = null;
                foreach(var el in shipmentVm.History)
                {
                    if (!el.NewValue.Equals(shipment.Status)) { continue; }

                    if (shipmentStatusDate == null)
                    {
                        shipmentStatusDate = el.Date;
                    }
                    else
                    {
                        int cIndex = string.Compare(shipmentStatusDate, el.Date);
                        if (cIndex < 0)
                        {
                            shipmentStatusDate = el.Date;
                        }
                    }
                }

                if (shipmentStatusDate != null)
                {
                    shipmentVm.ShipmentStatusDate = shipmentStatusDate;
                }
            }
            else
            {
                shipmentVm.ShipmentStatusName = string.Empty;
                shipmentVm.ShipmentStatus = string.Empty;
                shipmentVm.ShipmentStatusDate = string.Empty;
            }

            shipmentVm.ShippingMethod = GetShippingMethodViewModel(shipment, param);
            return shipmentVm;
        }

        [Obsolete("This function does not support multiple shipments. Use the overloaded GetShippingMethodViewModel with shipment instead")]
        protected virtual OrderShippingMethodViewModel GetShippingMethodViewModel(CreateOrderDetailViewModelParam param)
        {
            if (param.Order.Cart.Shipments == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.Order.Cart.Shipments)), nameof(param)); }

            var shipment = GetActiveShipments(param.Order).FirstOrDefault();
            var shippingMethodVm = GetShippingMethodViewModel(shipment, param);

            return shippingMethodVm;
        }

        protected virtual OrderShippingMethodViewModel GetShippingMethodViewModel(Shipment shipment, CreateOrderDetailViewModelParam param)
        {
            if (param.Order.Cart.Shipments == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.Order.Cart.Shipments)), nameof(param)); }

            if (shipment == null) { return new OrderShippingMethodViewModel(); }

            var shippingMethodVm = ViewModelMapper.MapTo<OrderShippingMethodViewModel>(shipment.FulfillmentMethod, param.CultureInfo);

            if (param.Order.Cart.FulfillmentCost == 0)
            {
                var freeLabel = LocalizationProvider.GetLocalizedString(new GetLocalizedParam
                {
                    Category = "ShoppingCart",
                    Key = "L_Free",
                    CultureInfo = param.CultureInfo
                });

                shippingMethodVm.Cost = freeLabel;
            }

            shippingMethodVm.Taxable = shipment.IsShippingTaxable();

            return shippingMethodVm;
        }

        protected virtual List<OrderSummaryPaymentViewModel> GetPaymentViewModels(CreateOrderDetailViewModelParam param)
        {
            var paymentVMs = new List<OrderSummaryPaymentViewModel>();
            var validPayments = param.Order.Cart.Payments.Where(x => !x.IsVoided()).ToList();
            var payments = validPayments.Any() ? validPayments : param.Order.Cart.Payments;

            foreach (var payment in payments)
            {
                OrderSummaryPaymentViewModel model;
                try
                {
                    var paymentProvider = PaymentProviderFactory.ResolveProvider(payment.PaymentMethod.PaymentProviderName);
                    model = paymentProvider.BuildOrderSummaryPaymentViewModel(payment, param.CultureInfo);
                }
                catch (ArgumentException)
                {
                    model = new OrderSummaryPaymentViewModel();
                }

                paymentVMs.Add(model);
            }

            return paymentVMs;
        }

        protected virtual void MapAdditionalFees(OrderDetailViewModel viewModel, CreateOrderDetailViewModelParam param)
        {
            CartViewModelFactory.MapShipmentsAdditionalFees(GetActiveShipments(param.Order), viewModel.OrderSummary, param.CultureInfo);

            var allLineItems = viewModel.Shipments.SelectMany(x => x.LineItems).ToList();
            viewModel.OrderSummary.AdditionalFeeSummaryList = CartViewModelFactory.GetAdditionalFeesSummary(allLineItems, param.CultureInfo);
        }

        protected virtual IEnumerable<Shipment> GetActiveShipments(Overture.ServiceModel.Orders.Order order)
        {
            return order.OrderStatus.Equals(_orderStatusCanceled) ? order.Cart.Shipments : order.Cart.GetActiveShipments();
        }
    }
}