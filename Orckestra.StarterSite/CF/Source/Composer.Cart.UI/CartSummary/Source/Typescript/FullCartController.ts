///<reference path='../../../../Composer.UI/Source/Typings/tsd.d.ts' />
///<reference path='../../../../Composer.UI/Source/TypeScript/Mvc/Controller.ts' />
///<reference path='../../../../Composer.UI/Source/TypeScript/Mvc/IControllerActionContext.ts' />
///<reference path='../../../../Composer.UI/Source/TypeScript/Repositories/CartRepository.ts' />
///<reference path='../../../../Composer.UI/Source/TypeScript/ErrorHandling/ErrorHandler.ts' />
///<reference path='CartService.ts' />

module Orckestra.Composer {

    export class FullCartController extends Orckestra.Composer.Controller {
        protected source: string = 'Checkout';
        private debounceUpdateLineItem: (args: any) => void;

        private loaded: boolean = false;
        private cartService: CartService = new CartService(new CartRepository(), this.eventHub);

        public initialize() {

            super.initialize();

            this.registerSubscriptions();
            this.loadCart();
        }

        private registerSubscriptions() {

            this.eventHub.subscribe('cartUpdated', e => this.onCartUpdated(e.data));
        }

        protected onCartUpdated(cart: any): void {
            this.render('CartContent', cart);
            ErrorHandler.instance().removeErrors();
        }

        private loadCart() {

            this.cartService.getFreshCart()
                .then(cart => {

                    if (this.loaded) {
                        return cart;
                    }

                    var e: IEventInformation = {
                        data: {
                            Cart: cart,
                            StepNumber: this.context.viewModel.CurrentStep
                        }
                    };

                    this.eventHub.publish('checkoutStepRendered', e);
                    return cart;

                })
                .done(cart => {

                    this.eventHub.publish('cartUpdated', { data: cart });
                    this.loaded = true;

                }, reason => this.loadCartFailed(reason));
        }

        protected loadCartFailed(reason: any): void {
            console.error('Error while loading the cart.', reason);
            this.context.container.find('.js-loading').hide();

            ErrorHandler.instance().outputErrorFromCode('LoadCartFailed');
        }

        public updateLineItem(actionContext: IControllerActionContext): void {
            if (!this.debounceUpdateLineItem) {
                this.debounceUpdateLineItem =
                    _.debounce((args) =>
                        this.executeLineItemUpdate(args), 300);
            }

            var context: JQuery = actionContext.elementContext;
            context.closest('.cart-row').addClass('is-loading');

            var lineItemId: string = <any>context.data('lineitemid');
            var productId: string = context.attr('data-productid');
            var action: string = <any>context.data('action');
            var quantity: number = parseInt(<any>context.data('quantity'), 10);
            var tmpQuantity: number = context.data('tmp-qte') ? parseInt(<any>context.data('tmp-qte'), 10) : null;
            var updatedQuantity: number = this.updateQuantity(action, tmpQuantity ? tmpQuantity : quantity);
            var recurringOrderFrequencyName: string = <any>context.data('recurringorderfrequencyname');
            var recurringOrderProgramName: string = <any>context.data('recurringorderprogramname');

            context.data('tmp-qte', updatedQuantity);

            var args: any = {
                context: context,
                lineItemId: lineItemId,
                originalQuantity: quantity,
                updatedQuantity: updatedQuantity,
                productId: productId,
                recurringOrderFrequencyName: recurringOrderFrequencyName,
                recurringOrderProgramName: recurringOrderProgramName
            };

            if (quantity !== updatedQuantity) {
                //use only debouced function when incrementing/decrementing quantity
                this.debounceUpdateLineItem(args);
            } else {
                this.executeLineItemUpdate(args);
            }
        }

