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
                            return results;
                        }
                    },
                    methods: {
                        onCartUpdated(cart) {
                            this.Cart = cart.data;
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
                            const { HasVariants, ProductId, VariantId, Price, RecurringOrderProgramName } = product;

                            this.Loading = true;
                            event.target.disabled = true;

                            var promise: Q.Promise<any>;

                            if (HasVariants) {
                                promise = self.addVariantProductToCart(ProductId, VariantId, Price);
                            } else {
                                promise = self.addNonVariantProductToCart(ProductId, Price, RecurringOrderProgramName);
                            }

                            promise.fin(() => {
                                event.target.disabled = false;
                                this.Loading = false;
                            });
                        }
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
            return 'Related Products';
        }

        protected getListNameForAnalytics(): string {
            return 'Related Products';
        }

        protected onLoadingFailed(reason: any) {
            console.error('Failed loading the Related Product View');

            ErrorHandler.instance().outputErrorFromCode('RelatedProductLoadFailed');
        }

        /**
         * Occurs when adding a product to the cart that happens to have variants.
         */
        protected addVariantProductToCart(productId: string, variantId: string, price: string): Q.Promise<any> {
            var promise = this.productService.loadQuickBuyProduct(productId, variantId, this.concern, this.source);
            promise.fail((reason: any) => this.onLoadingFailed(reason));

            return promise;
        }

        /**
         * Occurs when adding a product to the cart that has no variant.
         */
        protected addNonVariantProductToCart(productId: string, price: string, recurringProgramName: string): Q.Promise<any> {
            var vm = this.getProductViewModel(productId);
            if (vm) {
                var quantity = this.getCurrentQuantity();
                var data: any = this.getProductDataForAnalytics(vm);
                data.Quantity = quantity.Value ? quantity.Value : 1;

                this.eventHub.publish(ProductEvents.LineItemAdding,
                {
                    data: data
                });
            }

            var promise = this.cartService.addLineItem(productId, price, null, 1, null, recurringProgramName)
                .then((vm: any) => this.onAddLineItemSuccess(vm),
                    (reason: any) => this.onAddLineItemFailed(reason));

            return promise;
        }

        protected getProductViewModel(productId: string): any {
            var productVM = _.find(this.products, p => {
                return p.ProductId === productId;
            });

            if (!productVM) {
                console.warn(`Could not find the product with ID of ${productId} within related products.
                    This will cause the product to not be reported to Analytics.`);
            }

            return productVM;
        }

        protected getCurrentQuantity(): any {
            return {
                Min: 1,
                Max: 1,
                Value: 1
            };
        }
    }
}
