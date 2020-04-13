///<reference path='../..//Typings/tsd.d.ts' />
///<reference path='./BillingAddressSingleCheckoutController.ts' />

module Orckestra.Composer {
    'use strict';

    export class BillingAddressRegisteredSingleCheckoutController extends Orckestra.Composer.BillingAddressSingleCheckoutController {

        public initialize() {
            super.initialize();
            let self: BillingAddressRegisteredSingleCheckoutController = this;
            self.viewModelName = 'BillingAddressRegistered';

            let vueBillingAddressRegisteredMixin = {
 
                computed: {

                },
                methods: {
                    processBillingAddressRegistered() {
                        if (this.BillingAddress.UseShippingAddress && !this.FulfilledBillingAddress) {
                            return self.checkoutService.updateCart([self.viewModelName])
                                .then(() => {
                                    this.Steps.EnteredOnce.Billing = true;
                                    return true;
                                });
                        } else {
                            return true; //TODO: ...
                        }
                    }
                    ,
                    changeRegisteredBillingAddress(addressId, addingNewAddressPromise = null) {
                        this.BillingAddress.AddressBookId = addressId;
                        this.Mode.AddingNewAddress = false;
                        if (!this.debouncechangeRegisteredBillingAddress) {
                            this.debouncechangeRegisteredBillingAddress = _.debounce((addingNewAddressPromise) => {
                                let controllersToUpdate = [self.viewModelName];
                                self.checkoutService.updateCart(controllersToUpdate).then((result) => {
                                    let { Cart } = result;
                                    this.Cart.Payment.BillingAddress = Cart.Payment.BillingAddress;
                                    if (addingNewAddressPromise) {
                                        addingNewAddressPromise.resolve(true);
                                    }
                                }).fail((reason) => {
                                    console.log(reason);

                                    if (addingNewAddressPromise) {
                                        addingNewAddressPromise.resolve(false);
                                    }
                                });
                            }, 500);
                        }
                        this.debouncechangeRegisteredBillingAddress(addingNewAddressPromise);
                    },

                }
            };

            this.checkoutService.VueCheckoutMixins.push(vueBillingAddressRegisteredMixin);
        }

        public getViewModelNameForUpdatePromise(): Q.Promise<any> {
            return Q.fcall(() => {
                var vueData = this.checkoutService.VueCheckout;
                if(!vueData.IsAuthenticated) {
                    return;
                }

                if (vueData.billingAddressModified()) {
                    return this.viewModelName;
                };
            });
        }


        public getUpdateModelPromise(): Q.Promise<any> {
            return Q.fcall(() => {
                let vm = {};
                let { Payment } = this.checkoutService.VueCheckout.Cart;
                let { AddressBookId, UseShippingAddress } = Payment.BillingAddress;

                vm[this.viewModelName] = JSON.stringify({
                    UseShippingAddress,
                    BillingAddressId: AddressBookId
                });

                return vm;
            });
        }

    }
}
