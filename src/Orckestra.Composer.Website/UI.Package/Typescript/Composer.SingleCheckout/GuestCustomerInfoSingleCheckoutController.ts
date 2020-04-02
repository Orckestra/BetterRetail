///<reference path='../..//Typings/tsd.d.ts' />
///<reference path='./BaseSingleCheckoutController.ts' />

module Orckestra.Composer {
    'use strict';

    export class GuestCustomerInfoSingleCheckoutController extends Orckestra.Composer.BaseSingleCheckoutController {

        public initialize() {
            var self: GuestCustomerInfoSingleCheckoutController = this;
            self.viewModelName = 'GuestCustomerInfo';

            super.initialize();
            this.registerSubscriptions();

            var vueUserMixin = {
                created() {
                    this.customerBeforeEdit = { ...this.Cart.Customer }
                },
                computed: {
                    FulfilledCustomer() {
                        var fulfilled = this.Cart.Customer.FirstName &&
                            this.Cart.Customer.LastName &&
                            this.Cart.Customer.Email;
 
                        return fulfilled ? true : false;
                    }
                },
                methods: {
                    processCustomer() {
                        var processCustomer: Q.Deferred<boolean> = Q.defer<boolean>();
                        let isValid = this.initializeParsey('#editCustomerForms');

                        if (isValid) {

                            if (this.isCustomerModified()) {
                                self.checkoutService.updateCart([self.viewModelName]).then(result => {
                                    this.Cart.Customer = result.Cart.Customer;
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
                        var keys = _.keys(this.Cart.Customer);
                        var isModified = _.some(keys, (key) => this.customerBeforeEdit[key] != this.Cart.Customer[key]);
                        return isModified;
                    }
                }
            };

            this.checkoutService.VueCheckoutMixins.push(vueUserMixin);
        }

        public getViewModelNameForUpdatePromise(): Q.Promise<any> {
            return Q.fcall(() => {
                var vueData = this.checkoutService.VueCheckout;
                let isValid = vueData.initializeParsey('#editCustomerForms');
                if(!isValid) {
                    return Q.reject('User information is not valid');
                }

                if (vueData.isCustomerModified()) {
                    return this.viewModelName;
                };
            });
        }

        public getUpdateModelPromise(): Q.Promise<any> {
            return Q.fcall(() => {
                var vm = {};
                var vueCustomerData = this.checkoutService.VueCheckout.Cart.Customer;
                vm[this.viewModelName] = JSON.stringify(vueCustomerData);
                return vm;
            });
        }
    }
}
