/// <reference path='../../../Typings/tsd.d.ts' />
/// <reference path='../../JQueryPlugins/ISerializeObjectJqueryPlugin.ts' />
/// <reference path='../../Mvc/Controller.ts' />
/// <reference path='../../Mvc/IControllerContext.ts' />
/// <reference path='../../Mvc/IControllerActionContext.ts' />
///<reference path='../OrderHistory/Services/OrderService.ts' />

module Orckestra.Composer {
    'use strict';

    export class OrderDetailsController extends Controller {
        protected orderService = new OrderService();
        protected VueOrderDetails: Vue;

        public initialize() {
            super.initialize();
            let self = this;
            this.VueOrderDetails = new Vue({
                el: '#vueOrderDetails',
                data: {
                    Loading: false
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
                }
            });
        }
    }
}
