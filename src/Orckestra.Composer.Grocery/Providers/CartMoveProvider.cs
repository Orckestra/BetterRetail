using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Orckestra.Composer.Cart;
using Orckestra.Composer.Cart.Extensions;
using Orckestra.Composer.Cart.Factory;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Repositories;
using Orckestra.Composer.Grocery.Parameters;
using Orckestra.Composer.Grocery.Repositories;
using Orckestra.Overture.ServiceModel;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Grocery.Providers
{
    public class CartMoveProvider : ICartMoveProvider
    {
        public CartMoveProvider(ICartRepository cartRepository, ITimeSlotRepository timeSlotRepository, IFulfillmentMethodRepository fulfillmentMethodRepository)
        {
            CartRepository = cartRepository ?? throw new ArgumentNullException(nameof(cartRepository));
            TimeSlotRepository = timeSlotRepository ?? throw new ArgumentNullException(nameof(timeSlotRepository));
            FulfillmentMethodRepository = fulfillmentMethodRepository ?? throw new ArgumentNullException(nameof(fulfillmentMethodRepository));
        }
        protected ICartRepository CartRepository { get; }
        public ITimeSlotRepository TimeSlotRepository { get; }
        protected IFulfillmentMethodRepository FulfillmentMethodRepository { get; }

        public async Task<ProcessedCart> MoveCart(MoveCartParam param)
        {
            if (param == null) throw new ArgumentNullException(nameof(param));

            if (param.ScopeFrom.Equals(param.ScopeTo, StringComparison.OrdinalIgnoreCase))
            {
                //To update only shipment with new store since cart in same scope
                var cart = await CartRepository.GetCartAsync(new GetCartParam
                {
                    CustomerId = param.CustomerId,
                    CartName = CartConfiguration.ShoppingCartName,
                    Scope = param.ScopeFrom,
                    CultureInfo = param.CultureInfo
                }).ConfigureAwait(false);

                var shipment = cart.Shipments?.FirstOrDefault();

                return await UpdateShipment(param, shipment.Id, shipment).ConfigureAwait(false);
            }

            var getCurrentCartTask = CartRepository.GetCartAsync(new GetCartParam
            {
                CustomerId = param.CustomerId,
                CartName = CartConfiguration.ShoppingCartName,
                Scope = param.ScopeFrom,
                CultureInfo = param.CultureInfo
            });

            var getNewCartTask = CartRepository.GetCartAsync(new GetCartParam
            {
                CustomerId = param.CustomerId,
                CartName = CartConfiguration.ShoppingCartName,
                Scope = param.ScopeTo,
                CultureInfo = param.CultureInfo
            });

            var cartsResult = await Task.WhenAll(getCurrentCartTask, getNewCartTask).ConfigureAwait(false);
            ProcessedCart currentCart = cartsResult[0], newCart = cartsResult[1];

            var currentLineItems = currentCart.GetLineItems();
            await AddLineItems(param, currentLineItems);

            if (!newCart.Payments.Any(p => !p.IsVoided()))
            {
                var oldPayment = currentCart.Payments?.Find(p => !p.IsVoided());
                await AddPayment(param, oldPayment?.BillingAddress);
            }

            

            var oldShipment = currentCart.Shipments?.FirstOrDefault();
            var newShipment = newCart.Shipments?.FirstOrDefault()
                ?? throw new InvalidOperationException("No shipment in a cart.");
            var processedCart = await UpdateShipment(param, newShipment.Id, oldShipment).ConfigureAwait(false);

            if (currentCart.Customer != null && !string.IsNullOrEmpty(currentCart.Customer.Email))
            {
                processedCart.Customer = currentCart.Customer;
                processedCart = await CartRepository.UpdateCartAsync(UpdateCartParamFactory.Build(processedCart)).ConfigureAwait(false);
            }

            await CartRepository.DeleteCartAsync(new DeleteCartParam
            {
                CartName = CartConfiguration.ShoppingCartName,
                CultureInfo = param.CultureInfo,
                CustomerId = param.CustomerId,
                Scope = param.ScopeFrom
            }).ConfigureAwait(false);

            return processedCart;
        }

        private async Task<ProcessedCart> UpdateShipment(MoveCartParam param, Guid newShipmentId, Shipment oldShipment)
        {
            var fulfillmentMethod = oldShipment?.FulfillmentMethod;
            var isFulFillmentMethodChanged = fulfillmentMethod?.FulfillmentMethodType != param.FulfillementMethodType;
            if (fulfillmentMethod == null || isFulFillmentMethodChanged)
            {
                var fulfillmentMethods = await FulfillmentMethodRepository.GetCalculatedFulfillmentMethods(new GetShippingMethodsParam
                {
                    CartName = CartConfiguration.ShoppingCartName,
                    CustomerId = param.CustomerId,
                    CultureInfo = param.CultureInfo,
                    Scope = param.ScopeTo
                }).ConfigureAwait(false);

                // Selecting fulfillment method selected by customer, if it is available
                fulfillmentMethod = fulfillmentMethods.FirstOrDefault(method => method.FulfillmentMethodType == param.FulfillementMethodType);

                // Selecting 'Pickup' fulfillment method, if it is available
                if (fulfillmentMethod == null)
                {
                    fulfillmentMethod = fulfillmentMethods.FirstOrDefault(method => method.FulfillmentMethodType == FulfillmentMethodType.PickUp);
                }
            }

            if (oldShipment!= null && Guid.TryParse(oldShipment.FulfillmentScheduleReservationNumber, out Guid reservationNumber))
            {
                await TimeSlotRepository.DeleteFulfillmentLocationTimeSlotReservationByIdAsync(new BaseFulfillmentLocationTimeSlotReservationParam()
                {
                    SlotReservationId = reservationNumber,
                    Scope = param.ScopeFrom,
                    FulfillmentLocationId = oldShipment.FulfillmentLocationId
                }).ConfigureAwait(false);
            }
            var updateShipmentParam = new UpdateShipmentParam
            {
                CartName = CartConfiguration.ShoppingCartName,
                CultureInfo = param.CultureInfo,
                CustomerId = param.CustomerId,
                FulfillmentMethodName = fulfillmentMethod?.Name,
                FulfillmentScheduledTimeBeginDate = oldShipment?.FulfillmentScheduledTimeBeginDate,
                FulfillmentScheduledTimeEndDate = oldShipment?.FulfillmentScheduledTimeEndDate,
                PropertyBag = oldShipment?.PropertyBag,
                Id = newShipmentId,
                ScopeId = param.ScopeTo,
                ShippingAddress = string.IsNullOrEmpty(oldShipment?.Address?.City) || isFulFillmentMethodChanged ? null : oldShipment.Address,
                ShippingProviderId = fulfillmentMethod?.ShippingProviderId ?? Guid.Empty
            };

            if (param.NewStore.FulfillmentLocation != null)
            {
                updateShipmentParam.FulfillmentLocationId = param.NewStore.FulfillmentLocation.Id;
            }

            if (fulfillmentMethod.FulfillmentMethodType == FulfillmentMethodType.PickUp)
            {
                updateShipmentParam.PickUpLocationId = param.NewStore.Id;
                var storeAddress = param.NewStore.FulfillmentLocation.Addresses.FirstOrDefault();
                if (storeAddress != null)
                {
                    updateShipmentParam.ShippingAddress = new Address
                    {
                        AddressName = param.NewStore.Name,
                        City = storeAddress.City,
                        CountryCode = storeAddress.CountryCode,
                        Line1 = storeAddress.Line1,
                        Line2 = storeAddress.Line2,
                        PhoneNumber = param.NewStore.PhoneNumber ?? storeAddress.PhoneNumber,
                        PostalCode = storeAddress.PostalCode,
                        RegionCode = storeAddress.RegionCode,
                        PropertyBag = storeAddress.PropertyBag
                    };
                }

            }

            return await CartRepository.UpdateShipmentAsync(updateShipmentParam).ConfigureAwait(false);
        }

        private async Task AddPayment(MoveCartParam param, Address billingAddress)
        {
            await CartRepository.AddPaymentAsync(new AddPaymentParam
            {
                CartName = CartConfiguration.ShoppingCartName,
                CultureInfo = param.CultureInfo,
                CustomerId = param.CustomerId,
                Scope = param.ScopeTo,
                BillingAddress = billingAddress
            }).ConfigureAwait(false);
        }

        private async Task AddLineItems(MoveCartParam param, List<LineItem> currentLineItems)
        {
            if (currentLineItems == null || currentLineItems.Count == 0) return;

            await CartRepository.AddLineItemsAsync(new AddLineItemsParam
            {
                CartName = CartConfiguration.ShoppingCartName,
                CultureInfo = param.CultureInfo,
                CustomerId = param.CustomerId,
                LineItems = currentLineItems.Select(x => new LineItemInfo
                {
                    Quantity = x.Quantity,
                    ProductId = x.ProductId,
                    RecurringOrderFrequencyName = x.RecurringOrderFrequencyName,
                    RecurringOrderProgramName = x.RecurringOrderProgramName,
                    VariantId = x.VariantId
                }).ToList(),
                Scope = param.ScopeTo
            }).ConfigureAwait(false);
        }
    }
}