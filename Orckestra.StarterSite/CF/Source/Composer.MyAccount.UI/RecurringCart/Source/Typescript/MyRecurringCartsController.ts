///<reference path='../../../../Composer.UI/Source/Typings/tsd.d.ts' />
///<reference path='../../../../Composer.UI/Source/TypeScript/Mvc/Controller.ts' />
///<reference path='../../../../Composer.UI/Source/TypeScript/Mvc/IControllerActionContext.ts' />
///<reference path='./RecurringCartsController.ts' />
///<reference path='../../../../Composer.Cart.UI/RecurringOrder/source/TypeScript/Services/RecurringOrderService.ts' />
///<reference path='../../../../Composer.Cart.UI/RecurringOrder/source/TypeScript/Repositories/RecurringOrderRepository.ts' />

module Orckestra.Composer {

    export class MyRecurringCartsController extends Orckestra.Composer.RecurringCartsController {
        private recurringOrderService: IRecurringOrderService = new RecurringOrderService(new RecurringOrderRepository(), this.eventHub);

        public initialize() {
            super.initialize();

            this.getUpcomingOrders();
        }

        private getUpcomingOrders() {
            var busyHandle = this.asyncBusy();

            this.recurringOrderService.getRecurringOrderCartsByUser()
                .done((viewModel) => {

                    if (!_.isEmpty(viewModel)) {
                        this.render('MyRecurringCarts', viewModel);
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
