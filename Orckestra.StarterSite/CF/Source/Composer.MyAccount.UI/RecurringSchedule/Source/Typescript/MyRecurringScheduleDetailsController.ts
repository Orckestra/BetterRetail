///<reference path='../../../../Composer.UI/Source/Typings/tsd.d.ts' />
///<reference path='../../../../Composer.UI/Source/TypeScript/Mvc/Controller.ts' />
///<reference path='../../../../Composer.UI/Source/TypeScript/Mvc/IControllerActionContext.ts' />
///<reference path='./RecurringScheduleDetailsController.ts' />
///<reference path='../../../../Composer.Cart.UI/RecurringOrder/source/TypeScript/Services/RecurringOrderService.ts' />
///<reference path='../../../../Composer.Cart.UI/RecurringOrder/source/TypeScript/Services/IRecurringOrderService.ts' />
///<reference path='../../../../Composer.Cart.UI/RecurringOrder/source/TypeScript/Repositories/RecurringOrderRepository.ts' />
///<reference path='../../../../Composer.UI/Source/TypeScript/ErrorHandling/ErrorHandler.ts' />

module Orckestra.Composer {

    export class MyRecurringScheduleDetailsController extends Orckestra.Composer.RecurringScheduleDetailsController {
        private recurringOrderService: IRecurringOrderService = new RecurringOrderService(new RecurringOrderRepository(), this.eventHub);

        private viewModelName = '';
        private id = '';
        private viewModel;
        protected modalElementSelector: string = '#confirmationModal';
        private uiModal: UIModal;
        private busyHandler: UIBusyHandle;
        private window: Window;

        protected customerService: ICustomerService = new CustomerService(new CustomerRepository());
        protected recurringCartAddressRegisteredService: RecurringCartAddressRegisteredService =
            new RecurringCartAddressRegisteredService(this.customerService);

        public initialize() {

            super.initialize();
            this.viewModelName = 'MyRecurringScheduleDetails';
            this.window = window;

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
        public renderPayment(vm) {
            this.render('RecurringScheduleDetailsPayments', vm);
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

            let busy = this.asyncBusy();
            this.renderPayment({ IsLoading: true });

            let data: IRecurringOrderGetTemplatePaymentMethods = {
                id: this.id
            };
            this.recurringOrderService.getTemplatePaymentMethods(data)
                .then(result => {

                    let selected = this.viewModel.RecurringOrderTemplateLineItemViewModels[0].PaymentMethodId;
                    result.SavedCreditCards.forEach(payment => {
                        payment.IsSelected = payment.Id === selected;
                    });

                    console.log(result);

                    this.renderPayment(result);
                });


            busy.done();
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

        protected releaseBusyHandler(): void {
            if (this.busyHandler) {
                this.busyHandler.done();
                this.busyHandler = null;
            }
        }

        public saveRecurringOrderTemplate(actionContext: IControllerActionContext): Q.Promise<void> {

            let lineItemId = this.id;
            let paymentMethodId;
            let shippingAddressId;
            let billingAddressId;
            let nextOccurence;
            let frequencyName;
            let shippingProviderId;
            let shippingMethodName;
            let isAllValid = true;

            paymentMethodId = $(this.context.container).find('input[name=PaymentMethod]:checked').val();
            shippingAddressId = $(this.context.container).find('input[name=ShippingAddressId]:checked').val();
            billingAddressId = $(this.context.container).find('input[name=BillingAddressId]:checked').val();

            if (this.useShippingAddress()) {
                billingAddressId = shippingAddressId;
            }

            let element = <HTMLInputElement>$('#NextOcurrence')[0];
            let newDate = element.value;
            let isValid = this.nextOcurrenceIsValid(newDate);

            if (isValid) {
                nextOccurence = newDate;
            } else {
                isAllValid = false;
                console.error('Error: Invalid date while saving template');
                ErrorHandler.instance().outputErrorFromCode('InvalidDateSelected');
            }

            let frequency: any = $('#modifyFrequency').find(':selected')[0];
            frequencyName = frequency.value;
            if (frequencyName === '' || frequencyName === undefined) {
                isAllValid = false;
                console.error('Error: Invalid frequency while saving template');
                ErrorHandler.instance().outputErrorFromCode('InvalidFrequencySelected');
            }

            shippingProviderId = $('#ShippingProviderId').val();
            let elementShipping = $('#ShippingMethod').find('input[name=ShippingMethod]:checked');
            shippingMethodName = elementShipping.val();

            if (isAllValid) {

                this.busyHandler = this.asyncBusy({ elementContext: actionContext.elementContext });
                let updateTemplateLineItemParam: IRecurringOrderTemplateLineItemUpdateParam = {

                    nextOccurence: nextOccurence,
                    lineItemId: lineItemId,
                    billingAddressId: billingAddressId,
                    shippingAddressId: shippingAddressId,
                    paymentMethodId: paymentMethodId,
                    frequencyName: frequencyName,
                    shippingProviderId: shippingProviderId,
                    shippingMethodName: shippingMethodName
                };

                this.recurringOrderService.updateTemplateLineItem(updateTemplateLineItemParam)
                    .then(result => {

                        var templateLineItems = _.map(result.RecurringOrderTemplateViewModelList, (x: any) => {
                        return x.RecurringOrderTemplateLineItemViewModels;
                        });

                        var templateLineItemsList = _.reduce(templateLineItems,  function(a, b){ return a.concat(b); }, []);
                        var item = templateLineItemsList.filter(u => u.Id === this.id);

                        var vm = {
                            RecurringOrderTemplateLineItemViewModels: item
                        };

                        this.viewModel = vm;
                        this.reRenderPage(item[0]);
                    })
                    .fail((reason) => {
                        console.error(reason);
                        ErrorHandler.instance().outputErrorFromCode('UpdateTemplateError');
                    })
                    .fin(() => this.releaseBusyHandler());
            }
            return null;
        }

        public nextOcurrenceIsValid(value) {
            let newDate = this.convertDateToUTC(new Date(value));
            let today = this.convertDateToUTC(new Date(new Date().setHours(0, 0, 0, 0)));

            if (newDate > today) {
                return true;
            }
            return false;
        }

        private convertDateToUTC(date) {
            return new Date(date.getUTCFullYear(), date.getUTCMonth(), date.getUTCDate(),
            date.getUTCHours(), date.getUTCMinutes(), date.getUTCSeconds());
        }

        public cancelRecurringOrderTemplate(actionContext: IControllerActionContext) {

            let element = actionContext.elementContext[0];
            let url = element.dataset['scheduleurl'];

            this.window.location.href = url;
        }
    }
}
