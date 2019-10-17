///<reference path='../../Typings/tsd.d.ts' />
///<reference path='../App.ts' />
///<reference path='../Services/HachSearchService.ts' />

module Orckestra.Composer {
    export class HachSearchBoxController extends SearchBoxController {

        private renderedSuggestions;
        private searchService: HachSearchService;

        public initialize() {
            super.initialize();
            this.searchService = new HachSearchService(EventHub.instance(), window);
            this.searchService.initialize({
                correctedSearchTerm: '',
                facetRegistry: {}
            });

            this.searchService['_baseSearchUrl'] = $('#frm-search-box').attr('action');

            var products = new Bloodhound({
                name: 'products',
                limit: 8,
                remote: {
                    url: '/api/search/autocomplete',
                    prepare: ComposerClient.prepareBloodhound
                },
                datumTokenizer: function (datum) {
                    return Bloodhound.tokenizers.whitespace((<any>datum).val);
                },
                queryTokenizer: Bloodhound.tokenizers.whitespace
            });

            products.initialize();

            var searchTerms = new Bloodhound({
                name: 'searchTerms',
                limit: 5,
                remote: {
                    url: '/api/search/suggestTerms',
                    prepare: ComposerClient.prepareBloodhound
                },
                datumTokenizer: function (datum) {
                    return Bloodhound.tokenizers.whitespace((<any>datum).val);
                },
                queryTokenizer: Bloodhound.tokenizers.whitespace
            });

            searchTerms.initialize();

            var categories = new Bloodhound({
                name: 'categories',
                limit: 4,
                remote: {
                    url: '/api/search/suggestCategories',
                    prepare: ComposerClient.prepareBloodhound
                },
                datumTokenizer: function (datum) {
                    return Bloodhound.tokenizers.whitespace((<any>datum).val);
                },
                queryTokenizer: Bloodhound.tokenizers.whitespace
            });

            categories.initialize();

            var brands = new Bloodhound({
                name: 'brands',
                limit: 3,
                remote: {
                    url: '/api/search/suggestBrands',
                    prepare: ComposerClient.prepareBloodhound
                },
                datumTokenizer: function (datum) {
                    return Bloodhound.tokenizers.whitespace((<any>datum).val);
                },
                queryTokenizer: Bloodhound.tokenizers.whitespace
            });

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

                console.log(suggestions);
                console.log(evt);
                //cache the rendered suggestion at the render complet
                this.renderedSuggestions = suggestions;
/*
                if (suggestions !== undefined) {
                    this.renderedSuggestions = suggestions;
                }*/

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

        private resultsNotFound(evt) {
            var element: any = evt.currentTarget;

            //I think this is just for google analytics
            //EventHub.instance().publish('FGL.suggestionNoResultFound', {
            //    data: {searchTerm: element.value}
            //});
        }

        public selectedProduct(actionContext: Orckestra.Composer.IControllerActionContext) {
            var suggestionindex;
            var selectedSuggestion: Object;

            //sort the object to retrieve the matching sku
            $.each(this.renderedSuggestions, function (index, obj) {
                if (obj.Sku === actionContext.elementContext.data('sku').toString()) {
                    suggestionindex = index;
                    selectedSuggestion = obj;
                }
            });

            //push the object list name and index of the clicked object
            //I think this is just for google analytics
            //EventHub.instance().publish('Hach.suggestionProductclick', {
            //    data: {
            //        Product: selectedSuggestion,
            //        ListName: 'autosuggest product click',
            //        Index: suggestionindex != null && suggestionindex !== undefined ? parseInt(suggestionindex) : null
            //    }
            //});
        }

        public selectedSearchTermsSuggestion(actionContext: Orckestra.Composer.IControllerActionContext) {
            var suggestion = actionContext.elementContext.data('suggestion').toString();
            $('#search-box #search-input').val(suggestion);
            $('#search-box form').submit();
        }

        public selectedCategorySuggestion(actionContext: Orckestra.Composer.IControllerActionContext) {
            var suggestion = actionContext.elementContext.data('suggestion').toString();
            var parents = actionContext.elementContext.data('parents').toString().split(',').filter((parent) => parent);
            EventHub.instance().publish('categorySuggestionClicked', {
                data: {
                    suggestion,
                    parents
                }
            });
        }

        public selectedBrandSuggestion(actionContext: Orckestra.Composer.IControllerActionContext) {
            var suggestion = actionContext.elementContext.data('suggestion').toString();
            EventHub.instance().publish('brandSuggestionClicked', {
                data: {
                    suggestion
                }
            });
        }

    }
}
