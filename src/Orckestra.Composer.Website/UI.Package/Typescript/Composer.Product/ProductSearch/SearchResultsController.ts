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
                    sortingChanged(sortingType: string, url: string): void {
                        self.eventHub.publish(SearchEvents.SortingChanged, {
                            data: {sortingType, pageType: UrlHelper.resolvePageType(), url}
                        });
                    },
                    addToCart(product: any): void {
                        const {
                            HasVariants: hasVariants,
                            ProductId: productId,
                            VariantId: variantId,
                            RecurringOrderProgramName: recurringOrderProgramName
                        } = product;

                        const price: number = product.IsOnSale ? product.Price : product.ListPrice;

                      //  var busy = this.asyncBusy({ elementContext: actionContext.elementContext, containerContext: productContext });

                        if (hasVariants) {
                            self.productService.loadQuickBuyProduct(productId, variantId, 'productSearch', this.ListName)
                                .then(this.addToCartSuccess, this.onAddToCartFailed)
                              //  .fin(() => busy.done());

                        } else {
                            self.sendProductDataForAnalytics(product, price, this.ListName);

                            self.cartService.addLineItem(productId, '' + price, null, 1, null, recurringOrderProgramName)
                                .then(this.addToCartSuccess, this.onAddToCartFailed)
                              //  .fin(() => busy.done());
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
                        self.searchRepository.getSearchResults(data.queryString, data.categoryId, data.queryName, data.queryType)
                            .then(result => {
                                Object.keys(result.ProductSearchResults).forEach(key => this[key] = result.ProductSearchResults[key]);

                                self.eventHub.publish(SearchEvents.FacetsLoaded, { data: result });
                            });
                    }
                }
            });
        }

        public addToCart(actionContext: IControllerActionContext) {
            console.log(actionContext);
            var productContext: JQuery = $(actionContext.elementContext).closest('[data-product-id]');

            var hasVariants: string = <any>productContext.data('hasVariants');

            //Do not use .data since it may parse the id as a number.
            var productId: string = productContext.attr('data-product-id');
            var variantId: string = productContext.attr('data-product-variant-id');
            var recurringOrderProgramName: string = productContext.attr('data-recurring-order-program-name');

            var product = _.find(this.context.viewModel.SearchResults, function (product: any) {
                if (_.isEmpty(variantId)) {
                    return product.ProductId === productId;
                } else {
                    return product.ProductId === productId && product.VariantId === variantId;
                }
            });
            var price: number = product.IsOnSale ? product.Price : product.ListPrice;

            var busy = this.asyncBusy({ elementContext: actionContext.elementContext, containerContext: productContext });

            if (hasVariants === 'True') {

                this.productService.loadQuickBuyProduct(productId, variantId, 'productSearch', this.context.viewModel.ListName)
                    .then((data: any) => {
                        ErrorHandler.instance().removeErrors();
                        return data;
                    }, (reason: any) => this.onAddToCartFailed(reason))
                    .fin(() => busy.done());

            } else {
             //   var productData: any = this.getProductDataForAnalytics(productId, price);
            //    this.eventHub.publish('lineItemAdding', { data: productData });

                this.cartService.addLineItem(productId, '' + price, null, 1, null, recurringOrderProgramName)
                    .then((data: any) => {
                        ErrorHandler.instance().removeErrors();
                        return data;
                    }, (reason: any) => this.onAddToCartFailed(reason))
                    .fin(() => busy.done());
            }
        }

        protected onAddToCartFailed(reason: any): void {
            console.error('Error on adding item to cart', reason);

            ErrorHandler.instance().outputErrorFromCode('AddToCartFailed');
        }

        public searchProductClick(actionContext: IControllerActionContext) {
            var index: number = <any>actionContext.elementContext.data('index');
            var productId: string = actionContext.elementContext.data('productid').toString();
            var product: any = _.find(this.context.viewModel.SearchResults, { ProductId: productId });

            this.eventHub.publish('productClick', {
                data: {
                    Product: product,
                    ListName: this.context.viewModel.ListName,
                    Index: index,
                    PageNumber: this.currentPage.DisplayName,
                    MaxItemsPerPage: this.context.viewModel.MaxItemsPerPage
                }
            });
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
