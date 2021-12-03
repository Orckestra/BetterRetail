///<reference path='../Product/ProductController.ts' />
///<reference path='../ProductEvents.ts' />
///<reference path='../../Composer.Cart/RecurringOrder/Repositories/RecurringOrderRepository.ts' />
///<reference path='../../Utils/PriceHelper.ts' />
///<reference path='../../Composer.Grocery/FulfillmentEvents.ts' />

module Orckestra.Composer {

    enum RecurringMode {
        Single = 'Single',
        Recurring = 'Recurring'
    }

    export class ProductDetailController extends Orckestra.Composer.ProductController {

        protected concern: string = 'productDetail';

        private selectedRecurringOrderFrequencyName: string;
        private recurringMode: RecurringMode;

        public initialize() {

            super.initialize();

            this.productService.updateSelectedKvasWith(this.context.viewModel.selectedKvas, this.concern);

            let $recurringOrderContainer = this.context.container.find('[data-recurring-mode]');
            this.recurringMode = $recurringOrderContainer.data('recurring-mode');
            this.selectedRecurringOrderFrequencyName = $recurringOrderContainer.data('recurring-order-frequency');

            let { Sku } = this.context.viewModel;
            var availableToSellPromise = this.inventoryService.isAvailableToSell(Sku);
            let getCartPromise = this.cartService.getFreshCart();
            let authenticatedPromise = this._membershipService.isAuthenticated();
            let getWishListPromise = this._wishListService.getWishListSummary();
            let getPricePromise = this.calculatePrice();
            Q.all([availableToSellPromise, getCartPromise, authenticatedPromise, getWishListPromise, getPricePromise])
                .spread((isAvailableToSell, cartVm, authVm, wishListVm, priceVm) => {
                    //As KvaItems rendering for now Handelbar based, we can't have one Vue Component for the whole Product Page
                    // as Hanelbar rendering does not work inside Vue Component
                    //Need to rewrite KvaItems rendering, then we can have one Vue component
                    // For now we initialize separate components for each element 
                    this.initAddToCartWithQtyInCartVueComponent(isAvailableToSell, cartVm, authVm);
                    this.initAddToCartWithQtyVueComponent(isAvailableToSell, cartVm, authVm);
                    this.initAddToWishListVueComponent(authVm, wishListVm);
                    this.initProductPriceVueComponent();
                    this.notifyAnalyticsOfProductDetailsImpression();
                })

        }

