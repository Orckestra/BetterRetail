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
/// <reference path='../Product/InventoryService.ts' />

module Orckestra.Composer {
    'use strict';

    export class SearchResultsController extends Orckestra.Composer.Controller {
        protected cartService: ICartService = CartService.getInstance();
        protected inventoryService = new InventoryService();
        protected productService: ProductService = new ProductService(this.eventHub, this.context);
        protected currentPage: any;
        protected vueSearchResults: Vue;
        protected searchRepository: ISearchRepository = new SearchRepository();

        public initialize() {
            super.initialize();
            const { ProductSearchResults, ListName, MaxItemsPerPage } = this.context.viewModel;
            this.sendSearchResultsForAnalytics(ProductSearchResults, ListName, MaxItemsPerPage);

            const self = this;
            this.vueSearchResults = new Vue({
                el: `#${this.context.container.data('vueid')}`,
                components: {
                },
                data: {
                    ...ProductSearchResults,
                    ListName,
                    MaxItemsPerPage,
                    isLoading: false,
                    dataUpdatedTracker: 1,
                    ProductsMap: {},
                },
                mounted() {
                    this.registerSubscriptions();
                },
                computed: {
                    SearchResultsData() { 
                      // By using `dataUpdatedTracker` we tell Vue that this property depends on it,
                      // so it gets re-evaluated whenever `dataUpdatedTracker` changes
                      return this.dataUpdatedTracker && this.SearchResults;
                    },
                  },
                methods: {
                    getKeyVariantDisplayName(id, kvaName) {
                        const product = this.ProductsMap[id];
                        return ProductsHelper.getKeyVariantDisplayName(product, kvaName);
                    },
                    requireSelection(searchProduct, kvaName) {
                        const product = this.ProductsMap[searchProduct.ProductId];
                        return ProductsHelper.isSize(kvaName) ? !product.SizeSelected : false;
                    },
                    getKeyVariantValues(id, kvaName) {
                        const product = this.ProductsMap[id];
                        return ProductsHelper.getKeyVariantValues(product, kvaName, ProductsHelper.isSize(kvaName) ? !product.SizeSelected : false);
                    },
                    refreshData() {
                        this.dataUpdatedTracker += 1;
                    },
                    onMouseover(searchProduct) {
                        const { ProductId, VariantId, HasVariants } = searchProduct;
                        if(!HasVariants || this.ProductsMap[ProductId]) return;

                        this.loadingProduct(searchProduct, true, true);
                        const pricesTask = self.productService.calculatePrices(ProductId, this.ListName);
                        const productDetailsTask = self.productService.loadProduct(ProductId, VariantId);
                         Q.all([pricesTask, productDetailsTask])
                            .spread((prices, product) => {
                                product.ProductPrice = <any>_.find(prices.ProductPrices, { ProductId });
                                product.SelectedVariant = product.Variants.find(v => v.Id === VariantId);
                                product.SizeSelected = false;
                                this.ProductsMap[ProductId] = product;
                               })
                            .fin(() => this.loadingProduct(searchProduct, false, false));
                    },
                    selectKva(searchProduct, kvaName, kvaValue) {
                        const { ProductId: productId } = searchProduct;
                        const kva = { [kvaName]: kvaValue };
                        const product = this.ProductsMap[productId];
                        
                        const variant = ProductsHelper.findVariant(product, kva);
      
                        if(!variant) {
                           product.SizeSelected = false;
                            this.loadingProduct(searchProduct, false);
                            return;
                        };

                        this.loadingProduct(searchProduct, true);

                        product.SelectedVariant = variant;
                        searchProduct.ImageUrl = variant.Images.find(i => i.Selected).ImageUrl;

                        if(ProductsHelper.isSize(kvaName)) {
                            product.SizeSelected = true;
                        }

                        const variantPrice = product.ProductPrice.VariantPrices.find(p=> p.VariantId === variant.Id);
                        ProductsHelper.mergeVariantPrice(searchProduct, variantPrice);

                        self.inventoryService.isAvailableToSell(variant.Sku)
                        .then(result => searchProduct.IsAvailableToSell = result)
                        .fin(() => this.loadingProduct(searchProduct, false));
                    },
                    loadingProduct(product, loading, variantsLoading = false) {
                        product.loading = loading;
                        product.variantsLoading = variantsLoading;
                        this.refreshData(); 
                    },
                    productDetailsLoaded(searchProduct) {
                        return this.ProductsMap[searchProduct.ProductId] != undefined;
                    },
                    sortingChanged(url: string): void {
                        self.eventHub.publish(SearchEvents.SortingChanged, {data: {url}});
                    },
                    addToCart(event: any, product: any): void {
                        const {
                            HasVariants: hasVariants,
                            ProductId: productId,
                        } = product;
                        const price = product.IsOnSale ? product.Price : product.ListPrice;
                       
                        if(hasVariants) {
                            product.VariantId = this.ProductsMap[productId].SelectedVariant.Id;
                            product.Variants = this.ProductsMap[productId].Variants;
                        }

                        this.loadingProduct(product, true);
 
                        self.cartService.addLineItem(product, price, product.VariantId, 1, this.ListName)
                            .then(this.addToCartSuccess, this.onAddToCartFailed)
                            .fin(() => this.loadingProduct(product, false));
                        
                    },
                    isAddToCartDisabled(product) {
                        return ProductsHelper.isAddToCartDisabled(product, this.ProductsMap);
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

                        this.isLoading = true;
                        searchRequest.then(result => {
                            this.isLoading = false;
                            Object.keys(result.ProductSearchResults).forEach(key => this[key] = result.ProductSearchResults[key]);

                            self.eventHub.publish(SearchEvents.SearchResultsLoaded, { data: result });
                        });
                    },
                    searchProductClick(product, index): void {
                        self.sendProductClickForAnalytics(product, index, this.Pagination.CurrentPage, this.ListName, this.MaxItemsPerPage)
                    }
                }
            });
        }

        protected sendProductClickForAnalytics(product, index, currentPage, listName: string, maxItemsPerPage: any): void {
            const productData = {
                Product: product,
                ListName: listName,
                Index: index,
                PageNumber: currentPage.DisplayName,
                MaxItemsPerPage: maxItemsPerPage
            };

            this.eventHub.publish('productClick', { data: productData });
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
