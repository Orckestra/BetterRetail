/// <reference path='../../../Typings/tsd.d.ts' />
/// <reference path='../../Mvc/Controller.ts' />
/// <reference path='../../Mvc/IControllerActionContext.ts' />
/// <reference path='../../Mvc/IControllerContext.ts' />
/// <reference path='./Constants/SearchEvents.ts' />

module Orckestra.Composer {
    'use strict';

    export class SearchSummaryController extends Orckestra.Composer.Controller {
        protected vueSearchSummary: Vue;

        public initialize() {
            super.initialize();

            const ProductCount = this.context.viewModel.TotalCount;
            const TotalCount = this.context.container.data('total');
            const self = this;

            this.vueSearchSummary = new Vue({
                el: '#vueSearchSummary',
                components: {
                },
                data: {
                    ProductCount,
                    OtherCount: TotalCount > ProductCount ? TotalCount - ProductCount : 0
                },
                mounted() {
                    self.eventHub.subscribe(SearchEvents.SearchResultsLoaded, ({data}) => {
                        this.ProductCount = data.ProductSearchResults.TotalCount;
                    });
                },
                computed: {
                    totalCount() {
                        return this.ProductCount + this.OtherCount;
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
