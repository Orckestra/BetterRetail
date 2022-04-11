/// <reference path='../../../Typings/tsd.d.ts' />
/// <reference path='../../Mvc/Controller.ts' />
/// <reference path='../../Mvc/IControllerActionContext.ts' />
/// <reference path='../../Mvc/IControllerContext.ts' />
/// <reference path='./Constants/SearchEvents.ts' />
/// <reference path='../../Composer.ContentSearch/Constants/ContentSearchEvents.ts' />
///<reference path='../../Repositories/ISearchRepository.ts' />
///<reference path='../../Repositories/SearchRepository.ts' />
///<reference path='../../Events/EventScheduler.ts' />
///<reference path='./Services/IMultiKeywordsSearchService.ts' />

module Orckestra.Composer {
    'use strict';

    export class SearchSummaryController extends Orckestra.Composer.Controller {
        protected vueSearchSummary: any;
        protected vuewTabSearchSummary: any;
        protected searchRepository: ISearchRepository = new SearchRepository();
        protected keywordChangedSchedule = EventScheduler.instance(SearchEvents.SearchKeywordChanged);
        protected multiKeywordSearchService: IMultiKeywordsSearchService = MultiKeywordsSearchService.instance();
        public initialize() {
            super.initialize();
            const Tabs = this.context.viewModel;
            const ProductCount = this.context.viewModel[0].Total;
            const SearchQuery = this.context.container.data('searchquery');
            const CorrectedSearchTerms = this.context.container.data('сorrectedsearchterms'); 
            const IsProductTab = this.context.container.data('isproducttab');
            const currentSite = this.context.container.data('current-site') === 'True';
            const self = this;
            this.vuewTabSearchSummary = new Vue({
                el: '#vueTabSearchSummary',
                data: {
                    Tabs,
                    SearchQuery,
                    IsMultiKeywords: false
                },
                mounted() {
                    self.eventHub.subscribe(ContentSearchEvents.SearchResultsLoaded, ({ data }) => {
                       this.Tabs = [...this.Tabs];
                    });
                    self.eventHub.subscribe(SearchEvents.SearchKeywordChanged, ({ data }) => {
                        this.SearchQuery = data.keyword;
                    });
                    self.multiKeywordSearchService.getKeywords().then(items => this.IsMultiKeywords = items && items.length);
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
            this.vueSearchSummary = new Vue({
                el: '#vueSearchSummary',
                data: {
                    Tabs,
                    ProductCount,
                    SearchQuery,
                    CorrectedSearchTerms,
                    ProductsLoading: false,
                    ContentLoading: false
                },
                mounted() {
                    self.eventHub.subscribe(SearchEvents.SearchResultsLoaded, ({data}) => {
                        this.ProductsLoading = false;
                        this.Tabs.find(t => t.IsProducts).Total = data.ProductSearchResults.TotalCount;
                        this.Tabs = [...this.Tabs];
                        this.CorrectedSearchTerms = data.ProductSearchResults.CorrectedSearchTerms;
                        this.ProductCount = data.ProductSearchResults.TotalCount;
                    });

                    self.eventHub.subscribe(ContentSearchEvents.SearchResultsLoaded, ({data}) => {
                        this.ContentLoading = false;
                        data.Tabs.forEach(x => {
                            const foundTab = this.Tabs.find(tab => tab.Title === x.Title)
                            if(foundTab) {
                                foundTab.Total = x.Total;
                                foundTab.TabUrl = x.TabUrl;
                            }
                        })
                        this.Tabs = [...this.Tabs];
                    });

                    self.eventHub.subscribe(SearchEvents.SearchKeywordChanged,  ({data}) => {
                        this.SearchQuery = data.keyword;
                        self.searchRepository.getContentSearchResults("?keywords=" + data.keyword, "Products", currentSite).then(result => {
                            self.eventHub.publish(ContentSearchEvents.SearchResultsLoaded, { data: result });                        
                        });
                    });

                    self.keywordChangedSchedule.setPostEventCallback((data) => self.OnAllKeywordChangedExecuted(data));
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

        protected OnAllKeywordChangedExecuted(data: any): Q.Promise<any> {
            var promise: Q.Promise<any> = Q.fcall(() => {
                this.vueSearchSummary.ProductsLoading = true;
                this.vueSearchSummary.ContentLoading = true;
                var keyword = data.keyword;
                ['breadcrumbSearchKeyword', 'keywordInFacets'].forEach(id => {
                    let el = document.getElementById(id);
                    if (el) {
                        el.textContent = keyword;
                    }
                });
            });

            return promise;
        }

        protected sendSearchTermForAnalytics(viewModel: any): void {
            const { TotalCount, Keywords: Keyword, ListName, CorrectedSearchTerms } = viewModel;

            if (TotalCount === 0 && Keyword) {
                this.eventHub.publish('noResultsFound', {data: {Keyword, ListName}});
            }

            if (!_.isEmpty(CorrectedSearchTerms) && Keyword && TotalCount !== 0) {
                const data = {
                    KeywordEntered: Keyword,
                    KeywordCorrected: CorrectedSearchTerms,
                    ListName,
                };

                this.eventHub.publish('searchTermCorrected', {data});
            }
        }
    }
}