        protected initAddToCartWithQtyInCartVueComponent(isAvailableToSell, cartVm, authVm) {
            let elId = 'vueAddToCartWithQuantityInCart';
            let el = document.getElementById(elId);
            if (!el) return;
            let product = this.context.viewModel;
            let self: ProductDetailController = this;
            let addToCartWithQuantity = new Vue({
                el: '#' + elId,
                data: {
                    IsAuthenticated: authVm.IsAuthenticated,
                    Cart: cartVm,
                    Product: product,
                    IsUnavailable: false,
                    IsAvailableToSell: isAvailableToSell,
                    Loading: false
                },
                mounted() {
                    self.eventHub.subscribe(CartEvents.CartUpdated, this.onCartUpdated);
                    self.eventHub.subscribe(FulfillmentEvents.StoreSelected, this.onStoreSelected);
                    self.eventHub.subscribe(self.concern + 'SelectedVariantIdChanged', this.onSelectedVariantIdChanged);
                },
                computed: {
                    CartItem() {
                        if (!this.Cart) return null;
                        var item = _.find(this.Cart.LineItemDetailViewModels, (i: any) =>
                            i.ProductId === this.Product.productId && i.VariantId == this.Product.selectedVariantId);
                        return item;
                    },
                    Quantity() {
                        return this.CartItem ? this.CartItem.Quantity : 0;
                    },
                    IsUnavailableVariant() {
                        return $.isArray(this.Product.allVariants) && !this.Product.selectedVariantId;
                    },
                    DecrementDisabled() {
                        return !this.CartItem || this.Loading || (this.Cart.QuantityRange && this.CartItem.Quantity <= this.Cart.QuantityRange.Min);
                    },
                    IncrementDisabled() {
                        return !this.CartItem || this.Loading || (this.Cart.QuantityRange && this.CartItem.Quantity >= this.Cart.QuantityRange.Max);
                    },
                    AddToCartDisabled() {
                        return this.Loading ||
                            !this.Product.DefaultListPrice ||
                            !this.IsAvailableToSell ||
                            this.IsUnavailableVariant ||
                            (!this.IsAuthenticated && self.recurringMode === RecurringMode.Recurring);
                    }

                },
                methods: {
                    onCartUpdated(result) {
                        this.Cart = result.data;
                    },
                    onStoreSelected() {
                        self.inventoryService.clearCache();
                        self.inventoryService.isAvailableToSell(this.Product.Sku).then(result => {
                            this.IsAvailableToSell = result;
                        });
                    },
                    onSelectedVariantIdChanged(result) {
                        let { selectedSku } = result.data;
                        self.inventoryService.isAvailableToSell(selectedSku)
                            .then(isAvailableToSell => this.IsAvailableToSell = isAvailableToSell);
                    },
                    addItemToCart(event: JQueryEventObject) {
                        if (this.Loading) return;

                        this.Loading = true;
                        self.publishProductDataForAnalytics(self.context.viewModel, ProductEvents.LineItemAdding);

                        let { FrequencyName, RecurringProgramName } = self.getRecurringData();
                        let { ProductId, selectedVariantId, ListPrice } = this.Product;

                        self.cartService.addLineItem(ProductId, ListPrice, selectedVariantId, 1,
                            FrequencyName, RecurringProgramName)
                            .then(() => {
                                self.onAddLineItemSuccess();
                            }, (reason: any) => {
                                self.onAddLineItemFailed(reason);
                                throw reason;
                            })
                            .fin(() => this.Loading = false);

                    },
                    updateItemQuantity(quantity: number) {
                        if (this.Loading || !this.CartItem) return;

                        if (this.Cart.QuantityRange) {
                            const { Min, Max } = this.Cart.QuantityRange;
                            quantity = Math.min(Math.max(Min, quantity), Max);
                        }

                        if (quantity == this.Quantity) {
                            //force update vue component
                            this.Cart = { ...this.Cart };
                            return;
                        }

                        let analyticEventName = quantity > this.Quantity ? ProductEvents.LineItemAdding : ProductEvents.LineItemRemoving;
                        this.CartItem.Quantity = quantity;

                        if (this.Quantity < 1) {
                            this.Loading = true; // disable ui immediately when we will delete  the line item
                        }

                        self.publishProductDataForAnalytics(self.context.viewModel, analyticEventName);

                        let { FrequencyName, RecurringProgramName } = self.getRecurringData();
                        let { ProductId, selectedVariantId } = this.Product;

                        if (!this.debounceUpdateItem) {
                            this.debounceUpdateItem = _.debounce(() => {

                                this.Loading = true;
                                let updatePromise = this.Quantity > 0 ?
                                    self.cartService.updateLineItem(this.CartItem.Id, this.Quantity,
                                        ProductId, FrequencyName, RecurringProgramName) :
                                    self.cartService.deleteLineItem(this.CartItem.Id, ProductId);

                                updatePromise
                                    .then((cart: any) => {
                                        self.onAddLineItemSuccess({ Quantity: this.Quantity, Cart: cart, ProductId, selectedVariantId });
                                    }, (reason: any) => {
                                        self.onAddLineItemFailed(reason);
                                        throw reason;
                                    })
                                    .fin(() => this.Loading = false);

                            }, 400);
                        }

                        this.debounceUpdateItem();
                    }
                }
            })
        }

