///<reference path='../../../../Composer.UI/Source/Typings/tsd.d.ts' />
///<reference path='../../../../Composer.UI/Source/TypeScript/Mvc/Controller.ts' />
///<reference path='../../../../Composer.UI/Source/TypeScript/Mvc/IControllerActionContext.ts' />
///<reference path='./RecurringScheduleDetailsController.ts' />
///<reference path='../../../../Composer.Cart.UI/RecurringOrder/source/TypeScript/Services/RecurringOrderService.ts' />
///<reference path='../../../../Composer.Cart.UI/RecurringOrder/source/TypeScript/Services/IRecurringOrderService.ts' />
///<reference path='../../../../Composer.Cart.UI/RecurringOrder/source/TypeScript/Repositories/RecurringOrderRepository.ts' />

module Orckestra.Composer {

    export class MyRecurringScheduleDetailsController extends Orckestra.Composer.RecurringScheduleDetailsController {
        private recurringOrderService: IRecurringOrderService = new RecurringOrderService(new RecurringOrderRepository(), this.eventHub);

        private viewModelName = '';
        private id = '';

        protected customerService: ICustomerService = new CustomerService(new CustomerRepository());
        protected recurringCartAddressRegisteredService: RecurringCartAddressRegisteredService =
            new RecurringCartAddressRegisteredService(this.customerService);

        public initialize() {

            super.initialize();
            this.viewModelName = 'MyRecurringScheduleDetails';

            this.getRecurringTemplateDetail();
        }

        public getRecurringTemplateDetail() {

            var nameUrlQueryString: string = 'id=';
            var id: string = '';

            if (window.location.href.indexOf(nameUrlQueryString) > -1) {
                id = window.location.href.substring(window.location.href.indexOf(nameUrlQueryString)
                    + nameUrlQueryString.length);
            }

            this.recurringOrderService.getRecurringTemplateDetail(id)
                .then(result => {
                    console.log(result);
                    //this.viewModel = result;

                    this.id = id;
                    this.getAvailableEditList();
                    this.reRenderPage(result);
                })
                .fail((reason) => {
                    console.error(reason);
                });
        }

        public getAvailableEditList() {
            this.getAddresses();
            this.getShippingMethods();
            this.getPaymentMethods();
        }

        public reRenderPage(vm) {
            //this.viewModel = vm;
            this.render(this.viewModelName, vm);
        }

        public renderShippingMethods(vm) {
            this.render('', vm);
        }
        public renderAddresses(vm) {
            this.render('RecurringScheduleDetailsAddresses', vm);
        }

        public getAddresses() {
            this.recurringCartAddressRegisteredService.getRecurringTemplateAddresses(this.id)
                .then((addressesVm) => {
                    //addressesVm.EditMode = this.editAddress;
                    //addressesVm.Payment = {
                      //  BillingAddress: {
                        //    UseShippingAddress: this.viewModel.Payment.BillingAddress.UseShippingAddress
                        //}
                    //};

                    console.log(addressesVm);
                    this.renderAddresses(addressesVm);
                });
        }
        public getShippingMethods() {
            //TODO
        }
        public getPaymentMethods() {
            //TODO
        }
    }
}
