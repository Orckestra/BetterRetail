/// <reference path='../../../Typings/tsd.d.ts' />
/// <reference path='../../JQueryPlugins/ISerializeObjectJqueryPlugin.ts' />
/// <reference path='../../Mvc/Controller.ts' />
/// <reference path='../../ErrorHandling/ErrorHandler.ts' />
/// <reference path='../../Mvc/IControllerActionContext.ts' />
/// <reference path='../../Mvc/IControllerContext.ts' />
/// <reference path='../../Repositories/CartRepository.ts' />
/// <reference path='../../Composer.Cart/CartSummary/CartService.ts' />
/// <reference path='../Product/ProductService.ts' />
///<reference path='../../Composer.Cart/WishList/Services/WishListService.ts' />
///<reference path='../../Composer.Cart/WishList/WishListRepository.ts' />
///<reference path='../../Composer.MyAccount/Common/IMembershipService.ts' />
///<reference path='../../Composer.MyAccount/Common/MembershipService.ts' />
///<reference path='../../Utils/PriceHelper.ts' />


module Orckestra.Composer {
    'use strict';

    export class SearchResultsController extends Orckestra.Composer.Controller {
        protected cartService: ICartService = CartService.getInstance();
        protected productService: ProductService = new ProductService(this.eventHub, this.context);
        protected wishListService: WishListService = new WishListService(new WishListRepository(), this.eventHub);
        protected membershipService: IMembershipService = new MembershipService(new MembershipRepository());
        protected currentPage: any;
        protected VueSearchResults: Vue;

        public initialize() {

            super.initialize();
            let authenticatedPromise = this.membershipService.isAuthenticated();
            Q.all([authenticatedPromise]).spread((authVm) => this.initializeVueComponent(authVm));

            this.currentPage = this.getCurrentPage();

            let pageDisplayName;
            if (!this.currentPage || _.isUndefined(this.currentPage) || this.currentPage === null) {
                pageDisplayName = '';
            } else {
                pageDisplayName = this.currentPage.DisplayName;
            }

            this.eventHub.publish('searchResultRendered', {
                data: {
                    ProductSearchResults: this.context.viewModel.SearchResults,
                    Keywords: this.context.viewModel.Keywords,
                    TotalCount: this.context.viewModel.TotalCount,
                    ListName: this.context.viewModel.ListName,
                    PageNumber: pageDisplayName,
                    MaxItemsPerPage: this.context.viewModel.MaxItemsPerPage
                }
            });
        }

