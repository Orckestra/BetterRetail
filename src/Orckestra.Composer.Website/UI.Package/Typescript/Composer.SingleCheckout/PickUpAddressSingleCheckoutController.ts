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
            let commonOptions = this.getCommonStoreLocatorVueConfig(self);

            let vueStoreLocatorMixin = {
                data: {
                    ...commonOptions.data,
                    initialized: false
                },
                mounted() {
                    self.VueStoreList = this;

                    if (this.IsPickUpMethodType) {
                        this.initializeMap();
                    }
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
                        this.Errors.StoreNotSelectedError = false;
                    },
                    showStoreLocatorLocationError() {
                        this.Errors.StoreLocatorLocationError = true;
                    },
                    processPickUpAddress(): Q.Promise<any> {
                        let controllersToUpdate = [self.viewModelName];

                        if (!this.Cart.PickUpLocationId) {
                            this.Errors.StoreNotSelectedError = true;
                            return Q.reject('PickUpLocationId is not specified');
                        }

                        if (!this.pickUpAddressModified()) {
                            return Q.resolve(true);
                        }

                        return self.checkoutService.updateCart(controllersToUpdate)
                            .then(() => {
                                this.Steps.Shipping.EnteredOnce = true;
                                return true;
                            });
                    },
                    preparePickUpAddress() {
                        this.pickUpLocationIdBeforeEdit = this.Cart.PickUpLocationId;
                    },
                    pickUpAddressModified() {
                        return this.pickUpLocationIdBeforeEdit !== this.Cart.PickUpLocationId;
                    },
                    initializeMap() {
                        if (this.initialized) { return; }
                        this.initialized = true;
                        commonOptions.mounted();
                    },
                    onSelectPickUpMethod() {
                        this.initializeMap();
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