        protected initAddToCartWithQtyVueComponent(isAvailableToSell, cartVm, authVm) {
            let elId = 'vueAddToCartWithQuantity';
            let el = document.getElementById(elId);
            if (!el) return;
            let product = this.context.viewModel;
            let self: ProductDetailController = this;
            let addToCartWithQuantity = new Vue({
                el: '#' + elId,
                data: {
                    IsAuthenticated: authVm.IsAuthenticated,
                    Product: product,
                    Cart: cartVm,
                    Quantity: cartVm.QuantityRange.Min,
                    IsUnavailable: false,
                    IsAvailableToSell: isAvailableToSell,
                    Loading: false
                },
                mounted() {
                    self.eventHub.subscribe(self.concern + 'SelectedVariantIdChanged', this.onSelectedVariantIdChanged);
                },
                computed: {
                    IsUnavailableVariant() {
                        return $.isArray(this.Product.allVariants) && !this.Product.selectedVariantId;
                    },
                    DecrementDisabled() {
                        return this.Loading || (this.Cart.QuantityRange && this.Quantity <= this.Cart.QuantityRange.Min);
                    },
                    IncrementDisabled() {
                        return this.Loading || (this.Cart.QuantityRange && this.Quantity >= this.Cart.QuantityRange.Max);
                    },
                    AddToCartDisabled() {
                        return this.Loading ||
                            !this.Product.DefaultListPrice ||
                            !this.IsAvailableToSell ||
                            this.IsUnavailableVariant ||
                            (!this.IsAuthenticated && self.recurringMode === RecurringMode.Recurring);
                    }
                },
                methods: {
                    onSelectedVariantIdChanged(result) {
                        let { selectedSku } = result.data;
                        self.inventoryService.isAvailableToSell(selectedSku)
                            .then(isAvailableToSell => this.IsAvailableToSell = isAvailableToSell);
                    },
                    addItemToCart(event: JQueryEventObject) {
                        if (this.Loading) return;

                        this.Loading = true;
                        self.publishProductDataForAnalytics(self.context.viewModel, ProductEvents.LineItemAdding);

                        let { FrequencyName, RecurringProgramName } = self.getRecurringData();
                        let { ProductId, selectedVariantId, ListPrice } = this.Product;

                        self.cartService.addLineItem(ProductId, ListPrice, selectedVariantId, this.Quantity,
                            FrequencyName, RecurringProgramName)
                            .then(() => {
                                self.onAddLineItemSuccess();
                            }, (reason: any) => {
                                self.onAddLineItemFailed(reason);
                                throw reason;
                            })
                            .fin(() => {
                                this.Loading = false;
                                this.Quantity = this.Cart.QuantityRange.Min;
                            });

                    },
                    updateQuantity(quantity: any) {
                        if (this.Cart.QuantityRange) {
                            const { Min, Max } = this.Cart.QuantityRange;
                            this.Quantity = Math.min(Math.max(Min, quantity), Max);
                        }
                    }
                }
            })
        }

        protected initAddToWishListVueComponent(authVm, wishListVm) {
            let elId = 'vueAddProductToWishList';
            let el = document.getElementById(elId);
            if (!el) return;
            let product = this.context.viewModel;
            let self: ProductDetailController = this;
            let vueWishList = new Vue({
                el: '#' + elId,
                data: {
                    IsAuthenticated: authVm.IsAuthenticated,
                    Product: product,
                    WishList: wishListVm,
                    Loading: false
                },
                mounted() {
                    self.eventHub.subscribe(ProductEvents.WishListUpdated, this.onWishListUpdated);
                },
                computed: {
                    WishListItem() {
                        return _.find(this.WishList.Items, (i: any) =>
                            i.ProductId === this.Product.productId && i.VariantId == this.Product.selectedVariantId);
                    },
                    IsUnavailableVariant() {
                        return $.isArray(this.Product.allVariants) && !this.Product.selectedVariantId;
                    }
                },
                methods: {
                    onWishListUpdated(result) {
                        this.WishList = result.data;
                    },
                    addLineItemToWishList() {
                        if (this.Loading) return;

                        if (!this.IsAuthenticated) {
                            return self.redirectToSignInBeforeAddToWishList();
                        }

                        this.Loading = true;
                        let { DisplayName, ProductId, selectedVariantId, ListPrice, RecurringOrderProgramName } = this.Product;
                        self.eventHub.publish('wishListLineItemAdding', {
                            data: { DisplayName, ListPrice: ListPrice }
                        });

                        self._wishListService.addLineItem(ProductId, selectedVariantId, 1, null, RecurringOrderProgramName)
                            .fin(() => this.Loading = false);
                    },

                    removeLineItemFromWishList() {
                        if (this.Loading) return;

                        if (!this.IsAuthenticated) {
                            return self.redirectToSignInBeforeAddToWishList();
                        }

                        this.Loading = true;
                        self._wishListService.removeLineItem(this.WishListItem.Id)
                            .fin(() => this.Loading = false);
                    }
                }
            })
        }

        protected initProductPriceVueComponent() {
            let elId = 'vueProductPrice';
            let el = document.getElementById(elId);
            if (!el) return;
            let product = this.context.viewModel;
            let self: ProductDetailController = this;
            let vueProductPrice = new Vue({
                el: '#' + elId,
                data: {
                    Product: product
                },
                computed: {
                    PricePerUnit(){
                        let pricePerUnit = PriceHelper.PricePerUnit(this.Product.ListPrice,
                            this.Product.ProductUnitQuantity,
                            this.Product.ProductUnitSize,
                            this.Product.ConvertedVolumeMeasurement
                        );
                        return  pricePerUnit;
                    },
                    IsPricePerUnitZero(){
                        return PriceHelper.IsPricePerUnitZero(this.PricePerUnit);
                    },
                    IsUnavailableVariant() {
                        return $.isArray(this.Product.allVariants) && !this.Product.selectedVariantId;
                    }
                },
                methods: {
                }
            })
        }
        
