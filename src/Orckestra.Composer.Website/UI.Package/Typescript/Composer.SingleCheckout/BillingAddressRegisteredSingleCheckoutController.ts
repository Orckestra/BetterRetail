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
                        if (this.billingAddressModified()) {
                            return self.checkoutService.updateCart([self.viewModelName])
                                .then(() => {
                                    this.Steps.EnteredOnce.Billing = true;
                                    return true;
                                });
                        } else {
                            this.Steps.EnteredOnce.Billing = true;
                            return true;
                        }
                    },
                    addBillingAddressToMyAddressBook() {
                        let formId = '#addNewBillingAddressForm';
                        let isValid = this.initializeParsey(formId);
                        if (!isValid) {
                            return Q.reject('Billing Address information is not valid');
                        }
                        let addressData = { ...this.Cart.Payment.BillingAddress };
                        addressData.AddressName = this.AddressName;

                        self.checkoutService.saveAddressToMyAccountAddressBook(addressData)
                            .then(address => {
                                return this.changeRegisteredBillingAddress(address.Id);
                            })
                            .fail((reason) => {
                                console.log(reason);
                                if (reason.Errors && _.find(reason.Errors, (e: any) => e.ErrorCode == 'NameAlreadyUsed')) {
                                    this.Errors.AddressNameAlreadyInUseError = true;
                                }
                                if (reason.Errors && _.find(reason.Errors, (e: any) => e.ErrorCode == 'InvalidPhoneFormat')) {
                                    this.Errors.InvalidPhoneFormatError = true;
                                }
                            });
                    }
                    ,
                    changeRegisteredBillingAddress(addressId) {
                        this.BillingAddress.AddressBookId = addressId;
                        this.Mode.AddingNewAddress = false;

                        if (!this.debouncechangeRegisteredBillingAddress) {
                            this.debouncechangeRegisteredBillingAddress = _.debounce(() => {
                                let controllersToUpdate = [self.viewModelName];
                                self.checkoutService.updateCart(controllersToUpdate)
                            }, 500);
                        }
                        this.debouncechangeRegisteredBillingAddress();
                    },

                }
            };

            this.checkoutService.VueCheckoutMixins.push(vueBillingAddressRegisteredMixin);
        }

        public getViewModelNameForUpdatePromise(): Q.Promise<any> {
            return Q.fcall(() => {
                var vueData = this.checkoutService.VueCheckout;
                if (!vueData.IsAuthenticated) {
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
