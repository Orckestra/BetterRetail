///<reference path='../../../../Typings/tsd.d.ts' />
///<reference path='../../../Mvc/ComposerClient.ts' />
///<reference path='../Parameters/IGetOrdersParameter.ts' />
///<reference path='../../../Events/EventHub.ts' />
///<reference path='../../../Repositories/IOrderRepository.ts' />
///<reference path='../../../Repositories/OrderRepository.ts' />
///<reference path='../../CartSummary/ICartService.ts' />
///<reference path='../../CartSummary/CartService.ts' />
///<reference path='../../../Composer.Grocery/IFulfillmentService.ts' />
///<reference path='../../../Composer.Grocery/FulfillmentEvents.ts' />
///<reference path='../../../Composer.Grocery/FulfillmentService.ts' />
///<reference path='../../../Composer.MyAccount/Common/MyAccountEvents.ts' />
///<reference path='../../../ErrorHandling/ErrorHandler.ts' />

module Orckestra.Composer {
    'use strict';

    export class OrderService {
        protected eventHub: IEventHub = EventHub.instance();
        protected orderRepository: IOrderRepository = new OrderRepository;
        protected cartService: ICartService = CartService.getInstance();
        protected fulfillmentService: IFulfillmentService = FulfillmentService.instance();
        private cacheProvider: ICacheProvider = CacheProvider.instance();
        private orderCacheKey = 'orderCacheKey';
        private orderConfirmationCacheKey = 'orderConfirmationCacheKey';

        public editOrder(orderNumber: string) {
            return this.orderRepository.editOrder(orderNumber)
                .then(result => {
                    if (result.CartUrl) {
                        let data = { redirectUrl: result.CartUrl, orderNumber: orderNumber };
                        this.eventHub.publish(MyAccountEvents.EditOrderStarted, { data: data });
                    }
                })
                .fail(() => ErrorHandler.instance().outputErrorFromCode('EditingOrderFailed'));
        }

        public saveEditOrder(orderNumber: string) {
            return this.orderRepository.saveEditOrder(orderNumber).then((result: ICompleteCheckoutResult) => {

                result.IsUpdatedOrder = true;
                this.cacheProvider.defaultCache.set(this.orderCacheKey, result).done();
                this.cacheProvider.defaultCache.set(this.orderConfirmationCacheKey, result).done();

                this.eventHub.publish(MyAccountEvents.EditOrderFinished, { data: { orderNumber } });
                this.cartService.invalidateCache();
      
                if (result.NextStepUrl) {
                    window.location.href = result.NextStepUrl;
                }
                

            }).fail(() => ErrorHandler.instance().outputErrorFromCode('UpdatingOrderFailed'));
        }

        public cancelOrder(orderNumber: string) {
            return this.orderRepository.cancelOrder(orderNumber).then(result => this.eventHub.publish(MyAccountEvents.OrderCanceled, { data: { orderNumber } }))
                .fail(reason => ErrorHandler.instance().outputErrorFromCode('CancelOrderFailed'));
        }

        public cancelEditOrder(orderNumber: string) {
            return this.orderRepository.cancelEditOrder(orderNumber)
                .then(() => {
                    this.eventHub.publish(MyAccountEvents.EditOrderCanceled, { data: { orderNumber } });
                    return this.fulfillmentService.restoreFulfillment(true);
                })
                .then(() => {
                    this.cartService.invalidateCache();
                    return this.cartService.getFreshCart(true);
                })
                .then(cart => this.eventHub.publish(CartEvents.CartUpdated, { data: cart }))
                .then(() => {
                    return this.fulfillmentService.getSelectedFulfillment()
                })
                .then(fulfillment => {
                    this.eventHub.publish(FulfillmentEvents.StoreSelected, { data: fulfillment.Store });
                    this.eventHub.publish(FulfillmentEvents.TimeSlotSelected, {
                        data: { TimeSlot: fulfillment.TimeSlot, TimeSlotReservation: fulfillment.TimeSlotReservation }
                    });
                })
                .fail(reason => console.log(reason));
        }

        public getPastOrders(options: IGetOrdersParameter = { page: 1 }) {
            return this.orderRepository.getPastOrders(options);
        }

        public getCurrentOrders(options: IGetOrdersParameter = { page: 1 }) {
            return this.orderRepository.getCurrentOrders(options);
        }

        public getOrderByNumber(orderNumber: string) {
            return this.orderRepository.getOrderByNumber(orderNumber);
        }

        public getGuestOrderByNumber(orderNumber: string, email: string) {
            return this.orderRepository.getGuestOrderByNumber(orderNumber, email);
        }

    }
}

