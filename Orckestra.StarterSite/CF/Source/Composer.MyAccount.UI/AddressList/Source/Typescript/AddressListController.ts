///<reference path='../../../../Composer.UI/Source/Typings/tsd.d.ts' />
///<reference path='../../../../Composer.UI/Source/TypeScript/Mvc/Controller.ts' />
///<reference path='../../../../Composer.UI/Source/TypeScript/Mvc/IControllerActionContext.ts' />
///<reference path='../../../../Composer.UI/Source/Typescript/jqueryPlugins/ISerializeObjectJqueryPlugin.ts' />
///<reference path='../../../Common/Source/Typescript/CustomerService.ts' />
///<reference path='../../../Common/Source/Typescript/MyAccountEvents.ts' />
///<reference path='../../../Common/Source/Typescript/MyAccountStatus.ts' />
///<reference path='../../../MyAccount/Source/Typescript/MyAccountController.ts' />
///<reference path='../../../../Composer.UI/Source/Typescript/UI/UIModal.ts' />

module Orckestra.Composer {

    //TODO refactor modal : create a generic modal service
    export class AddressListController extends Orckestra.Composer.MyAccountController {

        private deleteModalElementSelector: string = '#confirmationModal';
        private uiModal: UIModal;

        protected customerService: ICustomerService = new CustomerService(new CustomerRepository());

        public initialize() {

            super.initialize();

            this.uiModal = new UIModal(window, this.deleteModalElementSelector, this.deleteAddress, this);

            this.registerSubscriptions();
        }

        protected registerSubscriptions() {

            this.eventHub.subscribe(MyAccountEvents[MyAccountEvents.AddressDeleted], e => this.onAddressDeleted(e));
        }

        private onAddressDeleted(e: IEventInformation) {

            let result = e.data,
                $container = result.$container;

            $container.remove();
        }

        /**
         * Requires the element in action context to have a data-address-id.
         */
        public setDefaultAddress(actionContext: IControllerActionContext): void {

            let $addressListItem: JQuery = $(actionContext.elementContext).closest('[data-address-id]'),
                addressId = $addressListItem.data('address-id'),
                busy = this.asyncBusy({elementContext: actionContext.elementContext, containerContext: $addressListItem});

            this.customerService.setDefaultAddress(addressId.toString(), '')
                .then(result => location.reload(), reason => console.error(reason))
                .fin(() => busy.done())
                .done();
        }

        /**
         * Requires the element in action context to have a data-address-id.
         */
        public deleteAddress(event: JQueryEventObject): void {
            let element = $(event.target),
                $addressListItem = element.closest('[data-address-id]'),
                addressId = $addressListItem.data('address-id'),
                busy = this.asyncBusy({
                    elementContext: element as JQuery<HTMLElement>,
                    containerContext: $addressListItem as JQuery<HTMLElement>
                });

            this.customerService.deleteAddress(addressId, '')
                .then(result => this.onDeleteAddressFulfilled(result, $addressListItem as JQuery<HTMLElement>), reason => console.error(reason))
                .fin(() => busy.done())
                .done();
        }

        private onDeleteAddressFulfilled(result: any, $addressListItem: JQuery): void {

            let data = {
                result: result,
                $container: $addressListItem
            };

            this.eventHub.publish(MyAccountEvents[MyAccountEvents.AddressDeleted], {data: data});
        }

        public deleteAddressConfirm(actionContext: IControllerActionContext) {
            this.uiModal.openModal(actionContext.event);
        }
    }
}
