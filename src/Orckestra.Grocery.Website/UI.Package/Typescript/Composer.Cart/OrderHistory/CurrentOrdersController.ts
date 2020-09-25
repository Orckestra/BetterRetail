/// <reference path='../../../Typings/tsd.d.ts' />
/// <reference path='../../JQueryPlugins/ISerializeObjectJqueryPlugin.ts' />
/// <reference path='../../Mvc/Controller.ts' />
/// <reference path='../../Mvc/IControllerContext.ts' />
/// <reference path='../../Mvc/IControllerActionContext.ts' />
///<reference path='./Services/OrderService.ts' />

module Orckestra.Composer {
    'use strict';

    export class CurrentOrdersController extends Controller {
        protected orderService = new OrderService();
        protected VueCurrentOrderData: Vue;

        public initialize() {
            super.initialize();
            let self: CurrentOrdersController = this;
            self.orderService.getCurrentOrders().then(data => {
                this.VueCurrentOrderData = new Vue({
                    el: '#vueCurrentOrders',
                    data: {
                        Orders: data ? data.Orders : null,
                        Pagination: data ? data.Pagination : null,
                        Loading: false
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
                        }
                    }
                })
            });
        }
    }
}