        protected getListNameForAnalytics(): string {
            return 'Detail';
        }

        protected notifyAnalyticsOfProductDetailsImpression() {
            var vm = this.context.viewModel;
            var variant: any = _.find(vm.allVariants, (v: any) => v.Id === vm.selectedVariantId);

            var data: any = this.getProductDataForAnalytics(vm);
            if (variant) {
                var variantData: any = this.getVariantDataForAnalytics(variant);

                _.extend(data, variantData);
            }

            this.publishProductImpressionEvent(data);
        }

        protected publishProductImpressionEvent(data: any) {
            this.eventHub.publish('productDetailsRendered', { data });
        }

        protected onSelectedVariantIdChanged(e: IEventInformation) {

            let varId = e.data.selectedVariantId || 'unavailable';
            var all = $('[data-variant]');

            $.each(all, (index, el) => {
                let $el = $(el);
                var vIds = $el.data('variant').toString().split(',');
                if (vIds.indexOf(varId) >= 0) {
                    this.handleHiddenImages($el);
                    $el.removeClass('d-none');
                } else {
                    $el.addClass('d-none');
                }
            });
        }

        protected handleHiddenImages(el) {
            el.find('img').each((index, img) => {
                if (!$(img).attr('src')) {
                    $(img).attr('src', $(img).data('src'));
                }
            });
        }

        protected onSelectedKvasChanged(e: IEventInformation) {

            this.render('KvaItems', { KeyVariantAttributeItems: e.data });
        }

        public selectKva(actionContext: IControllerActionContext) {

            let currentSelectedVariantId = this.context.viewModel.selectedVariantId;

            super.selectKva(actionContext);

            //IE8 check
            if (history) {
                this.replaceHistory(currentSelectedVariantId);
            }
        }

        private replaceHistory(previousSelectedVariantId: string) {
            let variantId = this.context.viewModel.selectedVariantId;

            if (variantId === null && previousSelectedVariantId === null) {
                return;
            }

            let pathArray = window.location.pathname.split('/').filter(Boolean);

            let prevVariantIdIndex = pathArray.lastIndexOf(previousSelectedVariantId); //Variant id should be at the foremost right
            if (variantId === null) {
                if (prevVariantIdIndex !== -1) {
                    pathArray.splice(prevVariantIdIndex, 1);
                }
            } else if (prevVariantIdIndex === -1) {
                //We couldn't find the variant id in the path, which means the PDP was accessed without a variant in the URL.
                //In that case, we add it right after the product id in the URL. If for some aweful reason the product id is not found,
                //add the variant id at the end.
                let productIdIndex = pathArray.indexOf(this.context.viewModel.productId);
                pathArray.splice(productIdIndex === -1 ? pathArray.length : productIdIndex + 1, 0, variantId);
            } else {
                //Replace the old variant id with the new one
                pathArray[prevVariantIdIndex] = variantId;
            }

            let { protocol, host } = window.location;
            let builtPath = `${protocol}//${host}${this.productService.buildUrlPath(pathArray)}`;

            history.replaceState({}, null, builtPath);
        }

        public onRecurringOrderFrequencySelectChanged(actionContext: IControllerActionContext) {
            let element = <any>actionContext.elementContext[0],
                option = element.options[element.selectedIndex];

            if (option) {
                this.selectedRecurringOrderFrequencyName = option.value === '' ? null : option.value;
            }
        }

        public changeRecurringMode(actionContext: IControllerActionContext) {
            let container$ = actionContext.elementContext.closest('.js-recurringModes');
            container$.find('.js-recurringModeRow.selected').removeClass('selected');
            actionContext.elementContext.closest('.js-recurringModeRow').addClass('selected');
            $('.modeSelection').collapse('toggle');
            this.recurringMode = actionContext.elementContext.val();
        }

        protected getRecurringData(): any {

            let FrequencyName = this.recurringMode === RecurringMode.Single ? null : this.selectedRecurringOrderFrequencyName;
            let RecurringProgramName = this.context.viewModel.RecurringOrderProgramName;

            return { FrequencyName, RecurringProgramName };
        }
    }
}
