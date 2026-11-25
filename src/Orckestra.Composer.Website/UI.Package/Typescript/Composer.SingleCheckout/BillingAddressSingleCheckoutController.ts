///<reference path='../..//Typings/tsd.d.ts' />
///<reference path='./BaseSingleCheckoutController.ts' />

namespace Orckestra.Composer {
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
                computed: {
                    FulfilledBillingAddress() {
                        return self.checkoutService.billingFulfilled(this.Cart, this.IsAuthenticated);
                    },
                    BillingAddress() {
                        return this.Cart.Payment.BillingAddress;
                    }
                },

                methods: {
                    prepareBilling() {
                        return this.IsAuthenticated ? this.prepareBillingAddressRegistered() : this.prepareBillingAddress();
                    },
                    prepareBillingAddress(): Promise<boolean> {
                        this.fillAddressNames(this.Cart.Payment.BillingAddress);
                        this.fixAddressNullValues(this.Cart.Payment.BillingAddress);

                        if (this.IsPickUpMethodType) {
                            this.Cart.Payment.BillingAddress.UseShippingAddress = false;
                        }

                        return Promise.resolve(true);
                    },
                    processBillingAddress(): Promise<boolean> {

                        if (!this.billingAddressModified()) {
                            return Promise.resolve(true);
                        }

                        if (!this.BillingAddress.UseShippingAddress) {
                            let isValid = this.validateParsey(self.formSelector);
                            if (!isValid) {
                                return Promise.reject('Billing Address information is not valid');
                            }

                            let postalCode = this.BillingAddress.PostalCode;
                            return this.changeBillingPostalCode(postalCode)
                                .then(() => this.updateBillingAddress());

                        } else {
                            return this.updateBillingAddress();
                        }
                    },
                    updateBillingAddress(): Promise<boolean> {
                        this.Steps.Billing.Loading = true;
                        return self.checkoutService.updateCart([self.viewModelName])
                            .then(() => {
                                this.Mode.AddingNewAddress = !this.BillingAddress.AddressBookId;
                                return true;
                            })
                            .finally(() =>  this.Steps.Billing.Loading = false);
                    },
                    changeBillingPostalCode(postalCode: any): Promise<boolean> {
                        this.Errors.PostalCodeError = false;
                        if (this.billingAddressBeforeEdit.PostalCode === postalCode) {
                            return Promise.resolve(true);
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
                            .catch(reason => {
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

        public getViewModelNameForUpdatePromise(): Promise<any> {
            return Promise.resolve().then(() => {
                var vueData = this.checkoutService.VueCheckout;
                if (vueData.IsAuthenticated) {
                    return;
                }
                let isValid = vueData.validateParsey(this.formSelector);
                if (!isValid) {
                    console.log('Billing Address information is not valid');
                    return Promise.reject('Billing Address information is not valid');
                }

                if (vueData.billingAddressModified()) {
                    return this.viewModelName;
                }
            });
        }

        public getUpdateModelPromise(): Promise<any> {
            return Promise.resolve().then(() => {
                let { Payment } = this.checkoutService.VueCheckout.Cart;
                return {[this.viewModelName]: JSON.stringify(Payment.BillingAddress)};
            });
        }

    }
}
