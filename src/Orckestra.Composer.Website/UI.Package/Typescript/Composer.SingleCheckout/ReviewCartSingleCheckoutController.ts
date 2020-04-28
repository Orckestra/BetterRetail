///<reference path='../..//Typings/tsd.d.ts' />
///<reference path='./BaseSingleCheckoutController.ts' />

module Orckestra.Composer {
    'use strict';

    export class ReviewCartSingleCheckoutController extends Orckestra.Composer.BaseSingleCheckoutController {

        public initialize() {
            let self: ReviewCartSingleCheckoutController = this;
            self.viewModelName = 'ReviewCart';

            super.initialize();

            var vueReviewCartMixin = {
                mounted() {
                    this.Steps.ReviewCart.EnteredOnce = this.FulfilledCart;
                },
                computed: {
                    FulfilledCart() {
                        return !!(this.Steps.Shipping.EnteredOnce);
                    },
                },
                methods: {
                    processCart() {
                        this.Steps.ReviewCart.EnteredOnce = true;
                        return true;
                    }
                    ,
                    DecrementDisabled(item) {
                        return item.Quantity < 2 || this.Mode.Loading;
                    },
                    IncrementDisabled(item) {
                        return item.Quantity >= 99 || this.Mode.Loading;
                    },
                    updateItemQuantity(id, action: string = '') {
                        let item = _.find(this.Cart.LineItemDetailViewModels, (i: any) => i.Id === id);
                        if (action === 'increment') {
                            item.Quantity++;
                        }

                        if (action === 'decrement') {
                            item.Quantity--;
                        }

                        if (!this.debounceUpdateItem) {
                            this.debounceUpdateItem = _.debounce(id => {
                                this.Mode.Loading = true;
                                let itemToUpdate = _.find(this.Cart.LineItemDetailViewModels, (i: any) => i.Id === id);
                                self.checkoutService.updateCartItem(itemToUpdate.Id,
                                    itemToUpdate.Quantity,
                                    itemToUpdate.ProductId,
                                    itemToUpdate.RecurringOrderFrequencyName === '' ? null : itemToUpdate.RecurringOrderFrequencyName,
                                    itemToUpdate.RecurringOrderProgramName)
                                    .then(cart => {
                                        if (cart) {
                                            this.Cart = cart;
                                        }
                                    })
                                    .finally(() => {
                                        this.Mode.Loading = false;
                                    });
                            }, 400);
                        }

                        this.debounceUpdateItem(id);
                    },

                    removeCartItem(index) {
                        var item = this.Cart.LineItemDetailViewModels[index];
                        this.Mode.Loading = true;
                        self.checkoutService.removeCartItem(item.Id, item.ProductId)
                            .then(cart => {
                                if (cart) {
                                    this.Cart = cart;
                                }
                            })
                            .finally(() => {
                                this.Mode.Loading = false;
                            });

                        this.Cart.LineItemDetailViewModels.splice(index, 1);
                    }
                }
            };

            this.checkoutService.VueCheckoutMixins.push(vueReviewCartMixin);
        }
    }
}
