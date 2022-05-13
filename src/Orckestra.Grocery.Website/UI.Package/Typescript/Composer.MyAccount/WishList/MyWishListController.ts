///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Mvc/Controller.ts' />
///<reference path='../../Mvc/IControllerActionContext.ts' />
///<reference path='./WishListController.ts' />
///<reference path='../../Composer.Product/ProductEvents.ts' />

module Orckestra.Composer {

    export class MyWishListController extends Orckestra.Composer.WishListController {
        protected VueWishList: Vue;

        public initialize() {

            super.initialize();

            const vueId = this.context.container.data("vueid");
            const self = this;

            this.VueWishList = new Vue({
                el: '#' + vueId,

                data: {
                    Items: [],
                    ...self.context.viewModel
                },
                mounted() {
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
                            data: self.getProductDataForAnalytics(
                                item.ProductId,
                                item.VariantId,
                                item.ProductSummary.DisplayName,
                                price,
                                item.ProductSummary.Brand,
                                item.ProductSummary.CategoryId
                            )
                        });

                        self._cartService.addLineItem(item.ProductId, price, item.VariantId, 1, null, item.RecurringOrderProgramName)
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
                        if(item) {
                            item.Removing = true;
                            this.Items = [...this.Items];
                        }

                        self._wishListService.removeLineItem(lineItemId)
                            .then(wishList => {
                                this.Items = this.Items.filter(x => x.Id !== lineItemId)
                            })
                    }
                }
            });
        }

        protected getListNameForAnalytics(): string {
            return 'My Wish List';
        }
    }
}
