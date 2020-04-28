///<reference path='../..//Typings/tsd.d.ts' />
///<reference path='./BaseSingleCheckoutController.ts' />

module Orckestra.Composer {
    'use strict';

    export class OrderSummarySingleCheckoutController extends Orckestra.Composer.BaseSingleCheckoutController {

        public initialize() {
            super.initialize();
            let self: OrderSummarySingleCheckoutController = this;

            let vueCompleteCheckoutMixin = {

                computed: {
                    OrderCanBePlaced() {
                        return !this.Mode.Loading
                            && !this.Mode.CompleteCheckoutLoading
                            && !this.CartEmpty
                            && this.FulfilledShipping
                            && this.FulfilledBillingAddress
                            && this.Payment;
                    }
                },
                methods: {
                    processCompleteCheckout(): Q.Promise<any> {
                        this.Mode.CompleteCheckoutLoading = true;
                        return self.checkoutService.collectViewModelNamesForUpdateCart().
                            then(viewModels => {
                                return self.checkoutService.updateCart(viewModels);
                            })
                            .then(() => this.submitPayment())
                            .then(() => self.checkoutService.completeCheckout())
                            .fail(reason => {
                                console.error('An error occurred while completing the checkout.', reason);
                                ErrorHandler.instance().outputErrorFromCode('CompleteCheckoutFailed');
                            })
                            .finally(() => this.Mode.CompleteCheckoutLoading = false);
                    }
                }
            };

            this.checkoutService.VueCheckoutMixins.push(vueCompleteCheckoutMixin);
        }

    }
}
