///<reference path='../..//Typings/tsd.d.ts' />
///<reference path='./BaseSingleCheckoutController.ts' />

module Orckestra.Composer {
    'use strict';

    export class ReviewCartSingleCheckoutController extends Orckestra.Composer.BaseSingleCheckoutController {

        public initialize() {
            let self: ReviewCartSingleCheckoutController = this;
            self.viewModelName = 'ReviewCart';

            super.initialize();

            let vueReviewCartMixin = {
                mounted() {
                    this.updateBeforeEditLineItemList();
                },
                methods: {
                    processCart() {
                        return true;
                    },
                    DecrementDisabled(item) {
                        return this.Mode.Loading;
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
                                let itemToUpdate = _.find(this.Cart.LineItemDetailViewModels, (i: any) => i.Id === id);

                                if (itemToUpdate.Quantity > 0) {
                                    self.checkoutService.updateCartItem(itemToUpdate.Id,
                                        itemToUpdate.Quantity,
                                        itemToUpdate.ProductId,
                                        itemToUpdate.RecurringOrderFrequencyName ? itemToUpdate.RecurringOrderFrequencyName : null,
                                        itemToUpdate.RecurringOrderProgramName)
                                } else {
                                    this.removeCartItem(itemToUpdate.Id);
                                }
                            }, 400);
                        }

                        this.debounceUpdateItem(id);
                    },
                    removeCartItem(id) {
                        let item = _.find(this.Cart.LineItemDetailViewModels, (i: any) => i.Id === id);
                        this.Mode.Loading = true;
                        self.checkoutService.removeCartItem(item.Id, item.ProductId)
                            .finally(() => {
                                this.Mode.Loading = false;
                            });

                        this.Cart.LineItemDetailViewModels = _.filter(this.Cart.LineItemDetailViewModels, (i: any) => i.Id != id);
                    },
                    updateBeforeEditLineItemList() {
                        this.beforeEditLineItemList = this.Cart.LineItemDetailViewModels.map(x => ({ ...x }));
                    }
                }
            };

            this.checkoutService.VueCheckoutMixins.push(vueReviewCartMixin);
        }
    }
}
