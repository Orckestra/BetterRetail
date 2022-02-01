/// <reference path='../../../Typings/tsd.d.ts' />
/// <reference path='../../JQueryPlugins/ISerializeObjectJqueryPlugin.ts' />
/// <reference path='../../Mvc/Controller.ts' />
/// <reference path='../../Mvc/IControllerContext.ts' />
/// <reference path='../../Mvc/IControllerActionContext.ts' />
///<reference path='./Services/OrderService.ts' />
/// <reference path='../../ErrorHandling/ErrorHandler.ts' />

module Orckestra.Composer {
    'use strict';

    export class CurrentOrdersController extends Controller {
        protected orderService = new OrderService();
        protected VueCurrentOrderData: Vue;
        private uiModal: UIModal;

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
                        Loading: false,
                        mounted: function () {
                            if (this.Mode.Authenticated) {
                                this.Modal.uiModal = new UIModal(window, cancelModalElementSelector, this.cancelOrder, this);
                            }
                        },
                    },

                    methods: {
                        getOrders(page: any) {
                            this.Loading = true;
                            self.orderService.getCurrentOrders({ page })
                                .then(data => {
                                    this.Orders = data.Orders;
                                    this.Pagination = data.Pagination;
                                })
                                .fail(reason => console.log(reason))
                                .fin(() => this.Loading = false);
                        },
                        editOrder(orderNumber: string) {
                            if(this.Loading) return;
                            this.Loading = true;
                            self.eventHub.publish(MyAccountEvents.StartEditOrder, { data: orderNumber });
                            self.orderService.editOrder(orderNumber)
                                .then(result => {
                                    if(result.CartUrl) {
                                        let data =  { redirectUrl: result.CartUrl };
                                        self.eventHub.publish(MyAccountEvents.EditOrderChanged, { data: data });
                                    }
                                })
                                .fail(reason => {
                                    console.log(reason);
                                    ErrorHandler.instance().outputErrorFromCode('EditingOrderFailed');
                                })
                                .fin(() => {
                                    this.Loading = false;
                                });
                        },
                        cancelOrderConfirm: function (actionContext) {
                            this.Modal.uiModal.openModal(actionContext.event);
                        },
                        cancelOrder(event){
                            var element = $(event.target);
                            var $orderItem = element.closest('[data-orderId]');
                            var orderId = $orderItem.data('orderId');
                            if(this.Loading) return;
                            this.Loading = true;
                            self.orderService.cancelOrder(orderId)
                            .then(result => {
                                console.log(result)
                            })
                            .fail(reason => {
                                console.log(reason);
                                ErrorHandler.instance().outputErrorFromCode('CancelingOrderFailed');
                            })
                            .fin(() => {
                                this.Loading = false;
                            });
                        }
                    }
                })
            });
        }
    }
}
