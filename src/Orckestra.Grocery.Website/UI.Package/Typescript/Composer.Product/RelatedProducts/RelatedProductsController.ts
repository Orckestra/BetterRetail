/// <reference path='../Product/ProductController.ts' />
///<reference path='../../Plugins/SlickCarouselPlugin.ts' />
///<reference path='../ProductEvents.ts' />
module Orckestra.Composer {
    export class RelatedProductController extends Orckestra.Composer.ProductController {

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
                            }
                        },
                        methods: {
                            onCartUpdated(cart) {
                                this.Cart = cart.data;
                            },
                            productClick(product, index) {
                                self.eventHub.publish(ProductEvents.ProductClick, {
                                    data: {
                                        Product: product,
                                        ListName: self.getListNameForAnalytics(),
                                        Index: index
                                    }
                                });
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
                                item.Quantity = quantity;

                                let analyticEventName = quantity > cartItem.Quantity ? ProductEvents.LineItemAdding : ProductEvents.LineItemRemoving;
                                cartItem.Quantity = quantity;

                                if (cartItem.Quantity < 1) {
                                    this.Loading = true; // disabling UI immediately when a line item is removed
                                }

                                let { ProductId, VariantId } = cartItem;
                                self.publishDataForAnalytics(ProductId, quantity, analyticEventName);

                                if (!this.debounceUpdateItem) {
                                    this.debounceUpdateItem = _.debounce(({ Id, Quantity, ProductId }) => {
                                        this.Loading = true;
                                        let updatePromise = Quantity > 0 ?
                                            self.cartService.updateLineItem(Id, Quantity, ProductId) :
                                            self.cartService.deleteLineItem(Id, ProductId);

                                        updatePromise
                                            .then(() => {
                                                self.onAddLineItemSuccess();
                                            }, (reason: any) => {
                                                self.onAddLineItemFailed(reason);
                                                throw reason;
                                            })
                                            .fin(() => this.Loading = false);

                                    }, 400);
                                }
                                this.debounceUpdateItem(cartItem);
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

            this.publishDataForAnalytics(productId, 1, ProductEvents.LineItemAdding);
            var promise = this.cartService.addLineItem(productId, price, null, 1, null, recurringProgramName)
                .then((vm: any) => this.onAddLineItemSuccess(vm),
                    (reason: any) => this.onAddLineItemFailed(reason));

            return promise;
        }

        protected publishDataForAnalytics(productId, quantity: number, eventName: string) {
            var vm = this.getProductViewModel(productId);
            if (vm) {
                var data: any = this.getProductDataForAnalytics(vm);
                data.Quantity = quantity;
                this.eventHub.publish(eventName, { data });
            }
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


    }
}
