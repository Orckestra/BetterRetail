/// <reference path='../../../Typings/tsd.d.ts' />
/// <reference path='../../JQueryPlugins/ISerializeObjectJqueryPlugin.ts' />
/// <reference path='../../Mvc/Controller.ts' />
/// <reference path='../../Mvc/IControllerContext.ts' />
/// <reference path='../../Mvc/IControllerActionContext.ts' />
///<reference path='./Services/OrderService.ts' />

module Orckestra.Composer {
    'use strict';

    export class PastOrdersController extends Controller {
        protected orderService = new OrderService();
        protected VuePastOrderData: Vue;


        public initialize() {
            super.initialize();

            let self: PastOrdersController = this;

            self.orderService.getPastOrders().then(data => {
                this.VuePastOrderData = new Vue({
                    el: '#vuePastOrders',
                    data: {
                        Orders: data ? OrderHelper.MapOrders(data.Orders) : null,
                        Pagination: data ? data.Pagination : null,
                        Loading: false
                    },
                    methods: {
                        getOrders(page: any) {
                            this.Loading = true;
                            self.orderService.getPastOrders({ page }).then(data => {
                                this.Orders = OrderHelper.MapOrders(data.Orders);
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
