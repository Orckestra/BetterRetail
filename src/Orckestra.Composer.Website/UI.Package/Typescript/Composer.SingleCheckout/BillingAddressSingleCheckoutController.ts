///<reference path='../..//Typings/tsd.d.ts' />
///<reference path='./BaseSingleCheckoutController.ts' />

module Orckestra.Composer {
    'use strict';

    export class BillingAddressSingleCheckoutController extends Orckestra.Composer.BaseSingleCheckoutController {

        public initialize() {
            super.initialize();
            let self: BillingAddressSingleCheckoutController = this;
            self.viewModelName = 'BillingAddress';
            self.formSelector = '#billingAddressForm';

            let vueBillingAddressMixin = {

                created() {
                    this.billingAddressBeforeEdit = { ...this.Cart.Payment.BillingAddress };
                },
                mounted() {
                    this.Steps.Billing.EnteredOnce = this.FulfilledBillingAddress;
                },
                computed: {
                    FulfilledBillingAddress() {
                        return self.checkoutService.billingFulfilled(this.Cart, this.IsAuthenticated)
                            && this.Steps.ReviewCart.EnteredOnce;
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

                        if (this.IsPickUpMethodType) {
                            this.Cart.Payment.BillingAddress.UseShippingAddress = false;
                        }

                        return Q.resolve(true);
                    },
                    processBillingAddress(): Q.Promise<boolean> {

                        if (!this.billingAddressModified()) {
                            this.Steps.Billing.EnteredOnce = true;
                            return Q.resolve(true);
                        }

                        if (!this.BillingAddress.UseShippingAddress) {
                            let isValid = this.validateParsey(self.formSelector);
                            if (!isValid) {
                                return Q.reject('Billing Address information is not valid');
                            }

                            let postalCode = this.BillingAddress.PostalCode;
                            return this.changeBillingPostalCode(postalCode)
                                .then(() => this.updateBillingAddress());

                        } else {
                            return this.updateBillingAddress();
                        }
                    },
                    updateBillingAddress(): Q.Promise<boolean> {
                        this.Steps.Billing.Loading = true;
                        return self.checkoutService.updateCart([self.viewModelName])
                            .then(() => {
                                this.Steps.Billing.EnteredOnce = true;
                                this.Mode.AddingNewAddress = !this.BillingAddress.AddressBookId;
                                return true;
                            })
                            .finally(() =>  this.Steps.Billing.Loading = false);
                    },
                    changeBillingPostalCode(postalCode: any): Q.Promise<boolean> {
                        this.Errors.PostalCodeError = false;
                        if (this.billingAddressBeforeEdit.PostalCode === postalCode) {
                            return Q.resolve(true);
                        }

                        this.Mode.Loading = true;
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
                                this.Errors.PostalCodeError = true;
                                throw Error(reason);
                            })
                            .finally(() => this.Mode.Loading = false);
                    },

                    billingAddressModified() {
                        let keys = _.keys(this.BillingAddress).filter(k => k !== 'UseShippingAddress');
                        let dataToCompare = this.BillingAddress.UseShippingAddress ? this.ShippingAddress : this.BillingAddress;
                        return this.BillingAddress && _.some(keys, (key) => this.billingAddressBeforeEdit[key] !== dataToCompare[key]);
                    },

                    changeUseShippingAddress(event) {
                        let { checked } = event.target;
                        if (checked === this.billingAddressBeforeEdit.UseShippingAddress) {
                            this.Cart.Payment.BillingAddress = { ...this.billingAddressBeforeEdit };
                        } else {
                            if (!checked) {
                                this.clearBillingAddress();
                            } else {
                                this.copyShippingAddress();
                            }
                        }
                    },
                    copyShippingAddress() {
                        let { FirstName, LastName, CountryCode,
                            Line1, City, RegionCode, PostalCode,
                            PhoneNumber, AddressBookId, PhoneRegex } = this.Cart.ShippingAddress;
                        this.Cart.Payment.BillingAddress = {
                            FirstName, LastName, CountryCode,
                            Line1, City, RegionCode, PostalCode,
                            PhoneNumber, AddressBookId, PhoneRegex, UseShippingAddress: true
                        };
                    },
                    clearBillingAddress() {
                        this.Mode.AddingLine2Address = true;

                        let { FirstName, LastName, CountryCode,
                            PhoneRegex, PostalCodeRegexPattern, UseShippingAddress } = this.Cart.Payment.BillingAddress;
                        this.Cart.Payment.BillingAddress = {
                            FirstName: FirstName || this.Cart.Customer.FirstName,
                            LastName: LastName || this.Cart.Customer.LastName,
                            CountryCode, PhoneRegex, PostalCodeRegexPattern, UseShippingAddress, AddressBookId: null,
                            Line1: '', City: '', RegionCode: '', PostalCode: '', PhoneNumber: ''
                        };
                    },
                }
            };

            this.checkoutService.VueCheckoutMixins.push(vueBillingAddressMixin);
        }

        public getViewModelNameForUpdatePromise(): Q.Promise<any> {
            return Q.fcall(() => {
                var vueData = this.checkoutService.VueCheckout;
                if (vueData.IsAuthenticated) {
                    return;
                }
                let isValid = vueData.validateParsey(this.formSelector);
                if (!isValid) {
                    console.log('Billing Address information is not valid');
                    return Q.reject('Billing Address information is not valid');
                }

                if (vueData.billingAddressModified()) {
                    return this.viewModelName;
                }
            });
        }

        public getUpdateModelPromise(): Q.Promise<any> {
            return Q.fcall(() => {
                let { Payment } = this.checkoutService.VueCheckout.Cart;
                return {[this.viewModelName]: JSON.stringify(Payment.BillingAddress)};
            });
        }

    }
}
