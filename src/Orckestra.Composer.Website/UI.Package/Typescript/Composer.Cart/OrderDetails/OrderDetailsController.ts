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
            this.uiModal = new UIModal(window, cancelModalElementSelector, this.cancelOrder, this);
            
            let self = this;
            this.VueOrderDetails = new Vue({
                el: '#vueOrderDetails',
                data: {
                    Loading: false,
                    Modal: {
                        cancelOrderModal: null,
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
                    },
                    reload(){
                        window.location.reload()
                    }
                }
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
