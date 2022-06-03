using Orckestra.Composer.Cart.Extensions;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Parameters.Order;
using Orckestra.Composer.Cart.Repositories;
using Orckestra.Composer.Cart.Repositories.Order;
using Orckestra.Composer.Exceptions;
using Orckestra.Composer.Services;
using Orckestra.Composer.Utils;
using Orckestra.Overture.ServiceModel.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Orckestra.Overture.ServiceModel;
using Orckestra.Overture.ServiceModel.Orders.Fulfillment;

namespace Orckestra.Composer.Cart.Providers.Order
{
    public class EditingOrderProvider : IEditingOrderProvider
    {
        protected virtual IOrderRepository OrderRepository { get; private set; }
        protected virtual ICartRepository CartRepository { get; private set; }
        protected virtual IComposerContext ComposerContext { get; private set; }

        public EditingOrderProvider(IOrderRepository orderRepository, ICartRepository cartRepository, IComposerContext composerContext)
        {
            OrderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            CartRepository = cartRepository ?? throw new ArgumentNullException(nameof(cartRepository));
            ComposerContext = composerContext ?? throw new ArgumentNullException(nameof(composerContext));
        }

        public virtual async Task<bool> CanEdit(Overture.ServiceModel.Orders.Order order)
        {
            if (!ComposerContext.IsAuthenticated || !ValidateOrder(order)) return false;

            var orderSettings = await OrderRepository.GetOrderSettings(ComposerContext.Scope).ConfigureAwait(false);

            var shipmentStatuses = order.Cart.GetAllShipmentStatuses();
            if (!shipmentStatuses.Any()
                || orderSettings == null
                || string.IsNullOrWhiteSpace(orderSettings.EditableShipmentStates))
            {
                return false;
            }

            var isOrderEditable = shipmentStatuses
                .All(item => orderSettings
                    .EditableShipmentStates
                    .ToLower()
                    .Split('|')
                    .Contains(item.ToLower()));

            return isOrderEditable;
        }

        public virtual async Task<CancellationStatus> GetCancellationStatus(Overture.ServiceModel.Orders.Order order)
        {
            if (!ComposerContext.IsAuthenticated || !ValidateOrder(order))
            {
                return new CancellationStatus();
            }
            var orderFulfillmentState = await OrderRepository.GetOrderFulfillmentStateAsync(new GetOrderFulfillmentStateParam
            {
                OrderId = order.Id,
                ScopeId = order.ScopeId
            }).ConfigureAwait(false);

            if (orderFulfillmentState == null || OrderHistoryConfiguration.CompletedOrderStatuses.Contains(orderFulfillmentState.Status))
            {
                return new CancellationStatus();
            }

            switch (orderFulfillmentState.IsCancelable)
            {
                case true when !orderFulfillmentState.IsProcessing && orderFulfillmentState.ShipmentFulfillmentStates.All(item => item.AllowedStatusChanges?.Contains(Constants.OrderStatus.Canceled) ?? false):
                    return new CancellationStatus() { CanCancel = true, CancellationPending = false };
                case false when orderFulfillmentState.IsProcessing && HasCancellationMessage(orderFulfillmentState):
                    return new CancellationStatus() { CanCancel = false, CancellationPending = true };
                default:
                    return new CancellationStatus();
            }
        }

        private bool HasCancellationMessage(OrderFulfillmentState orderFulfillmentState)
        {
            if (orderFulfillmentState?.ShipmentFulfillmentStates == null) return false;

            return orderFulfillmentState.ShipmentFulfillmentStates.Any(item =>
                item.Messages?.Exists(el => Guid.Parse(el.MessageId) == Guid.Parse(orderFulfillmentState.OrderId.ToString())
                                            && el.PropertyBag[Constants.RequestedOrderCancellationDatePropertyBagKey] is DateTime
                                            && (DateTime)el.PropertyBag[Constants.RequestedOrderCancellationDatePropertyBagKey] >
                                            DateTime.UtcNow.AddMinutes(-10)) ?? false);
        }

