///<reference path='../..//Typings/tsd.d.ts' />
///<reference path='./BaseSingleCheckoutController.ts' />

module Orckestra.Composer {
    'use strict';

    export class GuestCustomerInfoSingleCheckoutController extends Orckestra.Composer.BaseSingleCheckoutController {
       
        public initialize() {
            var self: GuestCustomerInfoSingleCheckoutController  = this;
            self.viewModelName = 'GuestCustomerInfo';

            super.initialize();
            this.registerSubscriptions();

            var vueUserMixin = {

                computed: {
                    FulfilledCustomer() {
                        return this.Cart.Customer.FirstName &&
                            this.Cart.Customer.LastName &&
                            this.Cart.Customer.Email;
                    }
                },
                methods: {
                    processCustomer()  {
                         var  processCustomer: Q.Deferred<boolean> = Q.defer<boolean>();
                         this.parsleyInit = $('#editCustomerForms').parsley();
                         this.parsleyInit.validate();
                         let isValid = this.parsleyInit.isValid();
                        // TODO: Investigate how to make this work: var isValid = self.isValidForUpdate();
                        
                        if (isValid) {
                            if (self.isModified()) {
                                self.checkoutService.updateCart().then(result => {
                                    processCustomer.resolve(true);
                                });
                            } else {
                                processCustomer.resolve(true);
                            }
                        } else {
                            processCustomer.resolve(false);
                        };

                        return processCustomer.promise;

                    }
                }
            };

            this.checkoutService.VueCheckoutMixins.push(vueUserMixin);
        }


        protected isModified(): boolean {
            var formData = this.getSerializedForm();
            var keys = _.keys(formData);
            var vueCustomerData = this.checkoutService.VueCheckout.Cart.Customer;
            var initialCustomer = this.checkoutService.VueCheckout.Cart.Customer;// TODO - how to hande modifications
            //var isModified = _.some(keys, (key) => initialCustomer[key] != vueCustomerData[key]);
            var isModified = true;
            return isModified;
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
