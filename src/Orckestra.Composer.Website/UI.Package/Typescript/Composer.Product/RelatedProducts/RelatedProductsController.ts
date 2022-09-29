/// <reference path='../Product/ProductController.ts' />
///<reference path='../../Plugins/SlickCarouselPlugin.ts' />
///<reference path='../ProductEvents.ts' />
module Orckestra.Composer {
    export class RelatedProductController extends Orckestra.Composer.ProductController  {

        protected concern: string = 'relatedProduct';
        private source: string = 'Related Products';
        protected VueRelatedProducts: Vue;

        private products: any[];

        public initialize() {

            super.initialize();
            let self: RelatedProductController = this;
            let vm = self.context.viewModel;
            let relatedProductsPromise = self.getRelatedProducts();
            let getCartPromise = self.cartService.getCart();
            Q.all([relatedProductsPromise, getCartPromise])
            .spread((relatedproducts, cart) => {
                self.VueRelatedProducts = new Vue({
                    el: '#vueRelatedProducts',
                    data: {
                        RelatedProducts: self.products,
                        ProductIdentifiers: vm.ProductIdentifiers,
                        Loading: false,
                        Cart: cart,
                        ProductsMap: {},
                        dataUpdatedTracker: 1,
                    },
                    mounted() {
                        self.eventHub.subscribe(CartEvents.CartUpdated, this.onCartUpdated);
                        self.eventHub.publish('iniCarousel', null);
                    },
                    computed: {
                        ExtendedRelatedProducts() {
                            const results = _.map(this.RelatedProducts, (product: any) => {
                                const isSameProduct = (i: any) => i.ProductId === product.ProductId && i.VariantId == product.VariantId;
                                let cartItem = this.Cart && this.Cart.LineItemDetailViewModels.find(isSameProduct);

                                product.InCart = !!cartItem;
                                product.Quantity = cartItem ? cartItem.Quantity : 0;
                                product.LineItemId = cartItem ? cartItem.Id : undefined;

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
                            if (!HasVariants || this.ProductsMap[ProductId]) return;

                            this.loadingProduct(relatedProduct, true);
                            self.productService.loadProduct(ProductId, VariantId)
                                .then(product => {
                                    product.SelectedVariant = product.Variants.find(v => v.Id === VariantId);
                                    product.SizeSelected = false;
                                    this.ProductsMap[ProductId] = product;
                                })
                                .fin(() => this.loadingProduct(relatedProduct, false));
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
                                .then(self.onAddLineItemSuccess, this.onAddToCartFailed)
                                .fin(() => this.loadingProduct(product, false));
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
