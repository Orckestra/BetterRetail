///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Mvc/Controller.ts' />
///<reference path='../../Mvc/IControllerActionContext.ts' />
///<reference path='../../Repositories/CartRepository.ts' />
///<reference path='../../ErrorHandling/ErrorHandler.ts' />
///<reference path='../../Events/EventScheduler.ts' />
///<reference path='../RecurringOrder/Services/RecurringOrderService.ts' />
///<reference path='../RecurringOrder/Repositories/RecurringOrderRepository.ts' />
///<reference path='./CartService.ts' />
///<reference path='./CartStateService.ts' />
///<reference path='./CartEvents.ts' />
///<reference path='../../Composer.Product/ProductEvents.ts' />

module Orckestra.Composer {

    export class CartSummaryController extends Orckestra.Composer.Controller {
        protected source: string = 'Checkout';
        protected debounceUpdateLineItem: (args: any) => void;

        protected loaded: boolean = false;
        protected cartService: ICartService = CartService.getInstance();
        protected cartStateService: ICartStateService = CartStateService.getInstance();

        public initialize() {

            super.initialize();
            let self: CartSummaryController = this;

            let cartSummaryMixins = {
                methods: {
                    DecrementDisabled(item) {
                        return this.IsLoading || (this.Cart.QuantityRange && item.Quantity <= this.Cart.QuantityRange.Min);
                    },
                    IncrementDisabled(item) {
                        return this.IsLoading || (this.Cart.QuantityRange && item.Quantity >= this.Cart.QuantityRange.Max);
                    },
                    updateItemQuantity(item, quantity: number) {
                        if (this.Mode.Loading) return;

                        if (this.Cart.QuantityRange) {
                            const { Min, Max } = this.Cart.QuantityRange;
                            quantity = Math.min(Math.max(Min, quantity), Max);
                        }

                        if (quantity == item.Quantity) {
                            //force update vue component
                            this.Cart = { ...this.Cart };
                            return;
                        }

                        let analyticEventName = quantity > this.Quantity ? ProductEvents.LineItemAdding : ProductEvents.LineItemRemoving;
                        item.Quantity = quantity;

                        if (item.Quantity < 1) {
                            this.Mode.Loading = true; // disable ui immediately when we will delete  the line item
                        }

                        if (!this.debounceUpdateItem) {
                            this.debounceUpdateItem = _.debounce(itemToUpdate => {
                                const { Id, Quantity, ProductId, RecurringOrderFrequencyName: Frequency, RecurringOrderProgramName: Program } = itemToUpdate;

                                self.publishProductDataForAnalytics(itemToUpdate, analyticEventName);

                                this.Mode.Loading = true;

                                let updatePromise = Quantity > 0 ?
                                    self.cartService.updateLineItem(Id, Quantity, ProductId, Frequency || null, Program) :
                                    self.cartService.deleteLineItem(Id, ProductId);

                                updatePromise.then(cart => {
                                    this.Cart = cart;
                                })
                                    .fail((reason: any) => self.lineItemUpdateFailed(reason))
                                    .fin(() => this.Mode.Loading = false)
                            }, 400);
                        }

                        this.debounceUpdateItem(item);
                    },
                    removeCartItem(id) {
                        let item = _.find(this.Cart.LineItemDetailViewModels, (i: any) => i.Id === id);
                        self.publishProductDataForAnalytics(item, ProductEvents.LineItemRemoving);

                        this.Mode.Loading = true;
                        self.cartService.deleteLineItem(item.Id, item.ProductId)
                            .then(cart => {
                                if (cart) {
                                    this.Cart = cart;
                                }
                            })
                            .fail((reason: any) => self.onLineItemDeleteFailed(reason))
                            .finally(() => {
                                this.Mode.Loading = false;
                            });

                        this.Cart.LineItemDetailViewModels = _.filter(this.Cart.LineItemDetailViewModels, (i: any) => i.Id != id);
                    }
                }
            }

            this.cartStateService.VueCartMixins.push(cartSummaryMixins);
        }

        protected publishProductDataForAnalytics(lineItem: any, eventName: string): void {
            const data = this.getLineItemDataForAnalytics(lineItem);
            this.eventHub.publish(eventName, { data });
        }

        protected getLineItemDataForAnalytics(lineItem: any): any {
            const data = {
                List: this.source,
                DisplayName: lineItem.ProductSummary.DisplayName,
                ProductId: lineItem.ProductId,
                ListPrice: lineItem.ListPrice,
                Brand: lineItem.ProductSummary.Brand,
                CategoryId: lineItem.ProductSummary.CategoryId,
                Variant: undefined,
                Quantity: lineItem.Quantity
            };

            if (lineItem.VariantId && lineItem.KeyVariantAttributesList) {
                data.Variant = this.buildVariantName(lineItem.KeyVariantAttributesList);
            }

            return data;
        }

        protected lineItemUpdateFailed(reason: any) {
            console.error('Error while updating line item quantity.', reason);
            ErrorHandler.instance().outputErrorFromCode('LineItemUpdateFailed');
        }

        protected onLineItemDeleteFailed(reason: any): void {
            console.error('Error while deleting line item.', reason);
            ErrorHandler.instance().outputErrorFromCode('LineItemDeleteFailed');
        }

        protected buildVariantName(kvas: any[]): string {
            var nameParts: string[] = [];

            for (var i: number = 0; i < kvas.length; i++) {
                var value: any = kvas[i].OriginalValue;

                nameParts.push(value);
            }

            return nameParts.join(' ');
        }
    }
}
