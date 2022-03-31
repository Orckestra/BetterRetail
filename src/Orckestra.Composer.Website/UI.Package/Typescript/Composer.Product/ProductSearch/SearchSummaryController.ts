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

        public initialize() {
            super.initialize();

            const Tabs = this.context.container.data('tabs') ;
            const self = this;

            this.vueSearchSummary = new Vue({
                el: '#vueSearchSummary',
                components: {
                },
                data: {
                    Tabs,
                    ProductSearchResults:  this.context.viewModel
                },
                mounted() {
                    self.eventHub.subscribe(SearchEvents.SearchResultsLoaded, ({data}) => {
                        this.ProductSearchResults = data.ProductSearchResults;

                        if(this.Tabs) {
                            this.Tabs[0].Total = data.ProductSearchResults.TotalCount;
                            this.Tabs = [...this.Tabs];
                        }
                    });

                    self.eventHub.subscribe(ContentSearchEvents.SearchResultsLoaded, ({data}) => {
                        if(this.Tabs) {
                            data.Tabs.forEach(x => {
                                const foundTab = this.Tabs.find(tab => tab.Title === x.Title)
                                if (foundTab) {
                                    foundTab.Total = x.Total;
                                }
                            })
                            this.Tabs = [...this.Tabs];
                        }
                    });
                },
                computed: {
                    totalTabsCount() {
                        return this.Tabs ? this.Tabs.reduce((accum, item) => accum + item.Total, 0) : 0;
                    },
                    totalCount() {
                        return this.ProductSearchResults.TotalCount;
                    }
                },
            });

            this.sendSearchTermForAnalytics(this.context.viewModel);
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