        protected executeLineItemUpdate(args: any): void {
            this.cartService.getCart().then((cart: any) => {
                let lineItem = _.find(cart.LineItemDetailViewModels, (li: any) => li.Id === args.lineItemId);

                if (this.isUpdateRequired(args, lineItem)) {
                    let delta = args.updatedQuantity - lineItem.Quantity;
                    let positiveDelta: number = delta < 0 ? delta * -1 : delta;
                    let data = this.getLineItemDataForAnalytics(lineItem, positiveDelta);

                    if (delta !== 0) {
                        let eventName = (delta > 0) ? 'lineItemAdding' : 'lineItemRemoving';
                        this.eventHub.publish(eventName, { data: data });
                    }

                    args.context.removeData('tmp-qte');
                    args.context.data('quantity', args.updatedQuantity);

                    return this.cartService.updateLineItem(args.lineItemId,
                        args.updatedQuantity,
                        args.productId,
                        args.recurringOrderFrequencyName === '' ? null : args.recurringOrderFrequencyName,
                        args.recurringOrderProgramName);
                } else {
                    this.render('CartContent', cart);
                    return cart;
                }
            }).fail((reason: any) => this.lineItemUpdateFailed(args.context, reason));
        }

        protected isUpdateRequired(updateLineItemArgs, lineItem): boolean {
            if (!lineItem) {
                return false;
            }

            var shouldUpdateQuantity = updateLineItemArgs.updatedQuantity - lineItem.Quantity !== 0;
            var shouldUpdateRecurringFrequency = updateLineItemArgs.recurringOrderFrequencyName !== lineItem.RecurringOrderFrequencyName;
            var shouldUpdateRecurringProgram = updateLineItemArgs.recurringOrderProgramName !== lineItem.RecurringOrderProgramName;

            return shouldUpdateQuantity || shouldUpdateRecurringFrequency || shouldUpdateRecurringProgram;
        }

        protected lineItemUpdateFailed(context: JQuery, reason: any) {
            console.error('Error while updating line item quantity.', reason);
            context.closest('.cart-row').removeClass('is-loading');

            ErrorHandler.instance().outputErrorFromCode('LineItemUpdateFailed');
        }

        protected getLineItemDataForAnalytics(lineItem: any, quantity: number) : any {
            var data = {
                List: this.source,
                DisplayName: lineItem.ProductSummary.DisplayName,
                ProductId: lineItem.ProductId,
                ListPrice: lineItem.ListPrice,
                Brand: lineItem.ProductSummary.Brand,
                CategoryId: lineItem.ProductSummary.CategoryId,
                Variant: undefined,
                Quantity: quantity
            };

            if (lineItem.VariantId && lineItem.KeyVariantAttributesList) {
                data.Variant = this.buildVariantName(lineItem.KeyVariantAttributesList);
        }

            return data;
        }

        public updateQuantity(action: string, quantity: number): number {
            if (!action) {
                return quantity;
            }

            switch (action.toUpperCase()) {
                case 'INCREMENT':
                    quantity++;
                    break;

                case 'DECREMENT':
                    quantity--;
                    if (quantity < 1) {
                        quantity = 1;
                    }
                    break;
            }

            return quantity;
        }

        public deleteLineItem(actionContext: IControllerActionContext) {
            var context: JQuery = actionContext.elementContext;
            var lineItemId: string = <any>context.data('lineitemid');
            var productId: string = context.attr('data-productid');

            context.closest('.cart-row').addClass('is-loading');

            this.cartService.getCart()
                .then((cart: any) => {
                    var lineItem = _.find(cart.LineItemDetailViewModels, (li: any) => li.Id === lineItemId);

                    var data = this.getLineItemDataForAnalytics(lineItem, lineItem.Quantity);

                    this.eventHub.publish('lineItemRemoving', { data: data });

                    return this.cartService.deleteLineItem(lineItemId, productId);
                }).fail((reason: any) => this.onLineItemDeleteFailed(context, reason));
        }

        protected onLineItemDeleteFailed(context: JQuery, reason: any): void {
            console.error('Error while deleting line item.', reason);
            context.closest('.cart-row').removeClass('is-loading');

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
