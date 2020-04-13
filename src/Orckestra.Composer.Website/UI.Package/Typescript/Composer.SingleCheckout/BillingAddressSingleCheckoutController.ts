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
                    this.Steps.EnteredOnce.Billing = this.FulfilledBillingAddress;
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

                        return !!(fulfilled && this.Steps.EnteredOnce.ReviewCart);
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

                    updateBillingAddress(): Q.Promise<boolean> {
                        return self.checkoutService.updateCart([self.viewModelName])
                            .then(() => {
                                this.Steps.EnteredOnce.Billing = true;
                                this.Mode.AddingNewAddress = !this.BillingAddress.AddressBookId;
                                return true;
                            });
                    },

                    processBillingAddress(): Q.Promise<boolean> {
                        
                        if (!this.billingAddressModified())
                            return Q.resolve(true);

                        if (!this.BillingAddress.UseShippingAddress) {
                            let isValid = this.initializeParsey(self.formSelector);
                            if (!isValid) {
                                return Q.reject('Billing Address information is not valid');
                            }

                            let postalCode = this.BillingAddress.PostalCode;
                            return this.changeBillingPostalCode(postalCode)
                                .then(() => this.updateBillingAddress());
                                
                        } else {
                            return this.updateBillingAddress()
                        }
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
                        let keys = _.keys(this.BillingAddress);
                        let dataToCompare = this.BillingAddress.UseShippingAddress ? this.ShippingAddress: this.BillingAddress;
                        dataToCompare.UseShippingAddress = this.BillingAddress.UseShippingAddress;
                        return this.BillingAddress && _.some(keys, (key) => this.billingAddressBeforeEdit[key] != dataToCompare[key]);
                    },

                    addNewBillingAddressMode() {
                        this.Mode.AddingNewAddress = true;
                        this.clearBillingAddress();
                    },

                    changeUseShippingAddress(event) {
                        let { checked } = event.target;
                        if (!checked) {
                            this.clearBillingAddress();
                        }
                    },
                    clearBillingAddress() {
                        this.Mode.AddingLine2Address = false;

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

        public getViewModelNameForUpdatePromise(): Q.Promise<any> {
            return Q.fcall(() => {
                var vueData = this.checkoutService.VueCheckout;
                if(vueData.IsAuthenticated) {
                    return;
                }
                let isValid = vueData.initializeParsey(this.formSelector);
                if(!isValid) {
                    console.log('Billing Address information is not valid')
                    return Q.reject('Billing Address information is not valid');
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
                vm[this.viewModelName] = JSON.stringify(Payment.BillingAddress);
                return vm;
            });
        }

    }
}
