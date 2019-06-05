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
        private viewModel;
        protected modalElementSelector: string = '#confirmationModal';
        private uiModal: UIModal;

        protected customerService: ICustomerService = new CustomerService(new CustomerRepository());
        protected recurringCartAddressRegisteredService: RecurringCartAddressRegisteredService =
            new RecurringCartAddressRegisteredService(this.customerService);

        public initialize() {

            super.initialize();
            this.viewModelName = 'MyRecurringScheduleDetails';

            this.getRecurringTemplateDetail();
            this.uiModal = new UIModal(window, this.modalElementSelector, this.deleteAddress, this);
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
                    this.viewModel = result;

                    this.id = id;
                    this.reRenderPage(result.RecurringOrderTemplateLineItemViewModels[0]);
                })
                .fail((reason) => {
                    console.error(reason);
                });
        }

        public getAvailableEditList() {
            this.getAddresses();
            this.getShippingMethodsList();
            this.getPaymentMethods();
        }

        public reRenderPage(vm) {
            //this.viewModel = vm;
            this.render(this.viewModelName, vm);
            this.getAvailableEditList();
        }

        public renderShippingMethods(vm) {
            this.render('RecurringScheduleDetailsShippingMethods', vm);
        }
        public renderAddresses(vm) {
            this.render('RecurringScheduleDetailsAddresses', vm);
        }

        public getAddresses() {
            this.recurringCartAddressRegisteredService.getRecurringTemplateAddresses(this.id)
                .then((addressesVm) => {
                    addressesVm.SelectedBillingAddressId = this.viewModel.RecurringOrderTemplateLineItemViewModels[0].BillingAddressId;
                    addressesVm.SelectedShippingAddressId = this.viewModel.RecurringOrderTemplateLineItemViewModels[0].ShippingAddressId;

                    addressesVm.UseShippingAddress = addressesVm.SelectedBillingAddressId === addressesVm.SelectedShippingAddressId;

                    console.log(addressesVm);
                    this.renderAddresses(addressesVm);
                });
        }
        public getShippingMethodsList() {

            let shippingMethodName = this.viewModel.RecurringOrderTemplateLineItemViewModels[0].ShippingMethodName;
            this.getShippingMethods()
                .then(shippingMethods => {

                    if (!shippingMethods) {
                        throw new Error('No viewModel received');
                    }

                    if (_.isEmpty(shippingMethods.ShippingMethods)) {
                        throw new Error('No shipping method was found.');
                    }

                    let selectedShippingMethodName = shippingMethodName;
                    shippingMethods.ShippingMethods.forEach(shippingMethod => {

                        if (shippingMethod.Name === selectedShippingMethodName) {
                            shippingMethods.SelectedShippingProviderId = shippingMethod.ShippingProviderId;
                        }
                    });

                    var vm = {
                        ShippingMethods: shippingMethods,
                        SelectedMethod: selectedShippingMethodName
                    };

                    console.log(vm);

                    this.renderShippingMethods(vm);
                });
        }

        public getShippingMethods() : Q.Promise<any> {
            return this.recurringOrderService.getOrderTemplateShippingMethods()
                    .fail((reason) => {
                        console.error('Error while retrieving shipping methods', reason);
                    });
        }

        public getPaymentMethods() {
            //TODO
        }

        private useShippingAddress() : Boolean {
            var useShippingAddress = $(this.context.container).find('input[name=UseShippingAddress]:checked').val() === 'true';
            return useShippingAddress;
        }


        public changeUseShippingAddress() {

            this.setBillingAddressFormVisibility();
            this.setSelectedBillingAddress();
            //TODO: form validation?
        }

        private setBillingAddressFormVisibility() {

            var useShippingAddress: Boolean = this.useShippingAddress();
            if (useShippingAddress) {
                $('#BillingAddressContent').addClass('hide');
            } else {
                $('#BillingAddressContent').removeClass('hide');
            }
        }

        protected setSelectedBillingAddress() {

            var selectedBillingAddressId: string = $(this.context.container).find('input[name=BillingAddressId]:checked').val();

            if (!selectedBillingAddressId) {
                return;
            }
        }

        public deleteAddressConfirm(actionContext: IControllerActionContext) {
            this.uiModal.openModal(actionContext.event);
        }

          /**
         * Requires the element in action context to have a data-address-id.
         */
        protected deleteAddress(event: JQueryEventObject): Q.Promise<void> {

            let element = $(event.target);
            var $addressListItem = element.closest('[data-address-id]');
            var addressId = $addressListItem.data('address-id');

            var busy = this.asyncBusy({elementContext: element, containerContext: $addressListItem });

            return this.customerService.deleteAddress(addressId, '')
                .then(result => {
                    this.reRenderPage(this.viewModel);
                })
                .fin(() => busy.done());
        }
    }
}
