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
                            this.Cart.Customer.Email &&
                            !this.IsLoading;
                        //TODO: Parsey doesn't work as expected && (this.parsleyInit ? this.parsleyInit.isValid(): true);

                        return fulfilled;
                    }
                },
                methods: {
                    processCustomer() {
                        var processCustomer: Q.Deferred<boolean> = Q.defer<boolean>();
                        this.parsleyInit = $('#editCustomerForms').parsley();
                        this.parsleyInit.validate();
                        let isValid = this.parsleyInit.isValid();
                        // TODO: Investigate how to make this work: var isValid = self.isValidForUpdate();

                        if (isValid) {
                            if (this.isCustomerModified()) {
                                self.checkoutService.updateCart().then(result => {
                                    this.customerBeforeEdit = { ...this.Cart.Customer };
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
                        var formData = self.getSerializedForm();
                        var keys = _.keys(formData);
                        var isModified = _.some(keys, (key) => this.customerBeforeEdit[key] != this.Cart.Customer[key]);
                        return isModified;
                    }
                }
            };

            this.checkoutService.VueCheckoutMixins.push(vueUserMixin);
        }

        public getUpdateModelPromise(): Q.Promise<any> {
            return Q.fcall(() => {
                var vm = {};
                var vueCustomerData = this.checkoutService.VueCheckout.Cart.Customer;
                var formData = this.getSerializedForm();

                var keys = _.keys(formData);
                _.each(keys, key => {
                    if (vueCustomerData.hasOwnProperty(key)) {
                        formData[key] = vueCustomerData[key];
                    }
                });
                vm[this.viewModelName] = JSON.stringify(formData);
                return vm;
            });
        }
    }
}
