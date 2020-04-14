///<reference path='../..//Typings/tsd.d.ts' />
///<reference path='./BaseSingleCheckoutController.ts' />
///<reference path='../Composer.Store/StoreLocator/StoreLocatorController.ts' />

module Orckestra.Composer {
    'use strict';

    export class PickUpAddressSingleCheckoutController extends Orckestra.Composer.StoreLocatorController implements Orckestra.Composer.IBaseSingleCheckoutController {
        public viewModelName: string = 'PickUpAddress';
        protected checkoutService: ISingleCheckoutService;

        protected registerStoreLocatorVue() {
            let self: PickUpAddressSingleCheckoutController = this;
            this.checkoutService = SingleCheckoutService.getInstance();
            let commonOptions =  this.getCommonStoreLocatorVueConfig(self);

            let vueStoreLocatorMixin = {
                data: {
                    ...commonOptions.data
                },
                mounted() {
                    self.VueStoreList = this;
                    commonOptions.mounted();
                },
                computed: {
                    SelectedStore() {
                        return this.Cart.PickUpLocationId && this.Stores.find(store => store.Id === this.Cart.PickUpLocationId);
                    },
                    SelectedStoreId() {
                        return this.Cart.PickUpLocationId;
                    }
                },
                methods: {
                    ...commonOptions.methods,
                    selectPickupStore(store: any) {
                        this.Cart.PickUpLocationId = store.Id;
                    },
                    showStoreLocatorLocationError() {
                        this.Errors.StoreLocatorLocationError = true
                    },
                    processPickUpAddress(): Q.Promise<any> {
                        let controllersToUpdate = [self.viewModelName];
                        return self.checkoutService.updateCart(controllersToUpdate)
                            .then(() => {
                                this.Steps.EnteredOnce.Shipping = true;
                                return true;
                            });
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
                let vm = {};
                let { PickUpLocationId } = this.checkoutService.VueCheckout.Cart;
                vm[this.viewModelName] = JSON.stringify({ PickUpLocationId });
                return vm;
            });
        }

        public getViewModelNameForUpdatePromise(): Q.Promise<any> {
            return Q.resolve(null);
        }
    }
}
