///<reference path='../../Typings/tsd.d.ts' />
///<reference path='../App.ts' />
///<reference path='../Services/AutocompleteSearchService.ts' />

module Orckestra.Composer {
    export class AutocompleteSearchBoxController extends SearchBoxController {

        private renderedSuggestions;
        private searchService: AutocompleteSearchService;

        public initialize() {
            super.initialize();
            this.searchService = new AutocompleteSearchService(EventHub.instance(), window);
            this.searchService.initialize({
                correctedSearchTerm: '',
                facetRegistry: {}
            });

            this.searchService['_baseSearchUrl'] = $('#frm-search-box').attr('action');

            let products = this.GetBloodhoundInstance('products', 8, '/api/search/autocomplete');
            products.initialize();

            let searchTerms = this.GetBloodhoundInstance('searchTerms', 5, '/api/search/suggestTerms');
            searchTerms.initialize();

            let categories = this.GetBloodhoundInstance('categories', 4, '/api/search/suggestCategories');
            categories.initialize();

            let brands = this.GetBloodhoundInstance('brands', 3, '/api/search/suggestBrands');
            brands.initialize();

            $('#search-box .js-typeahead').typeahead({
                    minLength: 3,
                    highlight: true,
                    hint: true,
                    //async: true
                },
                {
                    name: 'Products',
                    display: 'DisplayName',
                    source: products.ttAdapter(),
                    templates: {
                        notFound: (<any>Orckestra.Composer).Templates['SearchSuggestionsEmpty'],
                        suggestion: (<any>Orckestra.Composer).Templates['SearchSuggestions']
                    }
                },
                {
                    name: 'SearchTerms',
                    display: 'DisplayName',
                    source: searchTerms.ttAdapter(),
                    templates: {
                        notFound: (<any>Orckestra.Composer).Templates['SearchTermsSuggestionsEmpty'],
                        suggestion: (<any>Orckestra.Composer).Templates['SearchTermsSuggestions']
                    }
                },
                {
                    name: 'Categories',
                    display: 'DisplayName',
                    source: categories.ttAdapter(),
                    templates: {
                        notFound: (<any>Orckestra.Composer).Templates['CategorySuggestionsEmpty'],
                        suggestion: (<any>Orckestra.Composer).Templates['CategorySuggestions']
                    }
                },
                {
                    name: 'Brands',
                    display: 'DisplayName',
                    source: brands.ttAdapter(),
                    templates: {
                        notFound: (<any>Orckestra.Composer).Templates['BrandSuggestionsEmpty'],
                        suggestion: (<any>Orckestra.Composer).Templates['BrandSuggestions']
                    }
                }
            ).on('typeahead:render', (evt, suggestions) => {
                //cache the rendered suggestion at the render complete
                this.renderedSuggestions = suggestions;

                if ($('.js-suggestion-empty').length === 4) {
                    $('.tt-menu').addClass('right-empty');
                } else {
                    $('.tt-menu').removeClass('right-empty');
                }

            }).on('typeahead:asyncreceive', (evt) => {
                if (this.renderedSuggestions === undefined) {
                    this.resultsNotFound(evt);
                }
            });

            $('.tt-dataset').wrapAll('<div class="suggestions-wrapper"></div>');
            $('.tt-menu .tt-dataset:not(:first)').wrapAll('<div class="suggestion-right-col"></div>');
        }

        private GetBloodhoundInstance (name, limit, url): Bloodhound<any> {
             return new Bloodhound({
                name,
                limit,
                remote: {
                    url,
                    prepare: ComposerClient.prepareBloodhound
                },
                datumTokenizer: function (datum) {
                    return Bloodhound.tokenizers.whitespace((<any>datum).val);
                },
                queryTokenizer: Bloodhound.tokenizers.whitespace
            });
        }

        private resultsNotFound(evt) {
            let element: any = evt.currentTarget;
        }

        public selectedProduct(actionContext: Orckestra.Composer.IControllerActionContext) {
            let suggestionIndex;
            let selectedSuggestion: Object;

            //sort the object to retrieve the matching sku
            $.each(this.renderedSuggestions, function (index, obj) {
                if (obj.Sku === actionContext.elementContext.data('sku').toString()) {
                    suggestionIndex = index;
                    selectedSuggestion = obj;
                }
            });
        }

        public selectedSearchTermsSuggestion(actionContext: Orckestra.Composer.IControllerActionContext) {
            let suggestion = actionContext.elementContext.data('suggestion').toString();
            $('#search-box #search-input').val(suggestion);
            $('#search-box form').submit();
        }

        public selectedCategorySuggestion(actionContext: Orckestra.Composer.IControllerActionContext) {
            let suggestion = actionContext.elementContext.data('suggestion').toString();
            let parents = actionContext.elementContext.data('parents').toString().split(',').filter((parent) => parent);
            EventHub.instance().publish('categorySuggestionClicked', {
                data: {
                    suggestion,
                    parents
                }
            });
        }

        public selectedBrandSuggestion(actionContext: Orckestra.Composer.IControllerActionContext) {
            let suggestion = actionContext.elementContext.data('suggestion').toString();
            EventHub.instance().publish('brandSuggestionClicked', {
                data: {
                    suggestion
                }
            });
        }

    }
}
