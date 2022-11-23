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
///<reference path='../../Composer.MyAccount/Common/IMembershipService.ts' />
///<reference path='../../Composer.MyAccount/Common/MembershipService.ts' />
/// <reference path='./Services/ShowFacetsService.ts' />

module Orckestra.Composer {
    'use strict';

    export class SearchResultsController extends Orckestra.Composer.Controller {
        protected cartService: ICartService = CartService.getInstance();
        protected wishListService: WishListService = new WishListService(new WishListRepository(), this.eventHub);
        protected membershipService: IMembershipService = new MembershipService(new MembershipRepository());
        protected inventoryService = new InventoryService();
        protected productService: ProductService = new ProductService(this.eventHub, this.context);
        protected currentPage: any;
        protected vueSearchResults: Vue;
        protected showFacetsService: ShowFacetsService = ShowFacetsService.instance();
        protected searchRepository: ISearchRepository = new SearchRepository();

        public initialize() {
            super.initialize();
           
            let getWithListTask = this.wishListService.getWishListSummary();
            let authenticatedPromise = this.membershipService.isAuthenticated();
            Q.all([authenticatedPromise, getWithListTask]).spread((authVm, wishList) => this.initializeVueComponent(wishList, authVm));
        }

        protected initializeVueComponent(wishlist, authVm) {
            const { ProductSearchResults, ListName, MaxItemsPerPage } = this.context.viewModel;
            let FacetsVisible = this.context.container.data('facets-visible') || "True";
            const isOverriden = this.context.container.data('overriden') === 'True';

            this.sendSearchResultsForAnalytics(ProductSearchResults, ListName, MaxItemsPerPage);
            const self = this;
            $('[data-toggle="popover"]').popover({
                placement:"top"
            });
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
                    WishList: wishlist,
                    IsAuthenticated: authVm.IsAuthenticated,
                    ActiveProductId: undefined,
                    FacetsVisible: FacetsVisible,
                    isOverriden: isOverriden,
                },
                mounted() {
                    this.registerSubscriptions();
                    self.showFacetsService.getShowFacets().then(
                        value => {
                            if (value == "False") {
                                FacetsVisible = "True";
                                this.toggleFacet();  
                           }
                        }, 
                        failed => {
                            FacetsVisible = "True";
                            this.vueSearchResults.data.FacetsVisible = "True";
                            self.vueSearchResults.$el.setAttribute('data-facets-visible', 'True');
                            self.showFacetsService.setShowFacets("True");
                        }
                    );
                },
                computed: {
                    SearchResultsData() {
                        // By using `dataUpdatedTracker` we tell Vue that this property depends on it,
                        // so it gets re-evaluated whenever `dataUpdatedTracker` changes
                        const results = _.map(this.SearchResults, (product: any) => {
                            product.WishListItem = this.WishList && this.WishList.Items.find(i => i.ProductId === product.ProductId && i.VariantId == product.VariantId);
                            return product;
                        });
                    
                     
                      return this.dataUpdatedTracker && results;
                    },
                },
                updated: function () {
                    this.updateProductColumns();
                },  
                methods: {
                    toggleFacet(): void {
                        let rightCol = document.getElementById("rightCol");
                        if (FacetsVisible == "True") {
                            FacetsVisible = "False";
                            self.vueSearchResults.$el.setAttribute('data-facets-visible', 'False');
                            document.getElementById("leftCol").classList.add("w-0-lg");
                            rightCol.classList.remove("col-lg-9");
                            
                        }
                        else {
                            FacetsVisible = "True";
                            self.vueSearchResults.$el.setAttribute('data-facets-visible', 'True');
                            document.getElementById("leftCol").classList.remove("w-0-lg");
                            rightCol.classList.add("col-lg-9");
                        }
                        self.vueSearchResults.$data.FacetsVisible = FacetsVisible;
                        self.showFacetsService.setShowFacets(FacetsVisible);
                    },
                    updateProductColumns(){
                        let searchContainer = document.getElementsByClassName("search-container");
                        if (isOverriden) return;
                        if (FacetsVisible == "True") {
                            for (let i=0; i < searchContainer.length; i++) {
                                searchContainer[i].classList.replace("col-md-3", "col-md-4");
                                searchContainer[i].classList.replace("col-xl-3", "col-xl-4");
                            }
                        }
                        else {
                            for (let i=0; i < searchContainer.length; i++) {
                                searchContainer[i].classList.replace("col-md-4", "col-md-3");
                                searchContainer[i].classList.replace("col-xl-4", "col-xl-3");
                            }
                        }
                    },
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
                        if(this.ActiveProductId) return;
                        this.ActiveProductId = ProductId;
                        if(!HasVariants || this.ProductsMap[ProductId]) return;
                        this.loadingProduct(searchProduct, true, true);
                        const pricesTask = self.productService.calculatePrices(ProductId, this.ListName);
                        const productDetailsTask = self.productService.loadProduct(ProductId, VariantId);
                         Q.all([pricesTask, productDetailsTask])
                            .spread((prices, product) => {
                                product.ProductPrice = <any>_.find(prices.ProductPrices, { ProductId });
                                product.SelectedVariant = product.Variants.find(v => v.Id === VariantId);
                                product.SizeSelected = true;
                                this.ProductsMap[ProductId] = product;
                               })
                            .fin(() => this.loadingProduct(searchProduct, false, false));
                    },
                    onMouseleave(searchProduct) {
                        this.ActiveProductId = undefined;
                    },
                    onKvaHover(event: MouseEvent) {
                        let target = $(event.target);
                        if (target.hasClass("kva-color-value")) return;
                        $(target).popover('show');
                    },
                    onKvaOut(event: MouseEvent) {
                        let target = $(event.target);
                        if (target.hasClass("kva-color-value")) return;
                        $(target).popover('hide');
                    },
                    selectKva(searchProduct, kvaName, kvaValue) {
                        const { ProductId: productId } = searchProduct;
                        const kva = { [kvaName]: kvaValue };
                        const product = this.ProductsMap[productId];
                        
                        let variant = ProductsHelper.findVariant(product, kva, product.SelectedVariant.Kvas);
      
                        if(!variant) {
                           variant = ProductsHelper.findVariant(product, kva, null);
                           //reset size selection to select existent variant 
                           product.SizeSelected = false;
                        };

                        this.loadingProduct(searchProduct, true);

                        product.SelectedVariant = variant;
                        searchProduct.VariantId = variant.Id;
                        searchProduct.ImageUrl = variant.Images.find(i => i.Selected).ImageUrl;

                        if(ProductsHelper.isSize(kvaName)) {
                            product.SizeSelected = true;
                        }

                        const variantPrice = product.ProductPrice.VariantPrices.find(p=> p.VariantId === variant.Id);
                        ProductsHelper.mergeVariantPrice(searchProduct, variantPrice);
                        this.ActiveProductId = productId;
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
                            .fail(reason => this.onAddToCartFailed(reason, 'AddToCartFailed'))
                            .fin(() => this.loadingProduct(product, false));
                        
                    },
                    isAddToCartDisabled(product) {
                        return ProductsHelper.isAddToCartDisabled(product, this.ProductsMap);
                    },
                    onAddToCartFailed(reason: any, errorCode): void {
                        console.error('Error on adding item to cart', reason);

                        ErrorHandler.instance().outputErrorFromCode(errorCode);
                    },
                    addLineItemToWishList(searchProduct) {
                        if (!this.IsAuthenticated) {
                            return self.wishListService.redirectToSignIn();
                        }

                        let { ProductId, VariantId, RecurringOrderProgramName } = searchProduct;

                        self.wishListService.addLineItem(ProductId, VariantId, 1, undefined, RecurringOrderProgramName)
                            .then(wishList => this.WishList = wishList)
                            .fail(reason => this.onAddToCartFailed(reason, 'AddToWishListFailed'));
                    },

                    removeLineItemFromWishList(searchProduct) {
                        self.wishListService.removeLineItem(searchProduct.WishListItem.Id)
                            .then(wishList => this.WishList = wishList)
                            .fail(reason => this.onAddToCartFailed(reason, 'AddToWishListFailed'));
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
