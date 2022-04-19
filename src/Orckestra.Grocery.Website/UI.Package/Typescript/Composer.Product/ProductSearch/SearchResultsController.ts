/// <reference path='../../../Typings/tsd.d.ts' />
/// <reference path='../../../Typings/vue/index.d.ts' />
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
///<reference path='../../Repositories/ISearchRepository.ts' />
///<reference path='../../Repositories/SearchRepository.ts' />
/// <reference path='./UrlHelper.ts' />
/// <reference path='../ProductEvents.ts' />
/// <reference path='./Constants/SearchEvents.ts' />
///<reference path='../../Composer.Grocery/FulfillmentEvents.ts' />


module Orckestra.Composer {
    'use strict';

    export class SearchResultsController extends Orckestra.Composer.Controller {
        protected cartService: ICartService = CartService.getInstance();
        protected productService: ProductService = new ProductService(this.eventHub, this.context);
        protected wishListService: WishListService = new WishListService(new WishListRepository(), this.eventHub);
        protected membershipService: IMembershipService = new MembershipService(new MembershipRepository());
        protected searchRepository: ISearchRepository = new SearchRepository();
        protected currentPage: any;
        protected VueSearchResults: Vue;

        public initialize() {

            super.initialize();
            let authenticatedPromise = this.membershipService.isAuthenticated();

            const { ProductSearchResults, ListName, MaxItemsPerPage } = this.context.viewModel;
            this.sendSearchResultsForAnalytics(ProductSearchResults, ListName, MaxItemsPerPage);

            Q.all([authenticatedPromise]).spread((authVm) => this.initializeVueComponent(authVm));
        }

        private initializeVueComponent(authVm) {
            const { ProductSearchResults, ListName, MaxItemsPerPage, JsonContext } = this.context.viewModel;
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
                    this.registerSubscriptions();
                },
                data: {
                    ...ProductSearchResults,
                    SearchResults: JSON.parse(JsonContext).SearchResults,
                    ListName,
                    MaxItemsPerPage,

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

                            product.HasUnitValues = PriceHelper.HasUnitValues(product);
                            product.PricePerUnit = PriceHelper.PricePerUnit(product.DisplayListPrice,
                                product.ProductUnitQuantity,
                                product.ProductUnitSize,
                                product.ConvertedVolumeMeasurement
                            );

                            if (product.PricePerUnit) {
                                product.IsPricePerUnitZero = PriceHelper.IsPricePerUnitZero(product.PricePerUnit);
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
                        let product = this.SearchResults.find(product => product.ProductId === ProductId);

                        self.publishProductDataForAnalytics(product, cartItem.Quantity, this.ListName, analyticEventName);

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
                                        this.onAddToCartFailed(reason);
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
                    loadingProduct(product, loading) {
                        this.Loading = loading;
                        this.SearchResults = [...this.SearchResults];
                    },
                    sortingChanged(url: string): void {
                        self.eventHub.publish(SearchEvents.SortingChanged, {data: {url}});
                    },
                    addToCart(event: any, product: any): void {
                        const {
                            HasVariants: hasVariants,
                            ProductId: productId,
                            VariantId: variantId,
                            RecurringOrderProgramName: recurringOrderProgramName
                        } = product;

                        const price: number = product.IsOnSale ? product.Price : product.ListPrice;

                        this.loadingProduct(product, true);

                        if (hasVariants) {
                            self.productService.loadQuickBuyProduct(productId, variantId, 'productSearch', this.ListName)
                                .then(this.addToCartSuccess, this.onAddToCartFailed)
                                .fin(() => this.loadingProduct(product, false));

                        } else {
                            self.sendProductDataForAnalytics(product, price, this.ListName);

                            self.cartService.addLineItem(productId, '' + price, null, 1, null, recurringOrderProgramName)
                                .then(this.addToCartSuccess, this.onAddToCartFailed)
                                .fin(() => this.loadingProduct(product, false));
                        }
                    },
                    onAddToCartFailed(reason: any): void {
                        console.error('Error on adding item to cart', reason);

                        ErrorHandler.instance().outputErrorFromCode('AddToCartFailed');
                    },
                    addToCartSuccess(data: any): void {
                        ErrorHandler.instance().removeErrors();
                        return data;
                    },
                    registerSubscriptions(): void {
                        self.eventHub.subscribe(SearchEvents.SearchRequested, this.onSearchRequested.bind(this));
                    },
                    onSearchRequested({data}): void {
                        const searchRequest = (!data.categoryId && data.queryName) ?
                            self.searchRepository.getQuerySearchResults(data.queryString, data.queryName, data.queryType) :
                            self.searchRepository.getSearchResults(data.queryString, data.categoryId);

                        this.Loading = true;
                        searchRequest.then(result => {
                            this.Loading = false;
                            Object.keys(result.ProductSearchResults).forEach(key => this[key] = result.ProductSearchResults[key]);

                            self.eventHub.publish(SearchEvents.SearchResultsLoaded, { data: result });
                        });
                    },
                    productClick(product, index): void {
                        self.sendProductClickForAnalytics(product, index, this.Pagination.CurrentPage, this.ListName, this.MaxItemsPerPage)
                    }
                }
            });
        }

        protected onAddToWishFailed(reason: any): void {
            console.error('Error on adding item to wishList', reason);
            this.wishListService.clearCache();
            ErrorHandler.instance().outputErrorFromCode('AddToWishListFailed');
        }

        protected publishProductDataForAnalytics(product: any, quantity: number, listName: string, eventName: string): void {
            const data = {
                List: listName,
                ProductId: product.ProductId,
                DisplayName: product.DisplayName,
                ListPrice: product.IsOnSale ? product.Price : product.ListPrice,
                Brand: product.Brand,
                CategoryId: product.CategoryId,
                Quantity: quantity
            };

            this.eventHub.publish(eventName, { data });
        }

        protected sendProductClickForAnalytics(product, index, currentPage, listName: string, maxItemsPerPage: any): void {
            const productData = {
                Product: product,
                ListName: listName,
                Index: index,
                PageNumber: currentPage.DisplayName,
                MaxItemsPerPage: maxItemsPerPage
            };

            this.eventHub.publish(ProductEvents.ProductClick, { data: productData });
        }

        protected sendProductDataForAnalytics(product: any, price: any, list: string): void {
            const productData = {
                List: list,
                ProductId: product.ProductId,
                DisplayName: product.DisplayName,
                ListPrice: price,
                Brand: product.Brand,
                CategoryId: product.CategoryId,
                Quantity: 1
            };

            this.eventHub.publish(ProductEvents.LineItemAdding, { data: productData });
        }

        protected sendSearchResultsForAnalytics(productSearchResults: any, listName: string, maxItemsPerPage: any): void {
            const { Pagination: {CurrentPage}, SearchResults, Keywords, TotalCount} = productSearchResults;

            const searchResultsData = {
                ProductSearchResults: SearchResults,
                Keywords,
                TotalCount,
                ListName: listName,
                PageNumber: CurrentPage && CurrentPage.DisplayName || '',
                MaxItemsPerPage: maxItemsPerPage
            };

            this.eventHub.publish('searchResultRendered', { data: searchResultsData });
        }
    }
}
