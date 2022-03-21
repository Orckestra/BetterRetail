/// <reference path='../../Typings/tsd.d.ts' />
/// <reference path='../../Typings/vue/index.d.ts' />
///<reference path='../Repositories/ISearchRepository.ts' />
///<reference path='../Repositories/SearchRepository.ts' />
/// <reference path='../Utils/UrlHelper.ts' />
/// <reference path='./SearchParams.ts' />

module Orckestra.Composer {
    'use strict';

    export class ContentSearchResultsController extends Orckestra.Composer.Controller {
        protected vueSearchResults: Vue;
        protected searchRepository: ISearchRepository = new SearchRepository();

        public initialize() {
            super.initialize();
            console.log(this.context.viewModel);
            const { SearchResults, PagesCount, Total, SearchResultEntries } = this.context.viewModel.ActiveTab;

            const self = this;
            this.vueSearchResults = new Vue({
                el: `#${this.context.container.data('vueid')}`,
                components: {
                },
                data: {
                    SearchResults: SearchResultEntries,
                    TotalCount: SearchResults.ResultsFound,
                    Pagination: null,
                    isLoading: false
                },
                mounted() {
                    var pageUrl = decodeURIComponent(Composer.urlHelper.getURLParameter(window.location.href, 'errorpath'));
                   // console.log(window.location.href);
                    console.log(window.location.search);

                    console.log(window.location.pathname);

                    this.Pagination = this.getPagination(PagesCount)

                    console.log(this.Pagination);

                    console.log(SearchParams.toPage('5'));
                    console.log(SearchParams.nextPage());
                    console.log(SearchParams.previousPage());

                    //this._window.history.pushState(this._window.history.state, "", this._baseSearchUrl + queryString);
                    //pathInfo?.Split('/')[1];

                    // this.eventHub.publish(ProductEvents.LineItemAdding, { data: productData });
                },
                computed: {
                },
                methods: {
                    getPagination(count): any {
                        const currentPage = SearchParams.currentPage();
                        return ({
                            PagesCount: count,
                            CurrentPage: currentPage,
                            PreviousPage:  currentPage > 1,
                            NextPage:  currentPage < count
                        });
                    },
                    onSearchRequested({data}): void {
                        this.isLoading = true;
                        self.searchRepository.getContentSearchResults(data.queryString).then(result => {
                            this.isLoading = false;
                            console.log(result)
                        //    Object.keys(result.ProductSearchResults).forEach(key => this[key] = result.ProductSearchResults[key]);

                           // self.eventHub.publish(SearchEvents.SearchResultsLoaded, { data: result });
                        });
                    },
                }
            });
        }

        private encodeQuerystringValue(valueToEncode: string) {
            return encodeURIComponent(valueToEncode).replace(/%20/g, '+');
        }

        private decodeQuerystringValue(valueToDecode) {
            return decodeURIComponent(valueToDecode).replace(/\+/g, ' ');
        }
    }
}
