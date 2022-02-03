/// <reference path='../../../Typings/tsd.d.ts' />
/// <reference path='../../JQueryPlugins/ISerializeObjectJqueryPlugin.ts' />
/// <reference path='../../Mvc/Controller.ts' />
/// <reference path='../../Mvc/IControllerContext.ts' />
/// <reference path='../../Mvc/IControllerActionContext.ts' />
///<reference path='./Services/OrderService.ts' />
/// <reference path='../../ErrorHandling/ErrorHandler.ts' />
///<reference path='../../Utils/OrderHelper.ts' />

module Orckestra.Composer {
    'use strict';

    export class CurrentOrdersController extends Controller {
        protected orderService = new OrderService();
        protected VueCurrentOrderData: Vue;
        private uiModal: UIModal;

        public initialize() {
            super.initialize();
            var cancelModalElementSelector = '#cancelOrderModal';
            this.uiModal = new UIModal(window, cancelModalElementSelector, this.cancelOrder, this);
            let self: CurrentOrdersController = this;
            self.orderService.getCurrentOrders().then(data => {
                this.VueCurrentOrderData = new Vue({
                    el: '#vueCurrentOrders',
                    data: {
                        Orders: data ? OrderHelper.MapOrders(data.Orders) : null,
                        Pagination: data ? data.Pagination : null,
                        Page: 1,
                        Loading: false
                    },

                    methods: {
                        getOrders(page: any) {                            
                            this.Loading = true;
                            self.orderService.getCurrentOrders({ page })
                                .then(data => {
                                    this.Orders = OrderHelper.MapOrders(this.data.Orders);
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
                                .fin(() => {
                                    this.Loading = false;
                                });
                        },
                        cancelEditingOrder(orderNumber: string) {
                            if (this.Loading) return;
                            this.Loading = true;
                            self.orderService.cancelEditOrder(orderNumber)
                                .then(() => this.getOrders(this.Page))
                                .fail(() => {
                                    this.Loading = false;
                                });
                        },
                        reload(){
                            window.location.reload()
                        }
                    }
                })
            });
        }

        public cancelOrderConfirm(actionContext: IControllerActionContext) {
            this.uiModal.openModal(actionContext.event);
        }

        public cancelOrder(event){
            var element = $(event.target);
            var $orderItem: JQuery = element.closest('[data-orderid]');
            var orderId = $orderItem.data('orderid');
            this.orderService.cancelOrder(orderId);            
        }
    }
}
