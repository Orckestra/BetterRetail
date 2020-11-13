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
                            && this.IsValidOrder
                    },
                    IsValidOrder() {
                        return !this.CartEmpty
                        && this.Cart.InvalidLineItemCount <= 0 
                        && this.FulfilledShipping
                        && this.FulfilledBillingAddress
                        && this.IsTimeSlotReserved
                        && !this.TimeSlotReservationExpired
                        && this.Payment;
                    },
                    IsShippingEstimatedOrSelected() {
                        return this.OrderSummary.IsShippingEstimatedOrSelected && !this.IsPickUpMethodType
                    }
                },
                methods: {
                    processCompleteCheckout(): Q.Promise<any> {
                        this.Mode.CompleteCheckoutLoading = true;
                        self.fulfillmentService.invalidateCache();
                        return self.fulfillmentService.getFreshSelectedFulfillment()
                            .then(fulfillment => this.SelectedFulfillment = fulfillment)
                            .then(() => self.checkoutService.collectViewModelNamesForUpdateCart())
                            .then(viewModels => {
                                return self.checkoutService.updateCart(viewModels);
                            })
                            .then(() => {
                                if (this.IsValidOrder) { 
                                    return Q.resolve(true)
                                } else return Q.reject("Order can't be placed")
                            })
                            .then(() => this.submitPayment())
                            .then(() => self.checkoutService.completeCheckout())
                            .fail(reason => {
                                console.error('An error occurred while completing the checkout.', reason);
                                ErrorHandler.instance().outputErrorFromCode('CompleteCheckoutFailed');
                            })
                            .finally(() => {
                                this.Mode.CompleteCheckoutLoading = false;
                                self.fulfillmentService.invalidateCache();// it is important to clear cache at the end, so confirmation page will load fresh data
                            });
                    }
                }
            };

            this.checkoutService.VueCheckoutMixins.push(vueCompleteCheckoutMixin);
        }

    }
}
