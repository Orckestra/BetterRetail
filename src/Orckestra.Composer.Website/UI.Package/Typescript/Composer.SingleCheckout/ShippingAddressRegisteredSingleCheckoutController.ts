///<reference path='../..//Typings/tsd.d.ts' />
///<reference path='./BaseSingleCheckoutController.ts' />
///<reference path='./ShippingAddressSingleCheckoutController.ts' />
///<reference path='../Composer.Cart/CheckoutShippingAddressRegistered/ShippingAddressRegisteredService.ts' />

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
                            return self.checkoutService.updateCart([self.viewModelName])
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
                                            if(!reason.Errors) return;

                                            reason.Errors.forEach((e: any) => {
                                                switch (e.ErrorCode) {
                                                    case 'NameAlreadyUsed':
                                                        this.Errors.AddressNameAlreadyInUseError = true; break;
                                                    case 'InvalidPhoneFormat':
                                                        this.Errors.InvalidPhoneFormatError = true; break;
                                                }
                                            })
                                        });
                                } else {
                                    //
                                }
                            });
                    },

                    changeRegisteredShippingAddress(addressId, addingNewAddressPromise = null) {

                        this.SelectedShippingAddressId = addressId;
                        this.Mode.AddingNewAddress = false;
                        if (!this.debounceChangeRegisteredShippingAddress) {
                            this.debounceChangeRegisteredShippingAddress = _.debounce((addingNewAddressPromise) => {
                                //WHEN CHANGING SHIPPING, WE ALSO NEED UPDATE BILLING
                                let controllersToUpdate = [self.viewModelName, 'BillingAddressRegistered'];
                                self.checkoutService.updateCart(controllersToUpdate).then((response: any) => {
                                    let { Cart } = response;
                                    this.Cart.ShippingAddress = Cart.ShippingAddress;
                                    this.Cart.Payment.BillingAddress = Cart.Payment.BillingAddress;
                                    if (addingNewAddressPromise) {
                                        this.Steps.Shipping.EnteredOnce = true;
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
                        this.debounceChangeRegisteredShippingAddress(addingNewAddressPromise);
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
                let vm = {};
                let selectedAddressId = this.checkoutService.VueCheckout.SelectedShippingAddressId;
                vm[this.viewModelName] = JSON.stringify({ ShippingAddressId: selectedAddressId });

                return vm;
            });
        }
    }
}
