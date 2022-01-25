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
                    editOrder(orderNumber: string, orderId: string) {
                        if(this.Loading) return;
                        this.Loading = true;
                        self.eventHub.publish(MyAccountEvents.StartEditOrder, { data: orderId });
                        self.orderService.editOrder(orderNumber, orderId)
                            .then(result => {
                                if (result.IsEditingOrder) {
                                    let data = { redirectUrl: result.CartUrl };
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
                    }
                }
            });
        }
    }
}
