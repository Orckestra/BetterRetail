///<reference path='../..//Typings/tsd.d.ts' />
///<reference path='./BaseSingleCheckoutController.ts' />

module Orckestra.Composer {
    'use strict';

    export class ReviewCartSingleCheckoutController extends Orckestra.Composer.BaseSingleCheckoutController {

        public initialize() {
            var self: ReviewCartSingleCheckoutController = this;
            self.viewModelName = 'ReviewCart';

            super.initialize();
            this.registerSubscriptions();

            var vueReviewCartMixin = {

                methods: {
                    DecrementDisabled(item) {
                        return item.Quantity < 2 || this.IsLoading;
                    },
                    IncrementDisabled(item) {
                        return item.Quantity >= 99 || this.IsLoading;
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
                                this.IsLoading = true;
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
                                        this.IsLoading = false;
                                    })
                            }, 400);
                        }

                        this.debounceUpdateItem(id);
                    },

                    removeCartItem(index) {
                        var item = this.Cart.LineItemDetailViewModels[index];
                        this.IsLoading = true;
                        self.checkoutService.removeCartItem(item.Id, item.ProductId)
                            .then(cart => {
                                if (cart) {
                                    this.Cart = cart;
                                }
                            })
                            .finally(() => {
                                this.IsLoading = false;
                            });

                        this.Cart.LineItemDetailViewModels.splice(index, 1);
                    }
                }
            };

            this.checkoutService.VueCheckoutMixins.push(vueReviewCartMixin);
        }
    }
}
