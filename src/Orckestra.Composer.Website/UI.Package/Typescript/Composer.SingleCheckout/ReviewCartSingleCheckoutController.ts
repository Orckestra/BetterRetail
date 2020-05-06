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
                    this.updateBeforeEditLineItemList();
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
                                let itemToUpdate = _.find(this.Cart.LineItemDetailViewModels, (i: any) => i.Id === id);
                                self.checkoutService.updateCartItem(itemToUpdate.Id,
                                    itemToUpdate.Quantity,
                                    itemToUpdate.ProductId,
                                    itemToUpdate.RecurringOrderFrequencyName ? itemToUpdate.RecurringOrderFrequencyName : null,
                                    itemToUpdate.RecurringOrderProgramName)
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
                    },
                    changeRecurringMode(e, item) {
                        let { value } = e.target;
                        item.RecurringOrderFrequencyName = value !== 'single' && item.RecurringOrderProgramFrequencies.length
                            ? item.RecurringOrderProgramFrequencies[0].RecurringOrderFrequencyName : null
                    },
                    resetLineItemRecurringFrequency(item) {
                        let oldItem = this.beforeEditLineItemList.find(lineItem => lineItem.id === item.Id);
                        item.RecurringOrderFrequencyName = oldItem.name;
                        item.RecurringOrderFrequencyDisplayName = oldItem.displayName;
                    },
                    updateLineItemRecurringFrequency(event, item) {
                        let collapseId = $(event.target).data('lablecollapse');

                        if(!this.isRecurringFrequencyModified(item)) {
                            this.collapseById(collapseId, 'show');
                            return;
                        }

                        self.checkoutService.updateCartItem(item.Id,
                            item.Quantity,
                            item.ProductId,
                            item.RecurringOrderFrequencyName ? item.RecurringOrderFrequencyName : null,
                            item.RecurringOrderProgramName)
                            .finally(() => {
                               this.collapseById(collapseId, 'show');
                            })
                    },
                    collapseById(collapseId: string, action: string) {
                        $(`#${collapseId}`).collapse(action);
                    },
                    isRecurringFrequencyModified(item: any): boolean {
                        return this.beforeEditLineItemList.find(i => i.id === item.Id && i.name !== item.RecurringOrderFrequencyName)
                    },
                    updateBeforeEditLineItemList() {
                        this.beforeEditLineItemList = this.Cart.LineItemDetailViewModels.map(item => ({
                            name: item.RecurringOrderFrequencyName,
                            displayName: item.RecurringOrderFrequencyDisplayName,
                            id: item.Id
                        }));
                    }
                }
            };

            this.checkoutService.VueCheckoutMixins.push(vueReviewCartMixin);
        }
    }
}
