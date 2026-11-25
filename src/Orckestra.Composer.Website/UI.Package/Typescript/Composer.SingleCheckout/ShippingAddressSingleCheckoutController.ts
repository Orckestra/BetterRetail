///<reference path='../..//Typings/tsd.d.ts' />
///<reference path='./BaseSingleCheckoutController.ts' />

namespace Orckestra.Composer {
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
                    this.initializeParsey(self.formSelector);
                },
                computed: {
                },
                methods: {
                    prepareShippingAddress() {
                        if (!this.Cart.ShippingAddress.FirstName && !this.Cart.ShippingAddress.LastName) {
                            this.Cart.ShippingAddress.FirstName = this.Customer.FirstName;
                            this.Cart.ShippingAddress.LastName = this.Customer.LastName;
                        }

                        this.Mode.AddingLine2Address = !this.Cart.ShippingAddress.Line2;
                        this.Mode.AddingNewAddress = false;
                        this.initializeParsey(self.formSelector);
                    },
                    processShippingAddress(): Promise<boolean> {
                        let isValid = this.validateParsey(self.formSelector);
                        if (!isValid) {
                            return Promise.reject('Shipping Address information is not valid');
                        }

                        if (!this.shippingAddressModified()) {
                           return Promise.resolve(true);
                        }

                        let postalCode = this.Cart.ShippingAddress.PostalCode;
                        return this.changePostalCode(postalCode).then(success => {
                            if (!success) return false;

                            //WHEN CHANGING SHIPPING, WE ALSO NEED UPDATE BILLING
                            let needUpdateBilling = this.Cart.Payment.BillingAddress.UseShippingAddress;
                            let controllersToUpdate = needUpdateBilling ? [self.viewModelName, 'BillingAddress'] : [self.viewModelName];
                            if(needUpdateBilling) this.fixAddressNullValues(this.Cart.Payment.BillingAddress);

                            return self.checkoutService.updateCart(controllersToUpdate)
                                .catch(reason => {
                                    console.log(reason);
                                    return false;
                                });
                        });
                    },
                    recalculateShippingFee() {

                        let formId = !this.IsAuthenticated ? '#addressForm' : '#addNewAddressForm';
                        let isValid = this.validateParsey(formId);
                        if (isValid) {
                            this.changePostalCode(this.Cart.ShippingAddress.PostalCode);
                        }

                    },
                    changePostalCode(postalCode: any) {
                        var processPostalCode: Deferred<boolean> = new Deferred<boolean>();

                        this.Errors.PostalCodeError = false;
                        if (this.adressBeforeEdit.PostalCode !== postalCode) {
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
                                .catch(reason => {
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

                    shippingAddressModified() {
                        let keys = _.keys(this.Cart.ShippingAddress);
                        let isModified = _.some(keys, (key) => this.adressBeforeEdit[key] !== this.Cart.ShippingAddress[key]);
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

        public getViewModelNameForUpdatePromise(): Promise<any> {
            return Promise.resolve().then(() => {
                var vueData = this.checkoutService.VueCheckout;
                let isValid = vueData.validateParsey(this.formSelector);
                if (!isValid) {
                    return Promise.reject('Shipping Address information is not valid');
                }

                if (vueData.shippingAddressModified()) {
                    return this.viewModelName;
                }
            });
        }

        public getUpdateModelPromise(): Promise<any> {
            return Promise.resolve().then(() => {
                let vueAddressData = this.checkoutService.VueCheckout.Cart.ShippingAddress;
                return {[this.viewModelName]: JSON.stringify(vueAddressData)};
            });
        }

    }
}
