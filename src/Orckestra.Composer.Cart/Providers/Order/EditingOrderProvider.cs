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
using System.Linq;
using System.Threading.Tasks;
using Orckestra.Overture.ServiceModel.Orders.Fulfillment;

namespace Orckestra.Composer.Cart.Providers.Order
{
    public class EditingOrderProvider: IEditingOrderProvider
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
            if (order?.Cart?.Shipments == null ||
                OrderHistoryConfiguration.CompletedOrderStatuses.Contains(order.OrderStatus))
            {
                return false;
            }

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
                    ?.Split('|')
                    .Contains(item) ?? false);

            return isOrderEditable;
        }

        public virtual async Task<bool> IsOrderCancelable(Overture.ServiceModel.Orders.Order order)
        {
            if (order.OrderStatus.Equals("Canceled", StringComparison.InvariantCultureIgnoreCase)) return false;

            var orderFulfillmentState = await OrderRepository.GetOrderFulfillmentStateAsync(new GetOrderFulfillmentStateParam
            {
                OrderId = order.Id,
                ScopeId = order.ScopeId
            }).ConfigureAwait(false);

            if (orderFulfillmentState.IsCancelable 
                && !orderFulfillmentState.IsProcessing
                && !orderFulfillmentState.Status.Equals("Canceled", StringComparison.InvariantCultureIgnoreCase)
                && !orderFulfillmentState.Status.Equals("Completed", StringComparison.InvariantCultureIgnoreCase))
                return true;

            return false;
        }

        public virtual async Task<bool> IsOrderPendingCancel(Overture.ServiceModel.Orders.Order order)
        {
            if (order.OrderStatus.Equals("Canceled", StringComparison.InvariantCultureIgnoreCase)) return false;

            var orderFulfillmentState = await OrderRepository.GetOrderFulfillmentStateAsync(
                new GetOrderFulfillmentStateParam
                {
                    OrderId = order.Id,
                    ScopeId = order.ScopeId
                }).ConfigureAwait(false);

            return !orderFulfillmentState.IsCancelable;
        }
        
        public virtual bool IsBeingEdited(Overture.ServiceModel.Orders.Order order)
        {
            var guidOrderId = Guid.Parse(order.Id);
            return IsEditMode() & ComposerContext.EditingCartName == guidOrderId.ToString("N");
        }

        public virtual async Task<ProcessedCart> StartEditOrderModeAsync(Overture.ServiceModel.Orders.Order order)
        {
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
                if (ownedBySomeoneElseError != null)
                {
                    draftCart = await ChangeOwnership(order).ConfigureAwait(false);
                }
                else if (ownedByRequestedUserError != null)
                {
                    draftCart = await CartRepository.GetCartAsync(new GetCartParam
                    {
                        Scope = order.ScopeId,
                        CartName = orderId.ToString("N"),
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

            return draftCart;
        }

        public virtual async Task CancelEditOrderAsync(Overture.ServiceModel.Orders.Order order)
        {
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

        public virtual void ClearEditMode()
        {
            ComposerContext.EditingCartName = default;
        }

        public virtual string GetCurrentEditingCartName()
        {
            return ComposerContext.EditingCartName;
        }

        public bool IsEditMode()
        {
            return !String.IsNullOrEmpty(ComposerContext.EditingCartName);
        }
    }
}