        private initializeVueComponent(authVm) {
            const { SearchResults } = this.context.viewModel;
            const vueId = this.context.container.data("vueid");
            let self = this;

            this.VueSearchResults = new Vue({
                el: '#' + vueId,
                components: {},
                mounted() {
                    self.eventHub.subscribe(CartEvents.CartUpdated, this.onCartUpdated);
                    self.eventHub.subscribe(FulfillmentEvents.StoreSelected, this.onStoreSelected);
                    self.cartService.getCart()
                        .then(cart => this.Cart = cart)
                        .fin(() => this.IsBusy = false);

                    self.wishListService.getWishListSummary()
                        .then(wishList => this.WishList = wishList);
                    self.eventHub.publish('iniCarousel', null);
                },
                data: {
                    SearchResults,
                    Cart: undefined,
                    WishList: undefined,
                    UpdatingProductId: undefined,
                    Loading: false,
                    IsBusy: true,
                    IsAuthenticated: authVm.IsAuthenticated,
                },
                computed: {
                    ExtendedSearchResults() {
                        const results = _.map(this.SearchResults, (product: any) => {
                            const isSameProduct = (i: any) => i.ProductId === product.ProductId && i.VariantId == product.VariantId;
                            let cartItem = this.Cart && this.Cart.LineItemDetailViewModels.find(isSameProduct);
                            product.InCart = !!cartItem;
                            product.LineItemId = cartItem ? cartItem.Id : undefined;
                            product.Quantity = cartItem ? cartItem.Quantity : 0;

                            const wishListItem = this.WishList && this.WishList.Items.find(isSameProduct);
                            product.InWishList = !!wishListItem;
                            product.WishListItemId = wishListItem ? wishListItem.Id : undefined;
                            //product.UnitPriceAvailable = product.UnitPrice != null && product.UnitPriceDeclaration != null;

                            product.HasUnitValues = (product.ProductUnitQuantity > 0) && (product.ProductUnitSize > 0) && (product.ProductUnitMeasure != null);
                            
                            if(product.ProductBadgeValues)
                            {
                                product.ProductBadgeMap = Object.keys(product.ProductBadgeValues)
                                    .map((key) => ({Key: key, Value: product.ProductBadgeValues[key]}));
                            }

                            product.PricePerUnit = PriceHelper.PricePerUnit(product.DisplayListPrice,
                                product.ProductUnitQuantity,
                                product.ProductUnitSize,
                                product.ConvertedVolumeMeasurement
                            );

                            if(product.PricePerUnit){
                                product.IsPricePerUnitZero = parseFloat(product.PricePerUnit.replace(/[^0-9\.-]+/g,'')) == 0.00;
                            }
                            return product;

                        });
                        return results;
                    },
                    UpdatingProduct() {
                        return _.find(this.Cart.LineItemDetailViewModels, (i: any) => i.ProductId == this.UpdatingProductId);
                    }
                },
                methods: {
                    productClick(product, index) {
                        self.eventHub.publish(ProductEvents.ProductClick, {
                            data: {
                                Product: product,
                                ListName: self.context.viewModel.ListName,
                                Index: index,
                                PageNumber: self.currentPage.DisplayName,
                                MaxItemsPerPage: self.context.viewModel.MaxItemsPerPage
                            }
                        });
                    },
                    onCartUpdated(result) {
                        this.Cart = result.data;
                    },
                    onStoreSelected() {
                        //window.location.reload(); TODO the best approach to reload search results
                    },
                    updateItemQuantity(item: any, quantity: number) {
                        let cartItem = _.find(this.Cart.LineItemDetailViewModels, (i: any) => i.Id === item.LineItemId);

                        if (this.Loading || !cartItem) return;

                        if (this.Cart.QuantityRange) {
                            const { Min, Max } = this.Cart.QuantityRange;
                            quantity = Math.min(Math.max(Min, quantity), Max);
                        }

                        if (quantity == cartItem.Quantity) {
                            //force update vue component
                            this.Cart = { ...this.Cart };
                            return;
                        }

                        let analyticEventName = quantity > cartItem.Quantity ? ProductEvents.LineItemAdding : ProductEvents.LineItemRemoving;
                        cartItem.Quantity = quantity;

                        if (cartItem.Quantity < 1) {
                            this.Loading = true; // disabling UI immediately when a line item is removed
                        }

                        let { ProductId, VariantId } = cartItem;

                        self.publishProductDataForAnalytics(ProductId, cartItem.Quantity, analyticEventName);

                        if (!this.debounceUpdateItem) {
                            this.debounceUpdateItem = _.debounce(({ Id, Quantity, ProductId }) => {
                                this.Loading = true;
                                this.UpdatingProductId = ProductId;
                                let updatePromise = Quantity > 0 ?
                                    self.cartService.updateLineItem(Id, Quantity, ProductId) :
                                    self.cartService.deleteLineItem(Id, ProductId);

                                updatePromise
                                    .then(() => {
                                        ErrorHandler.instance().removeErrors();
                                    }, (reason: any) => {
                                        self.onAddToCartFailed(reason);
                                        throw reason;
                                    })
                                    .fin(() => this.Loading = false);

                            }, 400);
                        }

                        this.debounceUpdateItem(cartItem);
                    },
                    addLineItemToWishList(item, event: JQueryEventObject) {
                        if (!this.IsAuthenticated) {
                            return self.wishListService.redirectToSignIn();
                        }

                        let { DisplayName, ProductId, VariantId, ListPrice, RecurringOrderProgramName } = item;
                        self.eventHub.publish('wishListLineItemAdding', {
                            data: { DisplayName, ListPrice: ListPrice }
                        });
                        self.wishListService.addLineItem(ProductId, VariantId, 1, null, RecurringOrderProgramName)
                            .then(wishList => this.WishList = wishList).fail(self.onAddToWishFailed);
                    },

                    removeLineItemFromWishList(item, event: JQueryEventObject) {
                        if (!this.IsAuthenticated) {
                            return self.wishListService.redirectToSignIn();
                        }

                        self.wishListService.removeLineItem(item.WishListItemId)
                            .then(wishList => this.WishList = wishList).fail(self.onAddToWishFailed);
                    },
                    addToCart: this.addToCart.bind(this),
                }
            });
        }

        protected onAddToWishFailed(reason: any): void {
            console.error('Error on adding item to wishList', reason);
            this.wishListService.clearCache();
            ErrorHandler.instance().outputErrorFromCode('AddToWishListFailed');
        }

        private getCurrentPage(): any {

            return <any>this.context.viewModel.PaginationCurrentPage;
        }

        public addToCart(event, product) {
            const {HasVariants, ProductId, VariantId, Price, RecurringOrderProgramName} = product;

            let promise: Q.Promise<any>;
            product.Loading = true;
            event.target.disabled = true;

            if (HasVariants) {
                promise = this.productService.loadQuickBuyProduct(ProductId, VariantId, 'productSearch', this.context.viewModel.ListName)
                    .then((data: any) => {
                        ErrorHandler.instance().removeErrors();
                        return data;
                    }, (reason: any) => this.onAddToCartFailed(reason));
            } else {
                this.publishProductDataForAnalytics(ProductId, 1, ProductEvents.LineItemAdding);

                promise = this.cartService.addLineItem(ProductId, '' + Price, null, 1, null, RecurringOrderProgramName)
                    .then((data: any) => {
                        ErrorHandler.instance().removeErrors();
                        return data;
                    }, (reason: any) => this.onAddToCartFailed(reason));
            }

            promise.fin(() =>{
                event.target.disabled = false;
                product.Loading = false;
            } );
        }

        protected onAddToCartFailed(reason: any): void {
            console.error('Error on adding item to cart', reason);

            ErrorHandler.instance().outputErrorFromCode('AddToCartFailed');
        }

        protected publishProductDataForAnalytics(productId: string, quantity: number, eventName: string): void {
            const data = this.getProductDataForAnalytics(productId, quantity);

            this.eventHub.publish(eventName, { data });
        }

        protected getProductDataForAnalytics(productId: string, quantity: number): any {
            var results = this.context.viewModel.SearchResults;
            var vm = _.find(results, (r: any) => r.ProductId === productId);

            if (!vm) {
                throw new Error(`Could not find a product with the ID '${productId}'.`);
            }

            var data = {
                List: this.context.viewModel.ListName,
                ProductId: vm.ProductId,
                DisplayName: vm.DisplayName,
                ListPrice: vm.IsOnSale ? vm.Price : vm.ListPrice,
                Brand: vm.Brand,
                CategoryId: vm.CategoryId,
                Quantity: quantity
            };

            return data;
        }
    }
}
