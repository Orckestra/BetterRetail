/// <reference path='../../Typings/tsd.d.ts' />
/// <reference path='../../Typings/vue/index.d.ts' />
///<reference path='../Repositories/ISearchRepository.ts' />
///<reference path='../Repositories/SearchRepository.ts' />
/// <reference path='../Utils/UrlHelper.ts' />
/// <reference path='./SearchParams.ts' />
/// <reference path='./Constants/ContentSearchEvents.ts' />

module Orckestra.Composer {
    'use strict';

    export class ContentSearchResultsController extends Orckestra.Composer.Controller {
        protected vueSearchResults: Vue;
        protected searchRepository: ISearchRepository = new SearchRepository();

        public initialize() {
            super.initialize();
            console.log(this.context.viewModel);
            const { SearchResults, PagesCount, Total } = this.context.viewModel.ActiveTab;
            const { SelectedSortBy, AvailableSortBys } = this.context.viewModel;

            const self = this;
            this.vueSearchResults = new Vue({
                el: `#${this.context.container.data('vueid')}`,
                components: {
                },
                data: {
                    SearchResults,
                    TotalCount: Total,
                    SelectedSortBy,
                    AvailableSortBys,
                    Pagination: {
                        PagesCount: 1,
                        CurrentPage: 1,
                        PreviousPage: false,
                        NextPage: false,
                    },
                    isLoading: false
                },
                mounted() {
                    this.Pagination = this.getPagination(PagesCount)
                    self.eventHub.subscribe(ContentSearchEvents.SearchResultsLoaded, this.onSearchResultsLoaded);
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
                    previousPage(): void {
                        const queryString = SearchParams.previousPage();
                        this.loadSearchResults({queryString});
                    },
                    nextPage(): void {
                        const queryString = SearchParams.nextPage();
                        this.loadSearchResults({queryString});
                    },
                    toPage(page: any): void {
                        const queryString = SearchParams.toPage(page);
                        this.loadSearchResults({queryString});
                    },
                    sortingChanged(sortBy, sortOrder): void {
                        const queryString = SearchParams.changeSorting(sortBy, sortOrder);
                        this.loadSearchResults({queryString});
                    },
                    loadSearchResults({queryString}): void {
                        SearchParams.pushState(queryString);
                        const currentTab = SearchParams.getLastSegment();

                        this.isLoading = true;
                        self.searchRepository.getContentSearchResults(queryString, currentTab).then(result => {
                            this.isLoading = false;
                            console.log(result)
                            self.eventHub.publish(ContentSearchEvents.SearchResultsLoaded, { data: result });
                        });
                    },
                    onSearchResultsLoaded({data}): void {
                        const { SearchResults, PagesCount, Total } = data.ActiveTab;

                        this.Pagination = this.getPagination(PagesCount);
                        this.SearchResults = [...SearchResults];
                        this.TotalCount = Total;
                    }
                }
            });
        }
    }
}
