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

        public editOrder(orderNumber: string) {
            return this.orderRepository.editOrder(orderNumber)
                .then(result => {
                    if (result.CartUrl) {
                        let data = { redirectUrl: result.CartUrl };
                        this.eventHub.publish(MyAccountEvents.EditOrderStarted, { data: data });
                    }
                })
                .fail(reason => {
                    console.log(reason);
                    ErrorHandler.instance().outputErrorFromCode('EditingOrderFailed');
                });
        }

        public cancelOrder(orderNumber: string) {
            return this.orderRepository.cancelOrder(orderNumber).then(result => this.eventHub.publish(MyAccountEvents.OrderCanceled, { data: { orderNumber } })) 
            .fail(reason => ErrorHandler.instance().outputErrorFromCode('CancelOrderFailed'));
        }

        public saveEditOrder() {
            return this.orderRepository.saveEditOrder();
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
