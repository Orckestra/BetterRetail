/// <reference path='../../../../Composer.UI/Source/Typings/tsd.d.ts' />
/// <reference path='../../../../Composer.UI/Source/TypeScript/JqueryPlugins/ISerializeObjectJqueryPlugin.ts' />
/// <reference path='../../../../Composer.UI/Source/TypeScript/Mvc/Controller.ts' />
/// <reference path='../../../../Composer.UI/Source/TypeScript/Mvc/IControllerContext.ts' />
/// <reference path='../../../../Composer.UI/Source/TypeScript/Mvc/IControllerActionContext.ts' />
///<reference path='./Services/OrderService' />

module Orckestra.Composer {
    'use strict';

    export class PastOrdersController extends Controller {
        protected orderService = new OrderService();

        public initialize() {
            super.initialize();

            this.getPastOrders();
        }

        public getOrders(context: IControllerActionContext) {
            var page: number = context.elementContext.data('page');

            context.event.preventDefault();
            this.getPastOrders({ page: page });
        }

        private getPastOrders(param? : IGetOrderParameters) {
            var busyHandle = this.asyncBusy();

            this.orderService.getPastOrders(param)
                .done((viewModel) => {
                    this.render('PastOrders', viewModel);

                    if (!_.isEmpty(viewModel)) {
                        this.render('OrderHistoryPagination', viewModel.Pagination);
                    }

                    busyHandle.done();
                },
                (reason) => {
                    console.error(reason);
                    busyHandle.done();
                });
        }
    }
}
