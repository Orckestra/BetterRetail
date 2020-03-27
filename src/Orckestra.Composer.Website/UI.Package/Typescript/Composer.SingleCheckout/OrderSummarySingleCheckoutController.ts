///<reference path='../..//Typings/tsd.d.ts' />
///<reference path='./BaseSingleCheckoutController.ts' />

module Orckestra.Composer {
    'use strict';

    export class OrderSummarySingleCheckoutController extends Orckestra.Composer.BaseSingleCheckoutController {

        public initialize() {
            super.initialize();
            let self: OrderSummarySingleCheckoutController = this;

            let vueCompleteCheckoutMixin = {
                data: {
                    IsCompleteCheckoutLoading: false,
                },
                computed: {
                    OrderCanBePlaced() {
                        return !this.IsLoading
                            && !this.IsCompleteCheckoutLoading
                            && this.Payment
                            && this.FulfilledBillingAddress;
                            //TODO: && this.FullfilledPaymentInformation;
                    }
                },
                methods: {
                    processCompleteCheckout(): Q.Promise<any> {
                        this.IsCompleteCheckoutLoading = true;
                        return this.processPayment()
                            .then(() => self.checkoutService.completeCheckout())
                            .finally(() => this.IsCompleteCheckoutLoading = false);
                    },
                }
            };

            this.checkoutService.VueCheckoutMixins.push(vueCompleteCheckoutMixin);
        }

    }
}
