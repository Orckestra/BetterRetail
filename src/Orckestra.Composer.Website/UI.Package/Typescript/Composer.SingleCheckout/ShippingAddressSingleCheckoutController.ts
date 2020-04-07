///<reference path='../..//Typings/tsd.d.ts' />
///<reference path='./BaseSingleCheckoutController.ts' />

module Orckestra.Composer {
    'use strict';

    export class ShippingAddressSingleCheckoutController extends Orckestra.Composer.BaseSingleCheckoutController {

        public initialize() {
            super.initialize();
            var self: ShippingAddressSingleCheckoutController = this;
            self.viewModelName = 'ShippingAddress';
            self.formSelector = '#addressForm';

            let vueShippingAddressMixin = {
                created() {
                    this.adressBeforeEdit = { ...this.Cart.ShippingAddress };
                },
                mounted() {

                },
                computed: {
                },
                methods: {
                    processShippingAddress() {
                        var processShippingAddress: Q.Deferred<boolean> = Q.defer<boolean>();
                        let isValid = this.initializeParsey(self.formSelector);
                        if (isValid) {
                            if (this.addressModified()) {
                                let postalCode = this.Cart.ShippingAddress.PostalCode;
                                this.changePostalCode(postalCode).then(success => {
                                    if (success) {
                                        //WHEN CHANGING SHIPPING, WE ALSO NEED UPDATE BILLING
                                        let controlersToUpdate = [self.viewModelName, 'BillingAddress'];
                                        this.prepareBillingAddress()
                                            .then(() => self.checkoutService.updateCart(controlersToUpdate))
                                            .then(() => {
                                                this.Steps.EnteredOnce.Shipping = true;
                                                self.eventHub.publish("cartBillingAddressUpdated", { data: this });
                                                processShippingAddress.resolve(true);
                                            })
                                            .fail(reason => {
                                                console.log(reason);
                                                processShippingAddress.resolve(false);
                                            });
                                    } else {
                                        processShippingAddress.resolve(false);
                                    }
                                })

                            } else {
                                processShippingAddress.resolve(true);
                            }
                        } else {
                            processShippingAddress.resolve(false);
                        }

                        return processShippingAddress.promise;
                    },
                    recalculateShippingFee() {

                        let formId = !this.IsAuthenticated ? '#addressForm' : '#addNewAddressForm';
                        let isValid = this.initializeParsey(formId);
                        if (isValid) {
                            this.changePostalCode(this.Cart.ShippingAddress.PostalCode);
                        }

                    },
                    changePostalCode(postalCode: any) {
                        var processPostalCode: Q.Deferred<boolean> = Q.defer<boolean>();

                        this.Errors.PostalCodeError = false;
                        if (this.adressBeforeEdit.PostalCode != postalCode) {
                            this.Mode.Loading = true;
                            self.checkoutService.updatePostalCode(postalCode).then((cart: any) => {
                                this.adressBeforeEdit = { ...this.Cart.ShippingAddress };
                                this.Cart = {
                                    ...this.Cart,
                                    ShippingAddress: {
                                        ...this.Cart.ShippingAddress,
                                        PostalCode: cart.ShippingAddress.PostalCode,
                                        RegionCode: cart.ShippingAddress.RegionCode,
                                        RegionName: cart.ShippingAddress.RegionName
                                    },
                                    OrderSummary: cart.OrderSummary
                                };
                                processPostalCode.resolve(true);
                            })
                                .fail(reason => {
                                    console.log(reason);
                                    this.Errors.PostalCodeError = true;
                                    processPostalCode.resolve(false);
                                })
                                .finally(() => this.Mode.Loading = false);
                        } else {
                            processPostalCode.resolve(true);
                        }
                        return processPostalCode.promise;
                    },

                    addressModified() {
                        var keys = _.keys(this.Cart.ShippingAddress);
                        var isModified = _.some(keys, (key) => this.adressBeforeEdit[key] != this.Cart.ShippingAddress[key]);
                        return isModified;
                    },
                    adjustPostalCode() {
                        this.Cart.ShippingAddress.PostalCode = this.Cart.ShippingAddress.PostalCode.toUpperCase();
                        if (this.BillingAddress && this.BillingAddress.PostalCode) {
                            this.BillingAddress.PostalCode = this.BillingAddress.PostalCode.toUpperCase();
                        }
                    }
                }
            };

            this.checkoutService.VueCheckoutMixins.push(vueShippingAddressMixin);
        }

        public getViewModelNameForUpdatePromise(): Q.Promise<any> {
            return Q.fcall(() => {
                var vueData = this.checkoutService.VueCheckout;
                let isValid = vueData.initializeParsey(this.formSelector);
                if (!isValid) {
                    return Q.reject('Address information is not valid');
                }

                if (vueData.addressModified()) {
                    return this.viewModelName;
                };
            });
        }

        public getUpdateModelPromise(): Q.Promise<any> {
            return Q.fcall(() => {
                let vm = {};
                let vueAddressData = this.checkoutService.VueCheckout.Cart.ShippingAddress;
                vm[this.viewModelName] = JSON.stringify(vueAddressData);
                return vm;
            });
        }

    }
}
