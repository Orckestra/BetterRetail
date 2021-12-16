///<reference path='../../Typings/tsd.d.ts' />
///<reference path='../Services/AutocompleteSearchService.ts' />
/// <reference path='../Events/EventHub.ts' />
/// <reference path='./SearchBoxController.ts' />
///<reference path='../Mvc/ComposerClient.ts' />
///<reference path='../Composer.Cart/CartSummary/CartService.ts' />
///<reference path='../Composer.Product/Product/ProductService.ts' />
///<reference path='../Composer.Product/ProductEvents.ts' />
///<reference path='../ErrorHandling/ErrorHandler.ts' />

module Orckestra.Composer {
    interface VueAutocompleteData {
        sectionConfigs: any;
        debounceMilliseconds: number;
        query: string; suggestions: any[];
        results: any[];
        timeout: null;
        selected: any;
        Cart: object;
        Loading: boolean;
    }

    export class AutocompleteSearchBoxVueController extends SearchBoxController {
        private searchService: AutocompleteSearchService;
        public VueAutocomplete: Vue;

        protected concern: string = 'autocompleteSearchBoxVue';
        private source: string = 'Autosuggestion Search Box';
        protected cartService: ICartService = CartService.getInstance();
        protected productService: ProductService = new ProductService(this.eventHub, this.context);

        public initialize() {
            super.initialize();
            this.initializeVue();

            this.searchService = new AutocompleteSearchService(EventHub.instance(), window);
            this.searchService.initialize({
                correctedSearchTerm: '',
                facetRegistry: {}
            });

            this.searchService['_baseSearchUrl'] = document.getElementById("frm-search-box").getAttribute('action');
        }

