///<reference path='../Product/ProductController.ts' />
///<reference path='../ProductEvents.ts' />
///<reference path='../../Composer.Cart/RecurringOrder/Repositories/RecurringOrderRepository.ts' />
/// <reference path='../../JQueryPlugins/IPopOverJqueryPlugin.ts' />

module Orckestra.Composer {

    enum RecurringMode  {
        Single = 'Single',
        Recurring = 'Recurring'
    }

    export class ProductDetailController extends Orckestra.Composer.ProductController {

        protected concern: string = 'productDetail';

        private selectedRecurringOrderFrequencyName: string;
        private recurringMode: RecurringMode;

        public initialize() {

            super.initialize();
            this.initKvaSelectVueComponent();

            this.productService.updateSelectedKvasWith(this.context.viewModel.selectedKvas, this.concern);

            var priceDisplayBusy: UIBusyHandle = this.asyncBusy({
                msDelay: 300,
                loadingIndicatorSelector: '.loading-indicator-pricediscount'
            });

            Q.when(this.calculatePrice()).done(() => {
                priceDisplayBusy.done();
                this.notifyAnalyticsOfProductDetailsImpression();
            });

            let $recurringOrderContainer = this.context.container.find('[data-recurring-mode]');
            this.recurringMode = $recurringOrderContainer.data('recurring-mode');
            this.selectedRecurringOrderFrequencyName = $recurringOrderContainer.data('recurring-order-frequency');

            let { Sku } = this.context.viewModel;
            var availableToSellPromise = this.inventoryService.isAvailableToSell(Sku);
            let getCartPromise = this.cartService.getFreshCart();
            let authenticatedPromise = this._membershipService.isAuthenticated();
            let getWishListPromise = this._wishListService.getWishListSummary();
            Q.all([availableToSellPromise, getCartPromise, authenticatedPromise, getWishListPromise])
                .spread((isAvailableToSell, cartVm, authVm, wishListVm) => {
                    this.initAddToCartWithQtyInCartVueComponent(isAvailableToSell, cartVm, authVm);
                    this.initAddToCartWithQtyVueComponent(isAvailableToSell, cartVm, authVm);
                    this.initAddToWishListVueComponent(authVm, wishListVm);
                })

        }

        protected initAddToCartWithQtyInCartVueComponent(isAvailableToSell, cartVm, authVm) {
            let elId = 'vueAddToCartWithQuantityInCart';
            let el = document.getElementById(elId);
            if(!el) return;
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
                    onSelectedVariantIdChanged(result) {
                        let { selectedSku } = result.data;
                        self.inventoryService.isAvailableToSell(selectedSku)
                            .then(isAvailableToSell => this.IsAvailableToSell = isAvailableToSell);
                    },
                    addItemToCart(event: JQueryEventObject) {
                        if (this.Loading) return;

                        this.Loading = true;
                       
                        let { FrequencyName } = self.getRecurringData();
                        let { selectedVariantId, ListPrice } = this.Product;

                        self.cartService.addLineItem(this.Product, ListPrice, selectedVariantId,  1,
                            this.concern, FrequencyName)
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

                        if(this.Cart.QuantityRange) {
                            const {Min, Max} = this.Cart.QuantityRange;
                            quantity = Math.min(Math.max(Min, quantity), Max);
                        }

                        if(quantity == this.Quantity) {
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
            if(!el) return;
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
                        let { FrequencyName } = self.getRecurringData();
                        let { selectedVariantId, ListPrice } = this.Product;

                        self.cartService.addLineItem(this.Product, selectedVariantId, ListPrice, this.Quantity,
                            this.getListNameForAnalytics(), FrequencyName)
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
            if(!el) return;
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

        protected initKvaSelectVueComponent() {
            let elId = 'vueKvaList';
            let el = document.getElementById(elId);
            if(!el) return;
            let self: ProductDetailController = this;
            $('[data-toggle="popover"]').popover({
                placement:"top"
            });
            new Vue({
                el: `#${elId}`,
                data: {
                   dataUpdatedTracker: 1
                },
                computed: {
                    KvaAttributeItems() {
                        return this.dataUpdatedTracker && self.context.viewModel.keyVariantAttributeItems;
                    }
                },
                mounted() {
                    self.eventHub.subscribe(self.concern + 'SelectedVariantIdChanged', this.onSelectedVariantIdChanged);
                },
                methods: {
                    onSelectedVariantIdChanged(result) {
                        this.dataUpdatedTracker += 1;
                    },
                    KvaColorStyle(value) {
                        var colorStyle = value.ConfiguredValue ? {"background": value.ConfiguredValue} :  {"background": value.Value};
                        return colorStyle;
                    },
                    onMouseover(event: MouseEvent) {
                        let target = $(event.target);
                        $(target).popover('show');
                    },
                    onMouseleave(event: MouseEvent) {
                        let target = $(event.target);
                        $(target).popover('hide');
                    },
                    changeKva(event: JQueryEventObject) {
                        //target is kva-color (outter div of the color swatch)
                        var isColor = event.target.classList.contains("kva-color");
                        //target is kva-property (outter 'property' button)
                        var isProperty = event.target.classList.contains("kva-property");

                        let target = event.target;

                        if(isColor) {
                            target = event.target.getElementsByClassName('kva-color-value')[0];
                        } else if(isProperty) {
                            target = event.target.getElementsByClassName('kva-property-value')[0];
                        }
                        // we don't accept clicks if the button is disabled
                        if (target.parentElement.classList.contains("disabled")) return;
                        
                        // set element value in jquery for the parent ProductController's use
                        $(target).val($(target.parentElement).attr('value'));
                        self.selectKva({elementContext: $(target), event: event});
                    },
                }
            });
        }

        protected getListNameForAnalytics(): string {
            return 'Detail';
        }

        protected notifyAnalyticsOfProductDetailsImpression() {
            var product = this.context.viewModel;
            product.Variants = product.allVariants;
            var data: any = ProductsHelper.getProductDataForAnalytics(product, product.selectedVariantId, product.ListPrice, this.getListNameForAnalytics());

            this.publishProductImpressionEvent(data);
        }

        protected publishProductImpressionEvent(data: any) {
            this.eventHub.publish('productDetailsRendered', { data });
        }

        protected onSelectedVariantIdChanged(e: IEventInformation) {
            let varId = e.data.selectedVariantId || e.data.displayVariantId || 'unavailable';
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

        protected onPricesChanged(e: IEventInformation) {
            let vm = this.isProductWithVariants() && this.isSelectedVariantUnavailable() ? null : e.data;
            this.render('PriceDiscount', vm);
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
            } else  if (prevVariantIdIndex === -1) {
                //We couldn't find the variant id in the path, which means the PDP was accessed without a variant in the URL.
                //In that case, we add it right after the product id in the URL. If for some aweful reason the product id is not found,
                //add the variant id at the end.
                let productIdIndex = pathArray.indexOf(this.context.viewModel.productId);
                pathArray.splice(productIdIndex === -1 ? pathArray.length : productIdIndex + 1, 0, variantId);
            } else {
                //Replace the old variant id with the new one
                pathArray[prevVariantIdIndex] = variantId;
            }

            let {protocol, host} = window.location;
            let builtPath = `${protocol}//${host}${this.productService.buildUrlPath(pathArray)}`;

            history.replaceState( {} , null, builtPath);
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
