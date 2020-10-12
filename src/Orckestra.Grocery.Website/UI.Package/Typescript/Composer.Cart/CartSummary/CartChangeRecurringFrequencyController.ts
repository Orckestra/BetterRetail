///<reference path='../../../Typings/tsd.d.ts' />


module Orckestra.Composer {
    'use strict';

    export class CartChangeRecurringFrequencyController extends Orckestra.Composer.Controller {

        protected cartStateService: ICartStateService = CartStateService.getInstance();
        protected cartService: ICartService = CartService.getInstance();

        public initialize() {
            super.initialize();
            let self: CartChangeRecurringFrequencyController = this;

            let vueChangeRecurringFrequencyMixin = {
                methods: {
                    changeRecurringMode(e, item) {
                        let { value } = e.target;
                        item.RecurringOrderFrequencyName = value !== 'single' && item.RecurringOrderProgramFrequencies.length
                            ? item.RecurringOrderProgramFrequencies[0].RecurringOrderFrequencyName : null
                    },
                    resetLineItemRecurringFrequency(item) {
                        let oldItem = this.beforeEditLineItemList.find(lineItem => lineItem.Id === item.Id);
                        item.RecurringOrderFrequencyName = oldItem.RecurringOrderFrequencyName;
                        item.RecurringOrderFrequencyDisplayName = oldItem.RecurringOrderFrequencyDisplayName;
                    },
                    updateLineItemRecurringFrequency(event, item) {
                        let collapseId = $(event.target).data('lablecollapse');

                        if(!this.isRecurringFrequencyModified(item)) {
                            this.collapseById(collapseId, 'show');
                            return;
                        }

                        self.cartService.updateLineItem(item.Id,
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
                        return this.beforeEditLineItemList.find(i => i.Id === item.Id && i.RecurringOrderFrequencyName !== item.RecurringOrderFrequencyName)
                    },
                }
            };

            this.cartStateService.VueCartMixins.push(vueChangeRecurringFrequencyMixin);
        }
    }
}
