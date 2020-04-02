///<reference path='../..//Typings/tsd.d.ts' />
///<reference path='./BaseSingleCheckoutController.ts' />

module Orckestra.Composer {
    'use strict';

    export class BillingAddressSingleCheckoutController extends Orckestra.Composer.BaseSingleCheckoutController {

        public initialize() {
            super.initialize();
            let self: BillingAddressSingleCheckoutController = this;
            self.viewModelName = 'BillingAddress';

            let vueBillingAddressMixin = {
                data: {
                     //THIS PROPERTY IS NEEDED FOR DETERMING IF SHIPPING STEP WAS ENTERED AT LEAST ONCE 
                    BillingEnteredOnce: false,
                    AddingNewAddressMode: false,
                    ComplementaryAddressAddState: false,
                    PostalCodeError: false
                },
                created() {
                    this.billingAdressBeforeEdit = { ...this.Cart.Payment.BillingAddress };
                },
                mounted() {
                },
                computed: {
                    FulfilledBillingAddress() {

                        let fulfilled = 
                            this.BillingAddress.FirstName &&
                            this.BillingAddress.LastName &&
                            this.BillingAddress.Line1 &&
                            this.BillingAddress.City &&
                            this.BillingAddress.RegionCode &&
                            this.BillingAddress.PostalCode &&
                            this.BillingAddress.PhoneNumber;

                        if (this.IsAuthenticated) {
                            fulfilled = fulfilled || this.BillingAddress.AddressBookId
                        }

                        return fulfilled && this.BillingEnteredOnce ? true : false;
                    },

                    BillingAddress() {
                        return this.Cart.Payment.BillingAddress;
                    }
                },
                methods: {
                    prepareBillingAddress(): Q.Promise<boolean> {
                        if (!this.BillingAddress.FirstName && !this.BillingAddress.LastName) {
                            this.BillingAddress.FirstName = this.Customer.FirstName;
                            this.BillingAddress.LastName = this.Customer.LastName;
                        }
                 
                        let billingAddressParams = ['FirstName', 'LastName', 'Line1', 'City', 'RegionCode', 'PostalCode', 'PhoneNumber'];

                        billingAddressParams.forEach(param => {
                            if (this.BillingAddress[param] === null) {
                                this.BillingAddress[param] = '';
                            }
                        });
                        return Q.resolve(true);
                    },

                    updateBillingAddress(): Q.Promise<boolean> {
                        return self.checkoutService.updateCart([self.viewModelName])
                            .then(result => {
                                let { Cart } = result;
                                this.Cart.Payment.BillingAddress = Cart.Payment.BillingAddress;
                                this.BillingEnteredOnce = true;
                                this.AddingNewAddressMode = !this.BillingAddress.AddressBookId;
                                return true;
                            });
                    },

                    processBillingAddress(): Q.Promise<boolean> {
                        
                        if (!this.billingAddressModified())
                            return Q.resolve(true);

                        if (!this.BillingAddress.UseShippingAddress) {
                            let isValid = this.initializeParsey('#billingAddressForm');
                            if (!isValid) {
                                return Q.reject('Form not valid');
                            }

                            let postalCode = this.BillingAddress.PostalCode;
                            return this.changeBillingPostalCode(postalCode)
                                .then(() => this.updateBillingAddress())
                        } else {
                            return this.updateBillingAddress()
                        }
                    },

                    changeBillingPostalCode(postalCode: any): Q.Promise<boolean> {
                        this.PostalCodeError = false;
                        if (this.billingAdressBeforeEdit.PostalCode === postalCode) {
                            return Q.resolve(true);
                        }

                        this.IsLoading = true;
                        return self.checkoutService.updateBillingPostalCode(postalCode)
                            .then((cart: any) => {
                                let { PostalCode, RegionCode, RegionName } = cart.Payment.BillingAddress;
                                this.Cart.Payment.BillingAddress = {
                                    ...this.BillingAddress,
                                    PostalCode,
                                    RegionCode,
                                    RegionName
                                };
                                return true;
                            })
                            .fail(reason => {
                                this.PostalCodeError = true;
                                throw Error(reason);
                            })
                            .finally(() => this.IsLoading = false);
                    },

                    billingAddressModified() {
                        let keys = _.keys(this.BillingAddress);
                        return this.BillingAddress && _.some(keys, (key) => this.billingAdressBeforeEdit[key] != this.BillingAddress[key]);
                    },

                    addNewBillingAddressMode() {
                        this.AddingNewAddressMode = true;
                        this.clearBillingAddress();
                    },

                    changeUseShippingAddress(event) {
                        let { checked } = event.target;
                        if (!checked) {
                            this.clearBillingAddress();
                        }
                    },
                    clearBillingAddress() {
                        this.ComplementaryAddressAddState = false;

                        let { FirstName, LastName, CountryCode, UseShippingAddress } = this.Cart.Payment.BillingAddress;
                        this.Cart.Payment.BillingAddress = {
                            FirstName: FirstName || this.Cart.Customer.FirstName,
                            LastName: LastName || this.Cart.Customer.LastName,
                            CountryCode, UseShippingAddress, AddressBookId: null,
                            Line1: '', City: '', RegionCode: '', PostalCode: '', PhoneNumber: ''
                        };
                    },
                }
            };

            this.checkoutService.VueCheckoutMixins.push(vueBillingAddressMixin);
        }

        public getUpdateModelPromise(): Q.Promise<any> {
            return Q.fcall(() => {
                let vm = {};
                let { Payment } = this.checkoutService.VueCheckout.Cart;
                vm['BillingAddress'] = JSON.stringify(Payment.BillingAddress);
                return vm;
            });
        }

    }
}