        public initializeVue() {
            const self = this;
            this.VueAutocomplete = new Vue({
                el: '#vueAutocomplete',
                components: {
                    VueAutosuggest: VueAutosuggest.VueAutosuggest
                },
                data(): VueAutocompleteData {
                    return {
                        query: "",
                        results: [],
                        timeout: null,
                        selected: null,
                        debounceMilliseconds: 500,
                        suggestions: [],
                        sectionConfigs: {
                            autocomplete: {
                                active: true,
                                onSelected: function (selected) {
                                    this.selected = selected.item;
                                }.bind(this),
                                //label: "suggestbrands",
                                type: "default-section",
                                ulClass: "row autosuggest-top-results",
                                liClass: {
                                    "col-12": true,
                                }
                            },
                            suggestcategories: {
                                onSelected: function (selected) {
                                    this.selectedCategorySuggestion(selected);
                                    this.selected = selected.item;
                                }.bind(this),
                            },
                            suggestbrands: {
                                onSelected: function (selected) {
                                    this.selectedBrandSuggestion(selected);
                                    this.selected = selected.item;
                                }.bind(this),
                            },
                            suggestterms: {
                                onSelected: function (selected) {
                                    this.selected = selected.item;
                                    this.selectedSearchTermsSuggestion(selected);
                                }.bind(this),
                            },
                            default: {
                                onSelected: () => { },
                            }
                        },
                        Cart: undefined,
                        Loading: false
                    };
                },
                mounted() {
                    this.query = this.$el.attributes.keywords.value;
                    this.sectionConfigs.autocomplete.limit = +this.$el.attributes['autocomplete-limit'].value;
                    this.sectionConfigs.suggestcategories.limit = +this.$el.attributes['categories-limit'].value;
                    this.sectionConfigs.suggestbrands.limit = +this.$el.attributes['brand-limit'].value;
                    this.sectionConfigs.suggestterms.limit = +this.$el.attributes['search-terms-limit'].value;

                    this.sectionConfigs.suggestcategories.active = !!this.$el.attributes['categories-enable'];
                    this.sectionConfigs.suggestbrands.active = !!this.$el.attributes['brands-enable'];
                    this.sectionConfigs.suggestterms.active = !!this.$el.attributes['search-terms-enable'];

                    this.sectionConfigs.suggestcategories.displayCategoryPage = !!this.$el.attributes['category-suggestions-as-pages'];

                    this.minSearchSize = +this.$el.attributes['min-search-size'].value;

                    if (this.query) {
                        this.fetchResults(this.query);
                    }

                    const input = document.getElementById('autosuggest__input');
                    input.addEventListener('keydown', (event) => {
                        if (event.code === 'Enter') {
                            this.searchMore()
                        }
                    });

                    self.eventHub.subscribe(CartEvents.CartUpdated, (result) => this.onCartUpdated(result.data));
                    self.cartService.getCart().then(this.onCartUpdated)
                },
                updated() {
                },
                computed: {
                    isEmptyRight() {
                        return this.suggestions.length === 1;
                    }
                },
                methods: {
                    fetchResults(result) {
                        const query = this.query;
                        if (query.length < this.minSearchSize)
                            return;

                        clearTimeout(this.timeout);
                        this.timeout = setTimeout(() => {
                            const sectionNames = Object.keys(this.sectionConfigs).filter(name => this.sectionConfigs[name].active);

                            const results = sectionNames.map(sectionName => {
                                const limit = this.sectionConfigs[sectionName].limit;
                                let apiPath = `/api/search/${sectionName}?limit=${limit}`;
                                if (sectionName === 'suggestcategories' && this.sectionConfigs.suggestcategories.displayCategoryPage) {
                                    apiPath = `${apiPath}&withCategoriesUrl=${this.sectionConfigs.suggestcategories.displayCategoryPage}`;
                                }
                                return ComposerClient.post(apiPath, { Query: query }).catch(() => []);
                            });

                            Q.all(results).then(values => {
                                this.selected = null;

                                this.suggestions = sectionNames
                                    .map(this.mapSections(values, query))
                                    .filter(({ data, name }) => data.length || name === 'autocomplete');
                            });
                        }, this.debounceMilliseconds);
                    },
                    highlightSuggestion(value, query) {
                        const start = value.toLowerCase().indexOf(query.toLowerCase());
                        const end = start + query.length;
                        if (start < 0) return value;

                        return [
                            value.slice(0, start),
                            `<strong>${value.slice(start, end)}</strong>`,
                            value.slice(end),
                        ].join('');
                    },
                    mapSections(values, query) {
                        return (sectionName, index) => ({
                            name: sectionName,
                            data: this.mapSuggestions(values[index].Suggestions, sectionName, query)
                        });
                    },
                    mapSuggestions(suggestions = [], sectionName, query) {
                        return suggestions.map((suggest) => {
                            const title = sectionName === 'suggestcategories' ? [...suggest.Parents, suggest.DisplayName].join(' > ') : suggest.DisplayName;
                            const data = sectionName === 'autocomplete' ? this.extendProductItem(suggest) : suggest;

                            return ({ ...data, mappedDisplayName: this.highlightSuggestion(title, query) })
                        })
                    },
                    getSuggestionValue(suggestion) {
                        let { name, item } = suggestion;
                        return item.DisplayName;
                    },
                    shouldRenderSuggestions(size, loading) {
                        return this.query.length >= this.minSearchSize && !loading && this.suggestions.length
                    },
                    searchMore() {
                        const elem = document.getElementById("frm-search-box") as HTMLFormElement;
                        elem.submit();
                    },
                    selectedSearchTermsSuggestion(suggestion) {
                        //this.query = suggestion.item.DisplayName;
                        this.searchMore();
                    },
                    selectedCategorySuggestion(suggestion) {
                        EventHub.instance().publish('categorySuggestionClicked', {
                            data: {
                                suggestion: suggestion.item.DisplayName,
                                parents: suggestion.item.Parents,
                                url: suggestion.item.Url
                            }
                        });
                    },
                    selectedBrandSuggestion(suggestion) {
                        EventHub.instance().publish('brandSuggestionClicked', {
                            data: { suggestion: suggestion.item.DisplayName }
                        });
                    },
                    onImageError(e, suggestion) {
                        const img = suggestion.item.FallbackImageUrl;
                        if (img) {
                            e.target.onerror = null;
                            e.target.src = img;
                        }
                    },
                    onOpened() {
                        document.body.classList.add("modal-open");
                    },
                    onClosed() {
                        document.body.classList.remove("modal-open");
                    },
                    addToCart: this.addToCart.bind(this),
                    onCartUpdated(result) {
                        this.Cart = result;
                        const section = this.suggestions.find(section => section.name === 'autocomplete');
                        if (section) {
                            section.data = section.data.map(this.extendProductItem)
                        }
                    },
                    extendProductItem(product) {
                        let cartItem = this.Cart && this.Cart.LineItemDetailViewModels &&
                            this.Cart.LineItemDetailViewModels.find((i: any) => i.ProductId === product.ProductId && i.VariantId == product.VariantId);
                        product.InCart = !!cartItem;
                        product.LineItemId = cartItem && cartItem.Id;
                        product.Quantity = cartItem && cartItem.Quantity || 0;

                        return product;
                    },
                    updateItemQuantity(item: any, quantity: number) {
                        let cartItem = this.Cart.LineItemDetailViewModels && this.Cart.LineItemDetailViewModels.find((i: any) => i.Id === item.LineItemId);

                        if (this.Loading || !cartItem) return;

                        if (this.Cart.QuantityRange) {
                            const { Min, Max } = this.Cart.QuantityRange;
                            quantity = Math.min(Math.max(Min, quantity), Max);
                        }

                        if (quantity == cartItem.Quantity) {
                            //force update vue component
                            this.onCartUpdated({ ...this.Cart });
                            return;
                        }

                        let analyticEventName = quantity > cartItem.Quantity ? ProductEvents.LineItemAdding : ProductEvents.LineItemRemoving;
                        cartItem.Quantity = quantity;

                        if (cartItem.Quantity < 1) {
                            this.Loading = true; // disabling UI immediately when a line item is removed
                        }

                        self.publishDataForAnalytics(item, cartItem.Quantity, analyticEventName);

                        this.onCartUpdated(this.Cart);

                        if (!this.debounceUpdateItem) {
                            this.debounceUpdateItem = _.debounce(({ Id, Quantity, ProductId }) => {
                                this.Loading = true;
                                this.UpdatingProductId = ProductId;
                                let updatePromise = Quantity > 0 ?
                                    self.cartService.updateLineItem(Id, Quantity, ProductId) :
                                    self.cartService.deleteLineItem(Id, ProductId);

                                updatePromise
                                    .then(() => {
                                        ErrorHandler.instance().removeErrors();
                                    }, (reason: any) => {
                                        self.onAddToCartFailed(reason);
                                        throw reason;
                                    })
                                    .fin(() => this.Loading = false);

                            }, 400);
                        }

                        this.debounceUpdateItem(cartItem);
                    },
                }
            });
        }

