///<reference path='../..//Typings/tsd.d.ts' />
///<reference path='./BaseSingleCheckoutController.ts' />

module Orckestra.Composer {
    'use strict';

    export class BillingAddressRegisteredSingleCheckoutController extends Orckestra.Composer.BaseSingleCheckoutController {

        public initialize() {
            super.initialize();
            let self: BillingAddressRegisteredSingleCheckoutController = this;
            self.viewModelName = 'BillingAddressRegistered';

            let vueBillingAddressRegisteredMixin = {
                data: {
                    AddingNewAddressMode: false,
                    ComplementaryAddressAddState: false,
                    PostalCodeError: false
                },
                computed: {

                },
                methods: {
                    processBillingAddressRegistered() {
                        if (this.BillingAddress.UseShippingAddress && !this.FulfilledBillingAddress) {
                            return self.checkoutService.updateCart([self.viewModelName])
                            .then(() => {
                                  return true;
                            });
                        } else {
                            return true; //TODO: ...
                        }
                    }
                    ,
                    changeRegisteredBillingAddress(addressId, addingNewAddressPromise = null) {
                        this.BillingAddress.AddressBookId = addressId;
                        this.AddingNewAddressMode = false;
                        if (!this.debouncechangeRegisteredBillingAddress) {
                            this.debouncechangeRegisteredBillingAddress = _.debounce((addingNewAddressPromise) => {
                                let controlersToUpdate = [self.viewModelName];
                                self.checkoutService.updateCart(controlersToUpdate).then(() => {
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
