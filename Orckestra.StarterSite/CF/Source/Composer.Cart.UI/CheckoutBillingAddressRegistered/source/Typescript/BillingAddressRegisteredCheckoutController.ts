///<reference path='../../../../Composer.UI/Source/Typings/tsd.d.ts' />
///<reference path='../../../../Composer.MyAccount.UI/Common/Source/Typescript/CustomerService.ts' />
///<reference path='../../../../Composer.MyAccount.UI/Common/Source/Typescript/MyAccountEvents.ts' />
///<reference path='../../../../Composer.MyAccount.UI/Common/Source/Typescript/MyAccountStatus.ts' />
///<reference path='../../../CheckoutCommon/source/Typescript/BaseCheckoutController.ts' />
///<reference path='./BillingAddressRegisteredCheckoutService.ts' />
///<reference path='../../../../Composer.UI/Source/Typescript/UI/UIModal.ts' />

module Orckestra.Composer {
    'use strict';

    export class BillingAddressRegisteredCheckoutController extends Orckestra.Composer.BaseCheckoutController {

        protected debounceChangeBillingMethod: Function = _.debounce(this.changeBillingAddressImpl, 500, {'leading': true});
        protected modalElementSelector: string = '#billingConfirmationModal';
        private uiModal: UIModal;

        protected customerService: ICustomerService = new CustomerService(new CustomerRepository());
        protected billingAddressRegisteredCheckoutService: BillingAddressRegisteredCheckoutService =
            new BillingAddressRegisteredCheckoutService(this.customerService);

        public initialize() {
            super.initialize();

            this.viewModelName = 'BillingAddressRegistered';

            this.uiModal = new UIModal(window, this.modalElementSelector, this.deleteAddress, this);
        }

        protected registerSubscriptions() {

            this.eventHub.subscribe(`${this.viewModelName}Rendered`, e => this.onRendered(e));
            this.eventHub.subscribe(MyAccountEvents[MyAccountEvents.AddressDeleted], e => this.onAddressDeleted(e));
        }

        public renderData(checkoutContext: ICheckoutContext): Q.Promise<void> {

            return Q.fcall(() => {
                if (checkoutContext.authenticationViewModel.IsAuthenticated) {
                    this.renderAuthenticated(checkoutContext);
                } else {
                    this.renderUnauthenticated(checkoutContext);
                }
            });
        }

        protected renderUnauthenticated(checkoutContext: ICheckoutContext) {

            this.unregisterController();
            this.render(this.viewModelName, checkoutContext.authenticationViewModel);
        }

        protected renderAuthenticated(checkoutContext: ICheckoutContext): Q.Promise<any> {

            this.registerSubscriptions();
            this.render(this.viewModelName, {IsAuthenticated: true});

            return this.billingAddressRegisteredCheckoutService.getBillingAddresses(checkoutContext.cartViewModel)
                .then((billingAdressesVm) => {
                    this.render(this.viewModelName, checkoutContext.cartViewModel);
                    this.render('BillingRegisteredAddresses', billingAdressesVm);
                })
                .fail(reason => this.handleError(reason))
                .fin(() => this.eventHub.publish(`${this.viewModelName}Rendered`, checkoutContext.cartViewModel));
        }

        protected onRendered(e: IEventInformation) {

            this.formInstances = this.registerFormsForValidation($('#RegisteredBillingAddress', this.context.container));

            let selectedBillingAddressId: string = $(this.context.container).find('input[name=BillingAddressId]:checked').val() as string;

            if (!selectedBillingAddressId) {
                return;
            }

            this.checkoutService.getCart()
                .then(cart => {
                    if (selectedBillingAddressId !== cart.Payment.BillingAddress.AddressBookId) {
                        this.debounceChangeBillingMethod();
                    }
                })
                .fail(reason => this.handleError(reason));
        }

        protected setSelectedBillingAddress() {

            let selectedBillingAddressId: string = $(this.context.container).find('input[name=BillingAddressId]:checked').val() as string;

            if (!selectedBillingAddressId) {
                return;
            }

            this.checkoutService.getCart()
                .done(cart => {
                    if (selectedBillingAddressId !== cart.Payment.BillingAddress.AddressBookId) {
                        this.debounceChangeBillingMethod();
                    }
                });
        }

