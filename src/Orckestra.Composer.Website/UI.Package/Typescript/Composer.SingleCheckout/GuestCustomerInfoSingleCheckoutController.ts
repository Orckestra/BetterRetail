///<reference path='../..//Typings/tsd.d.ts' />
///<reference path='./BaseSingleCheckoutController.ts' />

module Orckestra.Composer {
    'use strict';

    export class GuestCustomerInfoSingleCheckoutController extends Orckestra.Composer.BaseSingleCheckoutController {

        public initialize() {
            let self: GuestCustomerInfoSingleCheckoutController = this;
            self.viewModelName = 'GuestCustomerInfo';
            self.formSelector = '#editCustomerForms';

            super.initialize();

            let vueUserMixin = {
                created() {
                    this.customerBeforeEdit = { ...this.Cart.Customer };
                },
                mounted() {
                    this.initializeParsey(self.formSelector);
                },
                computed: {
                    FulfilledCustomer() {
                        return self.checkoutService.customerFulfilled(this.Cart);
                    }
                },
                methods: {
                    prepareCustomer() {
                        this.initializeParsey(self.formSelector);
                    },
                    processCustomer() {
                        var processCustomer: Q.Deferred<boolean> = Q.defer<boolean>();
                        let isValid = this.validateParsey(self.formSelector);

                        if (isValid) {

                            if (this.isCustomerModified()) {
                                self.checkoutService.updateCart([self.viewModelName]).then(result => {
                                    processCustomer.resolve(true);
                                });
                            } else {
                                processCustomer.resolve(true);
                            }
                        } else {
                            processCustomer.resolve(false);
                        }

                        return processCustomer.promise;
                    },

                    isCustomerModified() {
                        let keys = _.keys(this.Cart.Customer);
                        let isModified = _.some(keys, (key) => this.customerBeforeEdit[key] != this.Cart.Customer[key]);
                        return isModified;
                    }
                }
            };

            this.checkoutService.VueCheckoutMixins.push(vueUserMixin);
        }

        public getViewModelNameForUpdatePromise(): Q.Promise<any> {
            return Q.fcall(() => {
                var vueData = this.checkoutService.VueCheckout;
                let isValid = vueData.validateParsey(this.formSelector);
                if (!isValid) {
                    return Q.reject('User information is not valid');
                }

                if (vueData.isCustomerModified()) {
                    return this.viewModelName;
                }
            });
        }

        public getUpdateModelPromise(): Q.Promise<any> {
            return Q.fcall(() => {
                let vm = {};
                let vueCustomerData = this.checkoutService.VueCheckout.Cart.Customer;
                vm[this.viewModelName] = JSON.stringify(vueCustomerData);
                return vm;
            });
        }
    }
}