        private bool ValidateOrder(Overture.ServiceModel.Orders.Order order)
        {
            return order?.Cart?.Shipments != null &&
                   !string.IsNullOrWhiteSpace(order.OrderStatus) &&
                   Guid.TryParse(order.CustomerId, out Guid orderCustomerId) &&
                   orderCustomerId != Guid.Empty &&
                   orderCustomerId == ComposerContext.CustomerId &&
                   !OrderHistoryConfiguration.CompletedOrderStatuses.Contains(order.OrderStatus);
        }
         public virtual bool IsBeingEdited(Overture.ServiceModel.Orders.Order order)
        {
            return order != null && IsEditMode() & ComposerContext.EditingCartName == order.Id.GetDraftCartName();
        }

        public virtual async Task<ProcessedCart> StartEditOrderModeAsync(Overture.ServiceModel.Orders.Order order)
        {
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order));
            }

            Guid orderId = Guid.Parse(order.Id);
            ProcessedCart draftCart = null;

            var createOrderDraftParam = new CreateCartOrderDraftParam
            {
                CultureInfo = ComposerContext.CultureInfo,
                OrderId = orderId,
                Scope = order.ScopeId,
                CustomerId = Guid.Parse(order.CustomerId)
            };

            try
            {
                draftCart = await OrderRepository.CreateCartOrderDraft(createOrderDraftParam).ConfigureAwait(false);
            }
            catch (ComposerException ex)
            {
                var ownedBySomeoneElseError = ex.Errors?.FirstOrDefault(e => e.ErrorCode == Constants.ErrorCodes.IsOwnedBySomeoneElse);
                var ownedByRequestedUserError = ex.Errors?.FirstOrDefault(e => e.ErrorCode == Constants.ErrorCodes.IsOwnedByRequestedUser);
                var cartAlreadyExistsError = ex.Errors?.FirstOrDefault(e => e.ErrorCode == Constants.ErrorCodes.CartAlreadyExists);

                if (ownedBySomeoneElseError != null)
                {
                    draftCart = await ChangeOwnership(order).ConfigureAwait(false);
                }
                else if (ownedByRequestedUserError != null || cartAlreadyExistsError != null)
                {
                    draftCart = await CartRepository.GetCartAsync(new GetCartParam
                    {
                        Scope = order.ScopeId,
                        CartName = order.Id.GetDraftCartName(),
                        CartType = CartConfiguration.OrderDraftCartType,
                        CustomerId = Guid.Parse(order.CustomerId),
                        CultureInfo = ComposerContext.CultureInfo
                    }).ConfigureAwait(false);
                }
                else
                {
                    throw;
                }
            }

            if (draftCart == null)
            {
                throw new InvalidOperationException("Expected draft cart, but received null.");
            }

            //Set Edit Mode
            ComposerContext.EditingCartName = draftCart.Name;
            ComposerContext.EditingScopeId = order.ScopeId;

            return draftCart;
        }

        public virtual async Task CancelEditOrderAsync(Overture.ServiceModel.Orders.Order order)
        {
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order));
            }

            try
            {
                await DeleteCartDraft(order).ConfigureAwait(false);
            }
            catch (ComposerException ex)
            {
                var ownedBySomeoneElseError = ex.Errors?.FirstOrDefault(e => e.ErrorCode == Constants.ErrorCodes.IsOwnedBySomeoneElse);
                if (ownedBySomeoneElseError != null)
                {
                    await ChangeOwnership(order).ConfigureAwait(false);
                    await DeleteCartDraft(order).ConfigureAwait(false);
                }
                else
                {
                    throw;
                }
            }

            ClearEditMode();
        }

        private Task DeleteCartDraft(Overture.ServiceModel.Orders.Order order)
        {
            return OrderRepository.DeleteCartOrderDraft(new DeleteCartOrderDraftParam
            {
                CustomerId = Guid.Parse(order.CustomerId),
                Scope = order.ScopeId,
                OrderId = Guid.Parse(order.Id)
            });
        }

        private Task<ProcessedCart> ChangeOwnership(Overture.ServiceModel.Orders.Order order)
        {
            return OrderRepository.ChangeOwnership(new ChangeOrderDraftOwnershipParam()
            {
                CultureName = ComposerContext.CultureInfo.Name,
                RevertPendingChanges = true,
                OrderId = Guid.Parse(order.Id),
                Scope = order.ScopeId,
                CustomerId = Guid.Parse(order.CustomerId)
            });
        }

        public virtual async Task<Overture.ServiceModel.Orders.Order> SaveEditedOrderAsync(Overture.ServiceModel.Orders.Order order)
        {
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order));
            }

            var isEditable = await CanEdit(order).ConfigureAwait(false);
            
            if(!isEditable)
            {
                await CancelEditOrderAsync(order).ConfigureAwait(false);
                throw new InvalidOperationException($"Cannot update edited order #${order.OrderNumber} as it can't be edited now.");
            }

            Overture.ServiceModel.Orders.Order updatedOrder;
            try
            {
                updatedOrder = await SaveEditedOrder(order).ConfigureAwait(false);
            }
            catch (ComposerException ex)
            {
                var ownedBySomeoneElseError = ex.Errors?.FirstOrDefault(e => e.ErrorCode == Constants.ErrorCodes.IsOwnedBySomeoneElse);
                if (ownedBySomeoneElseError != null)
                {
                    await ChangeOwnership(order).ConfigureAwait(false);
                    updatedOrder = await SaveEditedOrder(order).ConfigureAwait(false);
                }
                else
                {
                    throw;
                }
            }

            ClearEditMode();

            return updatedOrder;
        }

        private Task<Overture.ServiceModel.Orders.Order> SaveEditedOrder(Overture.ServiceModel.Orders.Order order)
        {
            return OrderRepository.SubmitCartOrderDraftAsync(new SubmitCartOrderDraftParam
            {
                CustomerId = Guid.Parse(order.CustomerId),
                Scope = order.ScopeId,
                OrderId = Guid.Parse(order.Id)
            });
        }

        public virtual void ClearEditMode()
        {
            ComposerContext.EditingCartName = default;
            ComposerContext.EditingScopeId = default;
        }

        public virtual string GetCurrentEditingCartName()
        {
            return ComposerContext.EditingCartName;
        }

        public virtual string GetCurrentEditingScopeId()
        {
            return ComposerContext.EditingScopeId;
        }

        public bool IsEditMode()
        {
            return !string.IsNullOrEmpty(ComposerContext.EditingCartName);
        }

        public async Task CancelOrder(Overture.ServiceModel.Orders.Order order)
        {
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order));
            }

            var isOrderCancelable = (await GetCancellationStatus(order).ConfigureAwait(false)).CanCancel;

            if (!isOrderCancelable) throw new InvalidOperationException($"Order {order.Id} can't be cancelled");

            var shipmentsTasks = order.Cart.Shipments.Select(shipment =>
                OrderRepository.ChangeShipmentStatusAsync(new ChangeShipmentStatusParam
                {
                    OrderId = Guid.Parse(order.Id),
                    ScopeId = order.ScopeId,
                    ShipmentId = shipment.Id,
                    Reason = Constants.DefaultOrderCancellationReason,
                    RequestedStatus = Constants.OrderStatus.Canceled
                }));

            await Task.WhenAll(shipmentsTasks).ConfigureAwait(false);

            var propertyBagShipment = new Dictionary<string, object>();
            propertyBagShipment.Add(Constants.RequestedOrderCancellationDatePropertyBagKey, DateTime.UtcNow);

            var shipmentFulfillmentMessagesTasks = order.Cart.Shipments
                .Select(shipment =>
                    OrderRepository.AddShipmentFulfillmentMessagesAsync(new AddShipmentFulfillmentMessagesParam
                    {
                        OrderId = Guid.Parse(order.Id),
                        ScopeId = order.ScopeId,
                        ShipmentId = shipment.Id,
                        ExecutionMessages = new List<ExecutionMessage>()
                        {
                            new ExecutionMessage()
                            {
                                Severity = ExecutionMessageSeverity.Info,
                                MessageId = order.Id,
                                PropertyBag = new PropertyBag(propertyBagShipment)
                            }
                        }
                    }));

            await Task.WhenAll(shipmentFulfillmentMessagesTasks).ConfigureAwait(false);
        }
    }
}
