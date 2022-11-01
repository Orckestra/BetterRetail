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
                    processShippingAddressRegistered(): Q.Promise<boolean> {
                        if(!this.shippingAddressModified()) {
                            return Q.resolve(true);
                        }

                        //WHEN CHANGING SHIPPING, WE ALSO NEED UPDATE BILLING
                        let controllersToUpdate = [self.viewModelName, 'BillingAddressRegistered'];
                        this.prepareBillingAddress();
                        return self.checkoutService.updateCart(controllersToUpdate)
                            .then(() => true);
                    },

                    addNewAddressMode() {
                        this.Mode.AddingNewAddress = true;
                        this.Mode.EditingAddress = false;
                        this.adressBeforeEdit = {};
                        this.AddressName = null;
                        this.SelectedShippingAddressId = undefined;
                        this.Cart.ShippingAddress = this.getClearShippingAddress();
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
                                            this.handleAddressErrors(reason);
                                        });
                                } else {
                                    //
                                }
                            });
                    },
                    changeRegisteredShippingAddress(addressId) {

                        this.SelectedShippingAddressId = addressId;
                        this.Mode.AddingNewAddress = false;
                        this.Mode.EditingAddress = false;
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
                    updateEditedShippingAddress() {
                        let isValid = this.validateParsey('#editAddressForm');
                        if (!isValid) {
                            return Q.reject('Address information is not valid');
                        }
                        this.Mode.Loading = true;
                        this.EditingAddress.AddressName = this.AddressName;

                        self.checkoutService.updateAddressInMyAccountAddressBook(this.EditingAddress)
                        .then(() =>  {
                            this.Mode.EditingAddress = false;
                            
                            if(this.Cart.ShippingAddress.AddressBookId === this.EditingAddress.Id) {
                                var isMatch = AddressUtils.isEquals(this.Cart.ShippingAddress, this.EditingAddress);
                                if(!isMatch) {
                                    return this.changeRegisteredShippingAddress(this.EditingAddress.Id);
                                }
                            }
                        })
                        .fail(reason => this.handleAddressErrors(reason))
                        .fin(() => this.Mode.Loading = false)
                    }
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
