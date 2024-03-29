/// <reference path='../../../Typings/tsd.d.ts' />
/// <reference path='../../JQueryPlugins/ISerializeObjectJqueryPlugin.ts' />
/// <reference path='../../Mvc/Controller.ts' />
/// <reference path='../../Mvc/IControllerContext.ts' />
/// <reference path='../../Mvc/IControllerActionContext.ts' />
///<reference path='./Services/OrderService.ts' />
/// <reference path='../../ErrorHandling/ErrorHandler.ts' />
///<reference path='../../UI/UIModal.ts' />

module Orckestra.Composer {
    'use strict';

    export class CurrentOrdersController extends Controller {
        protected orderService = new OrderService();
        protected VueCurrentOrderData: Vue;

        public initialize() {
            super.initialize();
            var cancelModalElementSelector = '#cancelOrderModal';
            let self: CurrentOrdersController = this;

            self.orderService.getCurrentOrders().then(data => {
                this.VueCurrentOrderData = new Vue({
                    el: '#vueCurrentOrders',
                    data: {
                        Orders: data ? data.Orders : null,
                        Pagination: data ? data.Pagination : null,
                        Page: 1,
                        Loading: false,
                        Modal: {
                            cancelOrderModal: null,
                        },
                        OrderNumber: null
                    },
                    mounted() {
                        this.Modal.cancelOrderModal = new Composer.UIModal(window, cancelModalElementSelector, this.cancelOrder, this);

                        self.eventHub.subscribe(MyAccountEvents.EditOrderCanceled, () => this.getOrders(this.Page));
                        self.eventHub.subscribe(MyAccountEvents.OrderCanceled, () => this.reload());
                    },
                    methods: {
                        getOrders(page: any) {
                            this.Loading = true;
                            self.orderService.getCurrentOrders({ page })
                                .then(data => {
                                    this.Orders = data.Orders;
                                    this.Pagination = data.Pagination;
                                    this.Page = page;
                                })
                                .fail(reason => console.log(reason))
                                .fin(() => this.Loading = false);
                        },
                        editOrder(orderNumber: string) {
                            if (this.Loading) return;
                            this.Loading = true;
                            self.eventHub.publish(MyAccountEvents.StartEditOrder, { data: orderNumber });
                            self.orderService.editOrder(orderNumber)
                                .fin(() => this.Loading = false);
                        },
                        cancelEditingOrder(orderNumber: string) {
                            if (this.Loading) return;
                            this.Loading = true;
                            self.orderService.cancelEditOrder(orderNumber)
                                .fin(() => this.Loading = false);
                        },
                        reload() {
                            window.location.reload()
                        },
                        cancelOrderConfirm(event: JQueryEventObject, orderNumber: string) {
                            this.OrderNumber = orderNumber;
                            this.Modal.cancelOrderModal.openModal(event);
                        },
                        cancelOrder() {
                            if (this.Loading) return;
                            this.Loading = true;
                            self.orderService.cancelOrder(this.OrderNumber).fin(() => this.Loading = false);
                        }
                    }
                })
            });
        }
    }
}
