///<reference path='../../Typings/tsd.d.ts' />
///<reference path='../Services/AutocompleteSearchService.ts' />
/// <reference path='../Events/EventHub.ts' />
/// <reference path='./SearchBoxController.ts' />
///<reference path='../Mvc/ComposerClient.ts' />

module Orckestra.Composer {
    export class AutocompleteSearchBoxController extends SearchBoxController {

        private renderedSuggestions;
        private searchService: AutocompleteSearchService;
        private searchTerm: string;
        private source: string = 'Autosuggest Products';
        private suggestions: any;

        public initialize() {
            super.initialize();
            this.searchService = new AutocompleteSearchService(EventHub.instance(), window);
            this.searchService.initialize({
                correctedSearchTerm: '',
                facetRegistry: {}
            });

            this.searchService['_baseSearchUrl'] = $('#frm-search-box').attr('action');

            let searchBox = $('#search-box');
            let datasetList = [];
            let rightSuggestionCount = 0;

            const products = this.getBloodhoundInstance(
                'products',
                searchBox.data('autocomplete-limit'),
                '/api/search/autocomplete',
                (response) => {
                    let {Suggestions} = response;
                    this.suggestions = Suggestions;
                    EventHub.instance().publish('suggestionSearchTermEntered', {data: this.searchTerm});
                    return Suggestions && Suggestions.length ? {Suggestions} : {};
                }
            );

            products.initialize();
            datasetList.push(this.getDataSetInst('Products', products, 'SearchSuggestions', 'SearchSuggestionsEmpty'));

            if (searchBox.data('search-terms-enable') === 'True') {
                let searchTerms = this.getBloodhoundInstance('searchTerms', searchBox.data('search-terms-limit'), '/api/search/suggestTerms');
                searchTerms.initialize();
                datasetList.push(this.getDataSetInst('SearchTerms', searchTerms, 'SearchTermsSuggestions', 'SearchTermsSuggestionsEmpty'));
                rightSuggestionCount++;
            }

            if (searchBox.data('categories-enable') === 'True') {
                let categories = this.getBloodhoundInstance('categories', searchBox.data('categories-limit'), '/api/search/suggestCategories');
                categories.initialize();
                datasetList.push(this.getDataSetInst('Categories', categories, 'CategorySuggestions', 'CategorySuggestionsEmpty'));
                rightSuggestionCount++;
            }

            if (searchBox.data('brands-enable') === 'True') {
                let brands = this.getBloodhoundInstance('brands', searchBox.data('brand-limit'), '/api/search/suggestBrands');
                brands.initialize();
                datasetList.push(this.getDataSetInst('Brands', brands, 'BrandSuggestions', 'BrandSuggestionsEmpty'));
                rightSuggestionCount++;
            }

            $('#search-box .js-typeahead').typeahead({
                    minLength: 3,
                    highlight: true,
                    hint: true,
                    //async: true
                },
                ...datasetList
            ).on('typeahead:render', (evt, suggestions) => {
                //cache the rendered suggestion at the render complete
                this.renderedSuggestions = suggestions;

                if ($('.js-suggestion-empty').length === rightSuggestionCount) {
                    $('.tt-menu').addClass('right-empty');
                } else {
                    $('.tt-menu').removeClass('right-empty');
                }

            }).on('typeahead:asyncreceive', (evt) => {
                this.searchTerm = (evt.currentTarget as HTMLInputElement).value;

                if (this.renderedSuggestions === undefined) {
                    this.resultsNotFound(evt);
                }
            });

            $('.tt-dataset').wrapAll('<div class="suggestions-wrapper"></div>');
            $('.tt-menu .tt-dataset:not(:first)').wrapAll('<div class="suggestion-right-col"></div>');
        }

        private getBloodhoundInstance(name, limit, url, transform?): Bloodhound<any> {
            const collectionName = 'Suggestions';
            return new Bloodhound({
                name,
                limit,
                remote: {
                    url: `${url}?limit=${limit}`,
                    prepare: ComposerClient.prepareBloodhound,
                    transform: transform || ((response) => {
                        const suggestions = response[collectionName];
                        return suggestions && suggestions.length ? {suggestions} : {};
                    }),
                    rateLimitWait: 0
                },
                datumTokenizer: (datum) => Bloodhound.tokenizers.obj.whitespace((<any>datum).val),
                queryTokenizer: Bloodhound.tokenizers.whitespace
            });
        }

        private getDataSetInst(name: string, bloodhound: Bloodhound<any>, template: string, templateEmpty: string): Twitter.Typeahead.Dataset<any> {
            return {
                name,
                display: 'DisplayName',
                source: bloodhound.ttAdapter(),
                templates: {
                    notFound: (<any>Orckestra.Composer).Templates[templateEmpty],
                    suggestion: (<any>Orckestra.Composer).Templates[template]
                }
            };
        }

        private resultsNotFound(evt) {
            let element: any = evt.currentTarget;
        }


        public selectedSearchTermsSuggestion(actionContext: Orckestra.Composer.IControllerActionContext) {
            let suggestion = actionContext.elementContext.data('suggestion').toString();
            EventHub.instance().publish('searchTermSuggestionClicked', { data: { suggestion } });
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

        public showMoreResults() {
            $('#search-box #search-input').val(this.searchTerm);
            $('#frm-search-box').submit();
        }

        public suggestedProductClick(actionContext: Orckestra.Composer.IControllerActionContext) {
            const productContext: JQuery = actionContext.elementContext.closest('[data-product-id]');
            let productId = productContext.attr('data-product-id');
            let index = productContext.attr('data-index');
            let suggestion: any = _.find(this.suggestions, {ProductId: productId});
            //for Analytics
            this.eventHub.publish('productSuggestionClicked', {
                data: {
                    suggestion,
                    Index: index,
                    ListName: this.source
                }
            });
        }
    }
}
