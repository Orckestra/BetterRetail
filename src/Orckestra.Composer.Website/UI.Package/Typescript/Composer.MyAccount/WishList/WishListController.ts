///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Mvc/Controller.ts' />
///<reference path='../../Mvc/IControllerActionContext.ts' />
///<reference path='../../Composer.Cart/WishList/WishListRepository.ts' />
///<reference path='../../Composer.Cart/WishList/Services/WishListService.ts' />
///<reference path='../../Composer.Cart/CartSummary/CartService.ts' />
///<reference path='../../Repositories/CartRepository.ts' />
///<reference path='../../Composer.Product/Product/ProductHelpers.ts' />

module Orckestra.Composer {

    export class WishListController extends Orckestra.Composer.Controller {

        protected _wishListService: IWishListService = new WishListService(new WishListRepository(), this.eventHub);
        protected _cartService: ICartService = CartService.getInstance();
        protected VueWishList: Vue;

        public initialize() {

            super.initialize();

            const vueId = this.context.container.data("vueid");
            const self = this;

            this.VueWishList = new Vue({
                el: '#' + vueId,

                data: {
                    Items: [],
                    ActiveProductId: undefined,
                    ...this.context.viewModel
                },
                computed: {
                    Total() {
                        return this.Items.length;
                    }
                },
                methods: {
                    addToCart(item) {
                        const price = item.IsOnSale ? item.ListPrice : item.DefaultListPrice;

                        item.Loading = true;
                        this.Items = [...this.Items];

                        self.eventHub.publish('wishListLineItemAddingToCart', {
                            data: ProductsHelper.getProductDataForAnalytics(
                                item,
                                item.VariantId,
                                price,
                                self.getListNameForAnalytics(), 1
                            )
                        });

                        self._cartService.addLineItem(item, price, item.VariantId, 1, self.getListNameForAnalytics())
                            .then(() => {
                                item.Loading = false;
                                this.Items = [...this.Items];
                            })
                    },
                    copyShareUrl(shareUrl) {
                        (navigator as any).clipboard.writeText(shareUrl);

                        self.eventHub.publish('wishListCopyingShareUrl', {
                            data: {}
                        });
                    },
                    deleteLineItem(lineItemId) {
                        const item = this.Items.find(x => x.Id === lineItemId)
                        if (item) {
                            item.Removing = true;
                            this.Items = [...this.Items];
                        }

                        self._wishListService.removeLineItem(lineItemId)
                            .then(wishList => {
                                this.Items = this.Items.filter(x => x.Id !== lineItemId)
                            })
                    },
                    onMouseover(searchProduct) {
                        const { ProductId, VariantId } = searchProduct;
                        if (this.ActiveProductId) return;
                        this.ActiveProductId = VariantId ? VariantId: ProductId;
                    },
                    onMouseleave(searchProduct) {
                        this.ActiveProductId = undefined;
                    }
                }
            });
        }

        protected getListNameForAnalytics(): string {
            throw new Error('ListName not defined for this controller');
        }
    }
}
