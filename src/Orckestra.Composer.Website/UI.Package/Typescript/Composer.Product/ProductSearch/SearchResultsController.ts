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
///<reference path='../../Repositories/ISearchRepository.ts' />
///<reference path='../../Repositories/SearchRepository.ts' />
/// <reference path='./UrlHelper.ts' />
/// <reference path='../ProductEvents.ts' />
/// <reference path='./Constants/SearchEvents.ts' />

module Orckestra.Composer {
    'use strict';

    export class SearchResultsController extends Orckestra.Composer.Controller {
        protected cartService: ICartService = CartService.getInstance();
        protected productService: ProductService = new ProductService(this.eventHub, this.context);
        protected currentPage: any;
        protected vueSearchResults: Vue;
        protected searchRepository: ISearchRepository = new SearchRepository();

        public initialize() {
            super.initialize();
            this.sendSearchResultsForAnalytics(this.context.viewModel.ProductSearchResults);

            const self = this;
            this.vueSearchResults = new Vue({
                el: '#vueSearchResults',
                components: {
                },
                data: {
                    ...this.context.viewModel.ProductSearchResults
                },
                mounted() {
                    this.registerSubscriptions();
                },
                computed: {
                },
                methods: {
                    loadingProduct(product, loading) {
                        product.loading = loading;
                        this.SearchResults = [...this.SearchResults];
                    },
                    sortingChanged(url: string): void {
                        self.eventHub.publish(SearchEvents.SortingChanged, {data: {url}});
                    },
                    addToCart(product: any): void {
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

                        searchRequest.then(result => {
                            Object.keys(result.ProductSearchResults).forEach(key => this[key] = result.ProductSearchResults[key]);

                            self.eventHub.publish(SearchEvents.FacetsLoaded, { data: result });
                        });
                    },
                    searchProductClick(product, index): void {
                        self.sendProductClickForAnalytics(product, index, this.Pagination.CurrentPage)
                    }
                }
            });
        }

        protected sendProductClickForAnalytics(product, index, currentPage): void {
            const productData = {
                Product: product,
                ListName: this.context.viewModel.ListName,
                Index: index,
                PageNumber: currentPage.DisplayName,
                MaxItemsPerPage: this.context.viewModel.MaxItemsPerPage
            };

            this.eventHub.publish('productClick', { data: productData });
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

        protected sendSearchResultsForAnalytics(data: any): void {
            const { Pagination: {CurrentPage}, SearchResults, Keywords, TotalCount, ListName, MaxItemsPerPage } = data;

            const searchResultsData = {
                ProductSearchResults: SearchResults,
                Keywords,
                TotalCount,
                ListName,
                PageNumber: CurrentPage && CurrentPage.DisplayName || '',
                MaxItemsPerPage
            };

            this.eventHub.publish('searchResultRendered', { data: searchResultsData });
        }
    }
}
