///<reference path='../..//Typings/tsd.d.ts' />
///<reference path='./BaseSingleCheckoutController.ts' />
///<reference path='../Composer.Store/StoreLocator/StoreLocatorController.ts' />

module Orckestra.Composer {
    'use strict';

    export class PickUpAddressSingleCheckoutController extends StoreLocatorController implements IBaseSingleCheckoutController {
        public viewModelName: string = 'PickUpAddress';
        protected checkoutService: ISingleCheckoutService;

        protected registerStoreLocatorVue() {
            let self: PickUpAddressSingleCheckoutController = this;
            this.checkoutService = SingleCheckoutService.getInstance();

            let vueStoreLocatorMixin = {

                methods: {
                    processPickUpAddress(): Q.Promise<boolean> {
                        let controllersToUpdate = [self.viewModelName];

                        if (!this.Cart.PickUpLocationId) {
                            this.Errors.StoreNotSelectedError = true;
                            return Q.reject('PickUpLocationId is not specified');
                        }

                        if (!this.pickUpAddressModified()) {
                            return Q.resolve(true);
                        }

                        return self.checkoutService.updateCart(controllersToUpdate)
                            .then(() => true);
                    },
                    preparePickUpAddress() {
                        this.pickUpLocationIdBeforeEdit = this.Cart.PickUpLocationId;
                    },
                    pickUpAddressModified() {
                        return this.pickUpLocationIdBeforeEdit !== this.Cart.PickUpLocationId;
                    }
                }
            };

            this.checkoutService.VueCheckoutMixins.push(vueStoreLocatorMixin);
            this.checkoutService.registerController(this);
        }

        public getValidationPromise(): Q.Promise<boolean> {
            return Q.resolve(true);
        }

        public getUpdateModelPromise(): Q.Promise<any> {
            return Q.fcall(() => {
                let { PickUpLocationId } = this.checkoutService.VueCheckout.Cart;
                return { [this.viewModelName]: JSON.stringify({ PickUpLocationId }) };
            });
        }

        public getViewModelNameForUpdatePromise(): Q.Promise<any> {
            let vueData = this.checkoutService.VueCheckout;
            if (vueData.pickUpAddressModified()) {
                return Q.resolve(this.viewModelName);
            }
        }

    }
}