        public changeBillingAddress(actionContext: IControllerActionContext) {

            this.debounceChangeBillingMethod();
        }

        protected changeBillingAddressImpl() {

            this.checkoutService.updateCart()
                .done(result => {

                    if (result.HasErrors) {
                        throw new Error('The updated cart contains errors');
                    }

                }, reason => this.handleError(reason));
        }

        /**
         * Requires the element in action context to have a data-address-id.
         */
        protected deleteAddress(event: JQueryEventObject): Q.Promise<void> {

            let element = $(event.target),
                $addressListItem = element.closest('[data-address-id]'),
                addressId = $addressListItem.data('address-id'),
                busy = this.asyncBusy({
                    elementContext: element as JQuery<HTMLElement>,
                    containerContext: $addressListItem as JQuery<HTMLElement>
                });

            return this.customerService.deleteAddress(addressId, '')
                .then(result => {
                    this.eventHub.publish(MyAccountEvents[MyAccountEvents.AddressDeleted], {data: addressId});
                })
                .fail(() => this.renderFailedForm(MyAccountStatus[MyAccountStatus.AjaxFailed]))
                .fin(() => busy.done());
        }

        public deleteAddressConfirm(actionContext: IControllerActionContext) {
            this.uiModal.openModal(actionContext.event);
        }

        private onAddressDeleted(e: IEventInformation) {
            let addressId = e.data,
                $addressListItem = $(this.context.container).find('[data-address-id=' + addressId + ']');

            $addressListItem.remove();
        }

        private useShippingAddress(): Boolean {
            let useShippingAddress = $(this.context.container).find('input[name=UseShippingAddress]:checked').val() === 'true';
            return useShippingAddress;
        }

        private getVisibleForms(): JQuery {
            let visibleForms = $('form', this.context.container).not('form:has(.hide)');
            return visibleForms;
        }

        public changeUseShippingAddress() {

            this.setBillingAddressFormVisibility();
            this.setBillingAddressFormValidation();
            this.setSelectedBillingAddress();
        }

        private setBillingAddressFormVisibility() {

            let useShippingAddress: Boolean = this.useShippingAddress();
            if (useShippingAddress) {
                $('#BillingAddressContent').addClass('hide');
            } else {
                $('#BillingAddressContent').removeClass('hide');
            }
        }

        private setBillingAddressFormValidation() {

            let useShippingAddress: Boolean = this.useShippingAddress(),
                isValidationEnabled: Boolean = this.isBillingAddressFormValidationEnabled();

            if (useShippingAddress) {
                if (isValidationEnabled) {
                    this.disableBillingAddressFormValidation();
                }
            } else {
                if (!isValidationEnabled) {
                    this.enableBillingAddressFormValidation();
                }
            }
        }

        private isBillingAddressFormValidationEnabled(): Boolean {

            return _.some(this.formInstances, (formInstance: any) => {
                return this.isBillingAddressFormInstance(formInstance);
            });
        }

        private disableBillingAddressFormValidation() {

            let formInstance = _.find(this.formInstances, (formInstance: any) => {
                return this.isBillingAddressFormInstance(formInstance);
            });

            formInstance.destroy();

            _.remove(this.formInstances, (formInstance: any) => {
                return this.isBillingAddressFormInstance(formInstance);
            });
        }

        private isBillingAddressFormInstance(formInstance: any): boolean {
            let isBillingAddressFormInstance = formInstance.$element.is('form#BillingAddressRegistered');
            return isBillingAddressFormInstance;
        }

        private enableBillingAddressFormValidation() {

            this.formInstances = this.formInstances.concat(this.registerFormsForValidation($('form#BillingAddressRegistered')));
        }

        private renderFailedForm(status: string) {
            //TODO
        }

        protected handleError(reason: any) {

            this.eventHub.publish('cartUpdatingFailed', null);

            console.error('The user changed the billing address, but an error occured when updating the preferred billing address', reason);
        }
    }
}
