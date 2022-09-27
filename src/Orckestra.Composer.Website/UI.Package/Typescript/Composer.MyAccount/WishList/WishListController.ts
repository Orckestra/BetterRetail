///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Mvc/Controller.ts' />
///<reference path='../../Mvc/IControllerActionContext.ts' />
///<reference path='../../Composer.Cart/WishList/WishListRepository.ts' />
///<reference path='../../Composer.Cart/WishList/Services/WishListService.ts' />
///<reference path='../../Composer.Cart/CartSummary/CartService.ts' />
///<reference path='../../Repositories/CartRepository.ts' />

module Orckestra.Composer {

    export class WishListController extends Orckestra.Composer.Controller {

        protected _wishListService: IWishListService = new WishListService(new WishListRepository(), this.eventHub);
        protected _cartService: ICartService =  CartService.getInstance();

        public initialize() {

            super.initialize();
        }

        public addToCart(actionContext: IControllerActionContext) {
            var context: JQuery = actionContext.elementContext;
            var container = context.closest('.wishlist-tile');
            var ProductId: string = <any>context.data('productid');
            var price: string = <any>context.data('price');
            var brand: string = <any>context.data('brand');
            var variantId: string = <any>context.data('variantid');
            var variant: string = <any>context.data('variant');
            var name: string = <any>context.data('name');
            var category: string = <any>context.data('category');
            var RecurringOrderProgramName: string = <any>context.data('recurringorderprogramname');

            this.eventHub.publish('wishListLineItemAddingToCart', {
                data: this.getProductDataForAnalytics(ProductId, variant, name, price, brand, category)
            });

            container.addClass('is-loading');

            this._cartService.addLineItem({ ProductId, RecurringOrderProgramName }, price, variantId, 1, this.getListNameForAnalytics())
                .fin(() => container.removeClass('is-loading'));

        }


        protected getListNameForAnalytics(): string {
            return 'WishList'
        }

        protected getProductDataForAnalytics(productId, variant, displayName, price, brand, category): any {

            var data = {
                List: this.getListNameForAnalytics(),
                ProductId: productId,
                Variant: variant,
                DisplayName: displayName,
                ListPrice: price,
                Brand: brand,
                CategoryId: category,
                Quantity: 1
            };

            return data;
        }
    }
}
