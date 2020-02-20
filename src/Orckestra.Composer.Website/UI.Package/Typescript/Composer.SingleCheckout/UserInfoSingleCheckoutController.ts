///<reference path='../..//Typings/tsd.d.ts' />
///<reference path='./BaseSingleCheckoutController.ts' />

module Orckestra.Composer {
    'use strict';

    export class UserInfoSingleCheckoutController extends Orckestra.Composer.BaseSingleCheckoutController {


        public initialize() {
            super.initialize();
            this.registerSubscriptions();

            var userInformationMixin = {
                computed: {
                    FulfilledCustomer () {
                        return this.Cart.Customer.FirstName &&
                            this.Cart.Customer.LastName &&
                            this.Cart.Customer.Email;
                    }
                },
                methods: {
                    validateCustomer(e) {
                         this.parsleyInit = $('#editCustomerForms').parsley();
                         this.parsleyInit.validate();
                        return this.parsleyInit.isValid();
                        //TODO: this.isValidForUpdate();
                    }
                }
            };

            this.checkoutService.SingleCheckoutMixins.push(userInformationMixin);
        }

    }
}
