///<reference path='../../Typings/tsd.d.ts' />
///<reference path='../Services/AutocompleteSearchService.ts' />
/// <reference path='../Events/EventHub.ts' />
/// <reference path='./SearchBoxController.ts' />
///<reference path='../Mvc/ComposerClient.ts' />

module Orckestra.Composer {
    interface VueAutocompleteData {
        sectionConfigs: any;
        debounceMilliseconds: number;
        query: string; suggestions: any[];
        results: any[];
        timeout: null;
        selected: any
    }

    export class AutocompleteSearchBoxVueController extends SearchBoxController {
        private searchService: AutocompleteSearchService;
        public VueAutocomplete: Vue;

        public initialize() {
            super.initialize();
            this.initializeVue();

            this.searchService = new AutocompleteSearchService(EventHub.instance(), window);
            this.searchService.initialize({
                correctedSearchTerm: '',
                facetRegistry: {}
            });
        }

        public initializeVue () {
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
                        debounceMilliseconds: 250,
                        suggestions: [],
                        sectionConfigs: {
                            autocomplete: {
                                active: true,
                                onSelected: function (selected) {
                                    this.selected = selected.item;
                                }.bind(this),
                                type: "default-section",
                                ulClass: "row autosuggest-top-results",
                                liClass: {
                                    "col-md-6": true,
                                }
                            },
                            suggestcategories: {
                                onSelected: function (selected) {
                                    this.selectedCategorySuggestion(selected);
                                    this.selected = selected.item;
                                }.bind(this),
                            },
                            suggestbrands: {
                                //label: "suggestbrands",
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
                        }
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

                    if(this.query) {
                        this.fetchResults(this.query);
                    }
                },
                updated() {
                },
                computed: {
                    isEmptyRight () {
                        return this.suggestions.length === 1;
                    }
                },
                methods: {
                    fetchResults(result) {
                        const query = this.query;

                        clearTimeout(this.timeout);
                        this.timeout = setTimeout(() => {
                            const sectionNames = Object.keys(this.sectionConfigs).filter(name => this.sectionConfigs[name].active);

                            const results = sectionNames.map(sectionName => {
                                const limit = this.sectionConfigs[sectionName].limit;
                                return ComposerClient.post(`/api/search/${sectionName}?limit=${limit}`, { Query: query })
                            });

                            Q.all(results).then(values => {
                                this.selected = null;

                                this.suggestions = sectionNames
                                    .map((sectionName, index) => {
                                        const data = this.mapSuggestions(values[index].Suggestions, sectionName, query);
                                        return ({ name: sectionName, data })
                                    })
                                    .filter(section => section.data.length || section.name === 'autocomplete');
                            });
                        }, this.debounceMilliseconds);
                    },
                    highlightSuggestion(value, query) {
                        const start = value.toLowerCase().indexOf(query.toLowerCase());
                        const end = start + query.length;
                        if(start < 0) return value;

                        return [
                            value.slice(0, start),
                            `<strong>${value.slice(start, end)}</strong>`,
                            value.slice(end),
                        ].join('');
                    },
                    mapSuggestions(suggestions = [], sectionName, query) {
                        return suggestions.map((suggest) => {
                            const title = sectionName === 'suggestcategories' ? [...suggest.Parents, suggest.DisplayName].join(' > ')  : suggest.DisplayName;
                            return ({ ...suggest, mappedDisplayName: this.highlightSuggestion(title, query) })
                        })
                    },
                    getSuggestionValue(suggestion) {
                        let { name, item } = suggestion;
                        return item.DisplayName;
                    },
                    shouldRenderSuggestions(size, loading) {
                        return this.query.length > 2 && !loading
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
                                parents: suggestion.item.Parents
                            }
                        });
                    },
                    selectedBrandSuggestion(suggestion) {
                        EventHub.instance().publish('brandSuggestionClicked', {
                            data: { suggestion: suggestion.item.DisplayName }
                        });
                    }
                }
            });
        }
    }
}
