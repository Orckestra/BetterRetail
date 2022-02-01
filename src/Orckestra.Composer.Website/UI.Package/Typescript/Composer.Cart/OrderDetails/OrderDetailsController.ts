/// <reference path='../../../Typings/tsd.d.ts' />
/// <reference path='../../JQueryPlugins/ISerializeObjectJqueryPlugin.ts' />
/// <reference path='../../Mvc/Controller.ts' />
/// <reference path='../../Mvc/IControllerContext.ts' />
/// <reference path='../../Mvc/IControllerActionContext.ts' />
///<reference path='../OrderHistory/Services/OrderService.ts' />
///<reference path='../../UI/UIModal.ts' />

module Orckestra.Composer {
    'use strict';

    export class OrderDetailsController extends Controller {
        protected orderService = new OrderService();
        protected VueOrderDetails: Vue;
        private uiModal: UIModal;

        public initialize() {
            super.initialize();
            var cancelModalElementSelector = '#cancelOrderModal';
            
            let self = this;
            this.VueOrderDetails = new Vue({
                el: '#vueOrderDetails',
                data: {
                    Loading: false,
                    Modal: {
                        cancelOrderModal: null,
                    }
                },
                mounted: function () {
                    if (this.Mode.Authenticated) {
                        this.Modal.uiModal = new UIModal(window, cancelModalElementSelector, this.cancelOrder, this);
                    }
                },
                methods: {
                    editOrder(orderNumber: string) {
                        if(this.Loading) return;
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
                            .fin(() => {
                                this.Loading = false;
                            });
                    } 
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
            });
        }
    }
}
