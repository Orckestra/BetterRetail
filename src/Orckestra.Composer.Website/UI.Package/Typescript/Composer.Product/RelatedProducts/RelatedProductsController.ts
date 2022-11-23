/// <reference path='../Product/ProductController.ts' />
///<reference path='../../Plugins/SlickCarouselPlugin.ts' />
///<reference path='../ProductEvents.ts' />
module Orckestra.Composer {
    export class RelatedProductController extends Orckestra.Composer.ProductController  {

        protected concern: string = 'relatedProduct';
        private source: string = 'Related Products';
        protected VueRelatedProducts: Vue;
        protected wishListService: WishListService = new WishListService(new WishListRepository(), this.eventHub);
        protected membershipService: IMembershipService = new MembershipService(new MembershipRepository());

        private products: any[];

        public initialize() {

            super.initialize();
            let self: RelatedProductController = this;
            let vm = self.context.viewModel;
            let relatedProductsPromise = self.getRelatedProducts();
            let getWithListTask = this.wishListService.getWishListSummary();
            let authenticatedPromise = this.membershipService.isAuthenticated();
            Q.all([relatedProductsPromise, getWithListTask, authenticatedPromise])
            .spread((relatedproducts, wishlist, authVm) => {
                self.VueRelatedProducts = new Vue({
                    el: '#vueRelatedProducts',
                    data: {
                        RelatedProducts: self.products,
                        ProductIdentifiers: vm.ProductIdentifiers,
                        Loading: false,
                        ProductsMap: {},
                        dataUpdatedTracker: 1,
                        WishList: wishlist,
                        IsAuthenticated: authVm.IsAuthenticated,
                        ActiveProductId: undefined

                    },
                    mounted() {
                        self.eventHub.subscribe(CartEvents.CartUpdated, this.onCartUpdated);
                        self.eventHub.publish('iniCarousel', null);
                    },
                    computed: {
                        ExtendedRelatedProducts() {
                            const results = _.map(this.RelatedProducts, (product: any) => {
                                product.WishListItem = this.WishList && this.WishList.Items.find(i => i.ProductId === product.ProductId && i.VariantId == product.VariantId);
                                return product;
                            });
                            return this.dataUpdatedTracker && results;
                        }
                    },
                    methods: {
                        onCartUpdated(cart) {
                            this.Cart = cart.data;
                        },
                        isAddToCartDisabled(product) {
                            return ProductsHelper.isAddToCartDisabled(product, this.ProductsMap);
                        },
                        productDetailsLoaded(relatedProduct) {
                            return this.ProductsMap[relatedProduct.ProductId] != undefined;
                        },
                        getKeyVariantDisplayName(id, kvaName) {
                            const product = this.ProductsMap[id];
                            return ProductsHelper.getKeyVariantDisplayName(product, kvaName);
                        },
                        requireSelection(relatedProduct, kvaName) {
                            const product = this.ProductsMap[relatedProduct.ProductId];
                            return ProductsHelper.isSize(kvaName) ? !product.SizeSelected : false;
                        },
                        getKeyVariantValues(id, kvaName) {
                            const product = this.ProductsMap[id];
                            return ProductsHelper.getKeyVariantValues(product, kvaName, ProductsHelper.isSize(kvaName) ? !product.SizeSelected : false);
                        },
                        onMouseover(relatedProduct) {
                            const { ProductId, VariantId, HasVariants } = relatedProduct;
                            this.ActiveProductId = ProductId;
                            if (!HasVariants || this.ProductsMap[ProductId]) return;

                            this.loadingProduct(relatedProduct, true);
                            self.productService.loadProduct(ProductId, VariantId)
                                .then(product => {
                                    product.SelectedVariant = product.Variants.find(v => v.Id === VariantId);
                                    product.SizeSelected = true;
                                    this.ProductsMap[ProductId] = product;
                                })
                                .fin(() => this.loadingProduct(relatedProduct, false));
                        },
                        onMouseleave(relatedProduct) {
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
                        selectKva(relatedProduct, kvaName, kvaValue) {
                            const { ProductId: productId } = relatedProduct;
                            const kva = { [kvaName]: kvaValue };
                            const product = this.ProductsMap[productId];

                            let variant = ProductsHelper.findVariant(product, kva, product.SelectedVariant.Kvas);

                            if (!variant) {
                                variant = ProductsHelper.findVariant(product, kva, null);
                                //reset size selection to select existent variant 
                                product.SizeSelected = false;
                            };

                            this.loadingProduct(relatedProduct, true);

                            product.SelectedVariant = variant;
                            relatedProduct.ImageUrl = variant.Images.find(i => i.Selected).ImageUrl;
                            relatedProduct.VariantId = variant.Id;
                            if (ProductsHelper.isSize(kvaName)) {
                                product.SizeSelected = true;
                            }

                            const variantPrice = relatedProduct.ProductPrice.VariantPrices.find(p => p.VariantId === variant.Id);
                            ProductsHelper.mergeVariantPrice(relatedProduct, variantPrice);

                            self.inventoryService.isAvailableToSell(variant.Sku)
                                .then(result => relatedProduct.IsAvailableToSell = result)
                                .fin(() => this.loadingProduct(relatedProduct, false));
                        },
                        searchProductClick(product, index) {
                            self.eventHub.publish(ProductEvents.ProductClick, {
                                data: {
                                    Product: product,
                                    ListName: self.getPageSource(),
                                    Index: index
                                }
                            });
                        },
                        addToCart(event, product) {
                            const { HasVariants, ProductId } = product;

                            const price = product.IsOnSale ? product.Price : product.ListPrice;
                       
                            if(HasVariants) {
                                product.VariantId = this.ProductsMap[ProductId].SelectedVariant.Id;
                                product.Variants = this.ProductsMap[ProductId].Variants;
                            }

                            this.loadingProduct(product, true);

                            self.cartService.addLineItem(product, price, product.VariantId,  1, self.getListNameForAnalytics())
                                .fail(reason => this.onAddToCartFailed(reason, 'AddToCartFailed'))
                                .fin(() => this.loadingProduct(product, false));
                        },
                        addLineItemToWishList(relatedProduct) {
                            if (!this.IsAuthenticated) {
                                return self.wishListService.redirectToSignIn();
                            }
    
                            let { ProductId, VariantId, RecurringOrderProgramName } = relatedProduct;
    
                            self.wishListService.addLineItem(ProductId, VariantId, 1, undefined, RecurringOrderProgramName)
                                .then(wishList => this.WishList = wishList)
                                .fail(reason => this.onAddToCartFailed(reason, 'AddToWishListFailed'));
                        },
    
                        removeLineItemFromWishList(relatedProduct) {
                            self.wishListService.removeLineItem(relatedProduct.WishListItem.Id)
                                .then(wishList => this.WishList = wishList)
                                .fail(reason => this.onAddToCartFailed(reason, 'AddToWishListFailed'));
                        },
                        onAddToCartFailed(reason: any, errorCode): void {
                            console.error('Error on adding item to cart', reason);
    
                            ErrorHandler.instance().outputErrorFromCode(errorCode);
                        },
                        refreshData() {
                            this.dataUpdatedTracker += 1;
                        },
                        loadingProduct(product, loading) {
                            product.loading = loading;
                            this.refreshData(); 
                        },
                    }
                });
            })
        }


        private getRelatedProducts(): Q.Promise<any> {
            let vm = this.context.viewModel;
            let identifiers = vm.ProductIdentifiers;
            return this.productService.getRelatedProducts(identifiers)
                .then(relatedProductsVm => {
                    this.products = relatedProductsVm.Products;
                    vm.Products = relatedProductsVm.Products;
                    return vm;
                })
                .then(vm => {
                    if (vm && vm.Products && vm.Products.length > 0) {
                        this.eventHub.publish('relatedProductsLoaded',
                            {
                                data: {
                                    ListName: this.getListNameForAnalytics(),
                                    Products: vm.Products
                                }
                            });
                    }
                })
                .then((vm: any) => this.onGetRelatedProductsSuccess(vm),
                    (reason: any) => this.onGetRelatedProductsFailed(reason));
        }

        protected onGetRelatedProductsSuccess(vm: any): void {
            //Hook for other projects
        }

        protected onGetRelatedProductsFailed(reason: any): void {
            console.error('Failed loading the related products', reason);
        }

        protected getPageSource(): string {
            return this.source;
        }

        protected getListNameForAnalytics(): string {
            return this.source;
        }

        protected onLoadingFailed(reason: any) {
            console.error('Failed loading the Related Product View');

            ErrorHandler.instance().outputErrorFromCode('RelatedProductLoadFailed');
        }
    }
}
