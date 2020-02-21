///<reference path='../..//Typings/tsd.d.ts' />
///<reference path='./BaseSingleCheckoutController.ts' />

module Orckestra.Composer {
    'use strict';

    export class UserInfoSingleCheckoutController extends Orckestra.Composer.BaseSingleCheckoutController {
       
        public initialize() {
            var self: UserInfoSingleCheckoutController  = this;
            self.viewModelName = 'UserInfo';

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
                        // TODO: Investigate how to make this work: var isValid = this.isValidForUpdate();

                        if (isValid) {
                            self.checkoutService.updateCart().then(result => {
                                processCustomer.resolve(true);
                            });
                        } else  {
                            processCustomer.resolve(false);
                        };

                        return processCustomer.promise;

                    }
                }
            };

            this.checkoutService.VueCheckoutMixins.push(vueUserMixin);
        }

        
        public getUpdateModelPromise(): Q.Promise<any> {

            return Q.fcall(() => {
                var vm = {};
                var vueCustomerData = this.checkoutService.VueCheckout.Cart.Customer;
                var formData = {
                    'FirstName': vueCustomerData.FirstName,
                    'LastName': vueCustomerData.LastName,
                    'Email': vueCustomerData.Email
                };

                vm['GuestCustomerInfo'] = JSON.stringify(formData);
                return vm;
            });
        }

    }
}
