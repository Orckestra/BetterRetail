using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Orckestra.Composer.Cart.Extensions;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Repositories;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Overture.ServiceModel.Orders;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;

namespace Orckestra.Composer.Cart.Services
{
    public class FixCartService : IFixCartService
    {
        protected virtual ICartRepository CartRepository { get; private set; }
        protected virtual IInventoryLocationProvider InventoryLocationProvider { get; private set; }

        public FixCartService(ICartRepository cartRepository, IInventoryLocationProvider inventoryLocationProvider)
        {
            CartRepository = cartRepository;
            InventoryLocationProvider = inventoryLocationProvider;
        }

        /// <summary>
        /// Fix cart missing informations like empty payments or fulfillment location.
        /// This method will be useless when overture will always return a valid cart.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual async Task<ProcessedCart> FixCartAsync(FixCartParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (param.Cart == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.Cart)), nameof(param)); }

            param.Cart = await AddPaymentIfRequired(param).ConfigureAwait(false);
            param.Cart = await SetFulfillmentLocationIfRequired(param);

            return param.Cart;
        }

        public virtual Task<ProcessedCart> AddPaymentIfRequired(FixCartParam param)
        {
            var cart = param.Cart;
            if (!HasAnyValidPayment(cart))
            {
                var task = CartRepository.AddPaymentAsync(new AddPaymentParam
                {
                    CartName = cart.Name,
                    CultureInfo = new CultureInfo(cart.CultureName),
                    CustomerId = cart.CustomerId,
                    Scope = cart.ScopeId
                });

                return task;
            }

            return Task.FromResult(cart);
        }

        //TODO: Remove the InventoryLocationProvider and pass the FulfillmentLocationId by parameter
        public virtual async Task<ProcessedCart> SetFulfillmentLocationIfRequired(FixCartParam param)
        {
            var cart = param.Cart;
            if (!HasValidFulfillmentLocation(cart))
            {
                var fulfillmentLocation =
                    await InventoryLocationProvider.GetFulfillmentLocationAsync(new GetFulfillmentLocationParam
                    {
                        Scope = cart.ScopeId
                    }).ConfigureAwait(false);

                var shipment = cart.Shipments.FirstOrDefault() ?? new Shipment();

                cart = await CartRepository.UpdateShipmentAsync(new UpdateShipmentParam
                {
                    CartName = cart.Name,
                    CultureInfo = new CultureInfo(cart.CultureName),
                    FulfillmentLocationId = fulfillmentLocation.Id,
                    CustomerId = cart.CustomerId,
                    FulfillmentMethodName = shipment.FulfillmentMethod?.Name,
                    FulfillmentScheduleMode = shipment.FulfillmentScheduleMode,
                    FulfillmentScheduledTimeBeginDate = shipment.FulfillmentScheduledTimeBeginDate,
                    FulfillmentScheduledTimeEndDate = shipment.FulfillmentScheduledTimeEndDate,
                    PropertyBag = shipment.PropertyBag,
                    Id = shipment.Id,
                    ScopeId = cart.ScopeId,
                    ShippingAddress = shipment.Address,
                    ShippingProviderId = shipment.FulfillmentMethod == null ? Guid.Empty : shipment.FulfillmentMethod.ShippingProviderId
                }).ConfigureAwait(false);
            }

            return cart;
        }

        /// <summary>
        /// Determines if the cart contains a valid Payment instance.
        /// </summary>
        /// <param name="cart">Cart used to evaluate payments.</param>
        /// <returns>True if the first shipment contains at least one valid payment.</returns>
        protected virtual bool HasAnyValidPayment(ProcessedCart cart)
        {
            if (cart.Payments == null) { return false; }

            bool hasValidPayment = cart.Payments.Any(p => !p.IsVoided());
            return hasValidPayment;
        }

        /// <summary>
        /// Determines if the cart's first shipment has a valid fulfillment location.
        /// </summary>
        /// <param name="cart"></param>
        /// <returns></returns>
        protected virtual bool HasValidFulfillmentLocation(ProcessedCart cart)
        {
            if (cart.Shipments == null || !cart.Shipments.Any()) { return false; }

            var hasValidFulfillmentLocation = cart.Shipments.First().FulfillmentLocationId != Guid.Empty;
            return hasValidFulfillmentLocation;
        }
    }
}