///<reference path='../..//Typings/tsd.d.ts' />
///<reference path='./BaseSingleCheckoutController.ts' />
///<reference path='./ShippingAddressSingleCheckoutController.ts' />

module Orckestra.Composer {
    'use strict';

    export class ShippingAddressRegisteredSingleCheckoutController extends Orckestra.Composer.BaseSingleCheckoutController {

        public initialize() {
            super.initialize();
            let self: ShippingAddressRegisteredSingleCheckoutController = this;
            self.viewModelName = 'ShippingAddressRegistered';
            self.formSelector = '#addNewAddressForm';

            let vueShippingAddressRegisteredMixin = {
                data: {
                    SelectedShippingAddressId: null,
                    AddressName: null,
                },
                methods: {
                    processShippingAddressRegistered() {
                        if (this.shippingAddressModified()) {
                            //WHEN CHANGING SHIPPING, WE ALSO NEED UPDATE BILLING
                            let controllersToUpdate = [self.viewModelName, 'BillingAddressRegistered'];
                            this.prepareBillingAddress();
                            return self.checkoutService.updateCart(controllersToUpdate)
                                .then(() => {
                                    this.Steps.Shipping.EnteredOnce = true;
                                    return true;
                                });
                        } else {
                            this.Steps.Shipping.EnteredOnce = true;
                            return true;
                        }
                    },

                    addNewAddressMode() {
                        this.Mode.AddingNewAddress = true;
                        this.adressBeforeEdit = {};
                        this.AddressName = null;
                        this.SelectedShippingAddressId = undefined;
                        this.clearShippingAddress();
                        this.initializeParsey(self.formSelector);
                    },

                    addShippingAddressToMyAddressBook() {
                        let isValid = this.validateParsey(self.formSelector);
                        if (!isValid) {
                            return Q.reject('Shipping Address information is not valid');
                        }

                        let postalCode = this.Cart.ShippingAddress.PostalCode;
                        this.changePostalCode(postalCode)
                            .then(success => {
                                if (success) {
                                    let addressData = { ...this.Cart.ShippingAddress };
                                    addressData.AddressName = this.AddressName;

                                    self.checkoutService.saveAddressToMyAccountAddressBook(addressData)
                                        .then(address => {
                                            address.RegionName = this.ShippingAddress.RegionName;
                                            this.changeRegisteredShippingAddress(address.Id);
                                        })
                                        .fail((reason) => {
                                            console.log(reason);
                                            if (!reason.Errors) { return; }

                                            reason.Errors.forEach((e: any) => {
                                                switch (e.ErrorCode) {
                                                    case 'NameAlreadyUsed':
                                                        this.Errors.AddressNameAlreadyInUseError = true; break;
                                                    case 'InvalidPhoneFormat':
                                                        this.Errors.InvalidPhoneFormatError = true; break;
                                                }
                                            });
                                        });
                                } else {
                                    //
                                }
                            });
                    },

                    changeRegisteredShippingAddress(addressId) {

                        this.SelectedShippingAddressId = addressId;
                        this.Mode.AddingNewAddress = false;
                        if (!this.debounceChangeRegisteredShippingAddress) {
                            this.debounceChangeRegisteredShippingAddress = _.debounce(() => {
                                //WHEN CHANGING SHIPPING, WE ALSO NEED UPDATE BILLING
                                let controllersToUpdate = [self.viewModelName, 'BillingAddressRegistered'];
                                self.checkoutService.updateCart(controllersToUpdate)
                                    .fail((reason) => {
                                        console.log(reason);
                                    });
                            }, 500);
                        }
                        this.debounceChangeRegisteredShippingAddress();
                    },
                    deleteShippingAddressConfirm(event: JQueryEventObject) {
                        this.Modal.deleteAddressModal.openModal(event);
                    },
                }
            };

            this.checkoutService.VueCheckoutMixins.push(vueShippingAddressRegisteredMixin);
        }

        public getViewModelNameForUpdatePromise(): Q.Promise<any> {
            return Q.fcall(() => {
                var vueData = this.checkoutService.VueCheckout;
                if (!vueData.IsAuthenticated) {
                    return;
                }

                if (vueData.shippingAddressModified()) {
                    return this.viewModelName;
                }
            });
        }

        public getUpdateModelPromise(): Q.Promise<any> {
            return Q.fcall(() => {
                let selectedAddressId = this.checkoutService.VueCheckout.SelectedShippingAddressId;
                return {[this.viewModelName]: JSON.stringify({ ShippingAddressId: selectedAddressId })};
            });
        }
    }
}
