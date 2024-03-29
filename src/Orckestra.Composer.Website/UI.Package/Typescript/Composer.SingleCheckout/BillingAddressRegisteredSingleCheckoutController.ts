///<reference path='../..//Typings/tsd.d.ts' />
///<reference path='./BillingAddressSingleCheckoutController.ts' />

module Orckestra.Composer {
    'use strict';

    export class BillingAddressRegisteredSingleCheckoutController extends Orckestra.Composer.BaseSingleCheckoutController {

        public initialize() {
            super.initialize();
            let self: BillingAddressRegisteredSingleCheckoutController = this;
            self.viewModelName = 'BillingAddressRegistered';
            self.formSelector = '#addNewBillingAddressForm';

            let vueBillingAddressRegisteredMixin = {
                data: {
                    SelectedBillingAddressId: null,
                    deleteBillingAddressModal: null,
                },
                methods: {
                    prepareBillingAddressRegistered() {
                        if (self.checkoutService.isAddressBookIdEmpty(this.Cart.Payment.BillingAddress.AddressBookId)
                        && !self.checkoutService.isAddressBookIdEmpty(this.SelectedBillingAddressId)) {
                            this.Cart.Payment.BillingAddress.AddressBookId = this.SelectedBillingAddressId;
                            return self.checkoutService.updateCart([self.viewModelName]);
                        }
                        if (this.IsPickUpMethodType) {
                            this.Cart.Payment.BillingAddress.UseShippingAddress = false;
                        }
                        return Q.resolve(true);
                    },
                    processBillingAddressRegistered():Q.Promise<boolean> {
                        if (!this.billingAddressModified()) {
                            return Q.resolve(true);
                        }

                        return self.checkoutService.updateCart([self.viewModelName])
                            .then(() => true);
                    },
                    addNewBillingAddress() {
                        this.Mode.AddingNewAddress = true;
                        this.clearBillingAddress();
                        this.initializeParsey(self.formSelector);
                    },

                    addBillingAddressToMyAddressBook() {
                        let isValid = this.validateParsey(self.formSelector);
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
                                if (reason.Errors && _.find(reason.Errors, (e: any) => e.ErrorCode === 'NameAlreadyUsed')) {
                                    this.Errors.AddressNameAlreadyInUseError = true;
                                }
                                if (reason.Errors && _.find(reason.Errors, (e: any) => e.ErrorCode === 'InvalidPhoneFormat')) {
                                    this.Errors.InvalidPhoneFormatError = true;
                                }
                            });
                    },
                    changeRegisteredBillingAddress(addressId) {
                        this.BillingAddress.AddressBookId = addressId;
                        this.Mode.AddingNewAddress = false;
                        this.Mode.EditingAddress = false;

                        if (!this.debouncechangeRegisteredBillingAddress) {
                            this.debouncechangeRegisteredBillingAddress = _.debounce(() => {
                                let controllersToUpdate = [self.viewModelName];
                                self.checkoutService.updateCart(controllersToUpdate);
                            }, 500);
                        }
                        this.debouncechangeRegisteredBillingAddress();
                    },
                    deleteBillingAddressConfirm(event: JQueryEventObject) {
                        this.Modal.deleteAddressModal.openModal(event);
                    },
                    updateEditedBillingAddress() {
                        let isValid = this.validateParsey('#editAddressForm');
                        if (!isValid) {
                            return Q.reject('Address information is not valid');
                        }
                        this.Mode.Loading = true;
                        this.EditingAddress.AddressName = this.AddressName;

                        self.checkoutService.updateAddressInMyAccountAddressBook(this.EditingAddress)
                        .then(() =>  {
                            this.Mode.EditingAddress = false;
                            
                            if(this.Cart.Payment.BillingAddress.AddressBookId === this.EditingAddress.Id) {
                                var isMatch = AddressUtils.isEquals(this.Cart.Payment.BillingAddress, this.EditingAddress);
                                if(!isMatch) {
                                    return this.changeRegisteredBillingAddress(this.EditingAddress.Id);
                                }
                            }
                        })
                        .fail(reason => this.handleAddressErrors(reason))
                        .fin(() => this.Mode.Loading = false)
                    }
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
                }
            });
        }


        public getUpdateModelPromise(): Q.Promise<any> {
            return Q.fcall(() => {
                let { Payment } = this.checkoutService.VueCheckout.Cart;
                let { AddressBookId, UseShippingAddress } = Payment.BillingAddress;

                let value = JSON.stringify({
                    UseShippingAddress,
                    BillingAddressId: AddressBookId
                });

                return {[this.viewModelName]: value};
            });
        }

    }
}