        protected onAddToCartFailed(reason: any): void {
            console.error('Error on adding item to cart', reason);

            ErrorHandler.instance().outputErrorFromCode('AddToCartFailed');
        }

        public addToCart(event, product) {
            const { HasVariants, ProductId, VariantId, Price, RecurringOrderProgramName } = product;

            let promise: Q.Promise<any>;
            product.Loading = true;
            event.target.disabled = true;

            if (HasVariants) {
                promise = this.addVariantProductToCart(ProductId, VariantId);
            } else {
                this.publishDataForAnalytics(product, 1, ProductEvents.LineItemAdding);
                promise = this.addNonVariantProductToCart(ProductId, Price, RecurringOrderProgramName);
            }

            promise.fin(() => {
                event.target.disabled = false;
                product.Loading = false;
            });
        }

        /**
         * Occurs when adding a product to the cart that happens to have variants.
         */
        protected addVariantProductToCart(productId: string, variantId: string): Q.Promise<any> {
            const promise = this.productService.loadQuickBuyProduct(productId, variantId, this.concern, this.source);
            promise.fail(this.onAddToCartFailed);

            return promise;
        }

        /**
         * Occurs when adding a product to the cart that has no variant.
         */
        protected addNonVariantProductToCart(productId: string, price: string, recurringProgramName: string): Q.Promise<any> {
            const promise = this.cartService.addLineItem(productId, price, null, 1, null, recurringProgramName);
            promise.fail(this.onAddToCartFailed);

            return promise;
        }

        protected publishDataForAnalytics(product, quantity: number, eventName: string) {
            const data: any = this.getProductDataForAnalytics(product, quantity);
            this.eventHub.publish(eventName, { data });
        }

        protected getProductDataForAnalytics(vm: any, quantity: number): any {
            return {
                List: this.source,
                ProductId: vm.ProductId,
                DisplayName: vm.DisplayName,
                ListPrice: vm.ListPrice,
                Brand: vm.Brand,
                CategoryId: vm.CategoryId,
                Quantity: quantity
            };
        }
    }
}
