/// <reference path='../../../Typings/tsd.d.ts' />
/// <reference path='../../Mvc/Controller.ts' />
/// <reference path='../../Mvc/IControllerActionContext.ts' />
/// <reference path='../../Mvc/IControllerContext.ts' />
/// <reference path='./Constants/SearchEvents.ts' />
/// <reference path='../../Composer.ContentSearch/Constants/ContentSearchEvents.ts' />

module Orckestra.Composer {
    'use strict';

    export class SearchSummaryController extends Orckestra.Composer.Controller {
        protected vueSearchSummary: Vue;
        protected vueTabSearchSummary: any;

        public initialize() {
            super.initialize();

            const Tabs = this.context.viewModel;
            const SearchQuery = this.context.container.data('searchquery');
            const CorrectedSearchTerms = this.context.container.data('сorrectedsearchterms');
            const IsProductTab = this.context.container.data('isproducttab');
            const self = this;
            this.initializeTabSummaryVue(Tabs, SearchQuery, self);

            this.vueSearchSummary = new Vue({
                el: '#vueSearchSummary',
                components: {
                },
                data: {
                    Tabs,
                    SearchQuery,
                    CorrectedSearchTerms,
                    ProductsLoading: false,
                    ContentLoading: false
                },
                mounted() {
                    self.eventHub.subscribe(SearchEvents.SearchRequested, () => this.ProductsLoading = true);
                    self.eventHub.subscribe(SearchEvents.SearchResultsLoaded, ({ data }) => {
                        this.ProductsLoading = false;
                        this.Tabs.find(t => t.IsProducts).Total = data.ProductSearchResults.TotalCount;
                        this.Tabs = [...this.Tabs];
                        this.CorrectedSearchTerms = data.ProductSearchResults.CorrectedSearchTerms;
                        this.ProductCount = data.ProductSearchResults.TotalCount;
                    });

                    self.eventHub.subscribe(ContentSearchEvents.SearchResultsLoaded, ({ data }) => {
                        this.ContentLoading = false;
                        data.Tabs.forEach(x => {
                            const foundTab = this.Tabs.find(tab => tab.Title === x.Title)
                            if (foundTab) {
                                foundTab.Total = x.Total;
                                foundTab.TabUrl = x.TabUrl;
                            }
                        })
                        this.Tabs = [...this.Tabs];
                    });
                },
                computed: {
                    Loading() { return this.ProductsLoading || this.ContentLoading },
                    TotalCount() {
                        return this.Tabs.reduce((accum, item) => accum + item.Total, 0);
                    },
                    IsProductsCorrected() {
                        return this.CorrectedSearchTerms && this.ProductCount > 0 && IsProductTab;
                    }
                },
            });

            this.sendSearchTermForAnalytics(this.context.viewModel);
        }

        private initializeTabSummaryVue(Tabs: any, SearchQuery: any, self: this) {
            let elTabSearchSummary = document.getElementById('vueTabSearchSummary');

            if (elTabSearchSummary) {
                this.vueTabSearchSummary = new Vue({
                    el: '#vueTabSearchSummary',
                    data: {
                        Tabs,
                        SearchQuery
                    },
                    mounted() {
                        self.eventHub.subscribe(ContentSearchEvents.SearchResultsLoaded, () => {
                            this.Tabs = [...this.Tabs];
                        });
                    },
                    computed: {
                        CurrentTab() {
                            return this.Tabs.find(t => t.IsActive);
                        },
                        TabsWithResults() {
                            return this.Tabs.filter(t => t.Total > 0);
                        }
                    }
                });
            }
        }

        protected sendSearchTermForAnalytics(viewModel: any): void {
            const { TotalCount, Keywords: Keyword, ListName, CorrectedSearchTerms } = viewModel;

            if (TotalCount === 0 && Keyword) {
                this.eventHub.publish('noResultsFound', { data: { Keyword, ListName } });
            }

            if (!_.isEmpty(CorrectedSearchTerms) && Keyword && TotalCount !== 0) {
                const data = {
                    KeywordEntered: Keyword,
                    KeywordCorrected: CorrectedSearchTerms,
                    ListName,
                };

                this.eventHub.publish('searchTermCorrected', { data });
            }
        }
    }
}
