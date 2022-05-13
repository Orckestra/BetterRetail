using System.Collections.Generic;
using System.Globalization;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.ViewModels;
using Orckestra.Overture.ServiceModel;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Cart.Factory
{
    public interface ICartViewModelFactory
    {
        /// <summary>
        /// Creates a <see cref="CartViewModel" /> based on a <see cref="Cart"/> object.
        /// </summary>
        /// <param name="param">Parameters used to create the ViewModel. May not be null.</param>
        /// <returns></returns>
        CartViewModel CreateCartViewModel(CreateCartViewModelParam param);

        /// <summary>
        /// Gets a ShippingMethodViewModel from an Overture FulfillmentMethod object.
        /// </summary>
        /// <param name="fulfillmentMethod"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        ShippingMethodViewModel GetShippingMethodViewModel(FulfillmentMethod fulfillmentMethod, CultureInfo cultureInfo);

        /// <summary>
        /// Gets a ShippingMethodTypeViewModel from an shippingMethods list.
        /// </summary>
        /// <param name="fulfillmentMethodType"></param>
        /// <param name="shippingMethods"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        ShippingMethodTypeViewModel GetShippingMethodTypeViewModel(FulfillmentMethodType fulfillmentMethodType, IList<ShippingMethodViewModel> shippingMethods, CultureInfo cultureInfo);

        /// <summary>
        /// Gets a PaymentMethodViewModel from an Overture PaymentMethod object.
        /// </summary>
        /// <param name="paymentMethod"></param>
        /// <param name="paymentMethodDisplayNames"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        IPaymentMethodViewModel GetPaymentMethodViewModel(PaymentMethod paymentMethod, Dictionary<string, string> paymentMethodDisplayNames, CultureInfo cultureInfo);

        /// <summary>
        /// Gets an OrderSummaryViewModel from a Cart.
        /// </summary>
        /// <param name="cart"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        OrderSummaryViewModel GetOrderSummaryViewModel(Overture.ServiceModel.Orders.Cart cart, CultureInfo cultureInfo);

        /// <summary>
        /// Map the address of the client
        /// </summary>
        /// <param name="address"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        AddressViewModel GetAddressViewModel(Address address, CultureInfo cultureInfo);

        /// <summary>
        /// Map the coupons of the client
        /// </summary>
        /// <param name="cart"></param>
        /// <param name="cultureInfo"></param>
        /// <param name="includeMessages"></param>  Gets or sets a message to indicate if coupons are usable and the reason when it is not.
        /// <returns></returns>
        CouponsViewModel GetCouponsViewModel(Overture.ServiceModel.Orders.Cart cart, CultureInfo cultureInfo, bool includeMessages);

        /// <summary>
        /// Map the shipment additionnal fees to the orderSummaryViewModel.
        /// </summary>
        /// <param name="shipment"></param>
        /// <param name="viewModel"></param>
        /// <param name="cultureInfo"></param>
        void MapShipmentAdditionalFees(Shipment shipment, OrderSummaryViewModel viewModel,CultureInfo cultureInfo);

        /// <summary>
        /// Map the shipment additionnal fees to the orderSummaryViewModel considering multiples shipments
        /// </summary>
        /// <param name="shipments"></param>
        /// <param name="viewModel"></param>
        /// <param name="cultureInfo"></param>
        void MapShipmentsAdditionalFees(IEnumerable<Shipment> shipments, OrderSummaryViewModel viewModel,
            CultureInfo cultureInfo);

        /// <summary>
        /// Gets the AdditionalFeeSummaryViewModel.
        /// </summary>
        /// <param name="lineItemDetailViewModels"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        List<AdditionalFeeSummaryViewModel> GetAdditionalFeesSummary(IEnumerable<LineItemDetailViewModel> lineItemDetailViewModels, CultureInfo cultureInfo);

        SavedCreditCardPaymentMethodViewModel MapSavedCreditCard(PaymentMethod payment, CultureInfo cultureInfo);
    }
}
