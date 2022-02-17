///<reference path='../../../../Typings/tsd.d.ts' />
///<reference path='../../../Mvc/ComposerClient.ts' />
///<reference path='../Parameters/IGetOrderParameters.ts' />
///<reference path='../../../Events/EventHub.ts' />
///<reference path='../../../Repositories/IOrderRepository.ts' />
///<reference path='../../CartSummary/ICartService.ts' />

module Orckestra.Composer {
    'use strict';

    export class OrderService {
        protected eventHub: IEventHub = EventHub.instance();
        protected orderRepository: IOrderRepository = new OrderRepository;
        protected cartService: ICartService = CartService.getInstance();
        private cacheProvider: ICacheProvider = CacheProvider.instance();;
        private orderCacheKey = 'orderCacheKey';
        private orderConfirmationCacheKey = 'orderConfirmationCacheKey';

        public editOrder(orderNumber: string) {
            return this.orderRepository.editOrder(orderNumber)
                .then(result => {
                    if (result.CartUrl) {
                        let data = { redirectUrl: result.CartUrl };
                        this.eventHub.publish(MyAccountEvents.EditOrderStarted, { data: data });
                    }
                })
                .fail(() => {
                    ErrorHandler.instance().outputErrorFromCode('EditingOrderFailed');
                });
        }

        public saveEditOrder(orderNumber: string) {
            return this.orderRepository.saveEditOrder(orderNumber).then((result: ICompleteCheckoutResult) => {

                result.IsUpdatedOrder = true;
                this.cacheProvider.defaultCache.set(this.orderCacheKey, result).done();
                this.cacheProvider.defaultCache.set(this.orderConfirmationCacheKey, result).done();

                this.eventHub.publish(MyAccountEvents.EditOrderFinished, { data: { orderNumber } });

                this.cartService.invalidateCache();
                this.cartService.getFreshCart(true);

                if (result.NextStepUrl) {
                    window.location.href = result.NextStepUrl;
                }

            }).fail(reason => ErrorHandler.instance().outputErrorFromCode('UpdatingOrderFailed'));
        }
        
        public cancelOrder(orderNumber: string) {
            return this.orderRepository.cancelOrder(orderNumber).then(result => this.eventHub.publish(MyAccountEvents.OrderCanceled, { data: { orderNumber } })) 
            .fail(reason => ErrorHandler.instance().outputErrorFromCode('CancelOrderFailed'));
        }

        public cancelEditOrder(orderNumber: string) {
            return this.orderRepository.cancelEditOrder(orderNumber).then(result => {
                this.eventHub.publish(MyAccountEvents.EditOrderCanceled, { data: { orderNumber } });
                return this.cartService.getFreshCart(true);
            })
                .then(cart => this.eventHub.publish(CartEvents.CartUpdated, { data: cart }))
                .fail(reason => console.log(reason));
        }

        public getPastOrders(options: IGetOrderParameters = { page: 1 }) {
            return this.orderRepository.getPastOrders(options);
        }

        public getCurrentOrders(options: IGetOrderParameters = { page: 1 }) {
            return this.orderRepository.getCurrentOrders(options);
        }
    }
}
