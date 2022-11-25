/// <reference path='../../Typings/tsd.d.ts' />
/// <reference path='../../Typings/vue/index.d.ts' />
/// <reference path='../Repositories/ISearchRepository.ts' />
/// <reference path='../Repositories/SearchRepository.ts' />
/// <reference path='../Utils/UrlHelper.ts' />
/// <reference path='./SearchParams.ts' />
/// <reference path='./Constants/ContentSearchEvents.ts' />

/// <reference path='../Composer.Product/ProductSearch/Services/ShowFacetsService.ts' />

module Orckestra.Composer {
    'use strict';

    export class ContentSearchResultsController extends Orckestra.Composer.Controller {
        protected vueSearchResults: Vue;
        protected showFacetsService: ShowFacetsService = ShowFacetsService.instance();
        protected searchRepository: ISearchRepository = new SearchRepository();

        public initialize() {
            super.initialize();
            const { SearchResults, PagesCount, Total } = this.context.viewModel;
            const SelectedSortBy = this.context.container.data('selected-sort');
            const AvailableSortBys = this.context.container.data('available-sort');
            const itemsCount = this.context.container.data('items-count');
            const currentSite = this.context.container.data('current-site') === 'True';
            let FacetsVisible = true;
            const isOverriden = this.context.container.data('overriden') === 'True';
            

            this.sendContentSearchResultsForAnalytics(Total);

            const self = this;
            this.vueSearchResults = new Vue({
                el: `#${this.context.container.data('vueid')}`,
                components: {
                },
                data: {
                    SearchResults: SearchResults && SearchResults.slice(0, itemsCount),
                    TotalCount: Total,
                    SelectedSortBy,
                    AvailableSortBys,
                    FacetsVisible: FacetsVisible,
                    Pagination: {
                        PagesCount: 1,
                        CurrentPage: 1,
                        PreviousPage: false,
                        NextPage: false,
                    },
                    isOverriden: isOverriden,
                    isLoading: false
                },
                mounted() {
                    this.Pagination = this.getPagination(PagesCount)
                    self.eventHub.subscribe(ContentSearchEvents.SearchResultsLoaded, this.onSearchResultsLoaded);
                    self.showFacetsService.getShowFacets().then(
                        (value: boolean) => {
                                FacetsVisible = value;
                                if (!value) this.hideFacet();  
                           
                        }, 
                        (error: any) => {
                            FacetsVisible = true;
                            self.showFacetsService.setShowFacets(true);
                        }
                    );
                    
                },
                computed: {
                },
                updated: function () {
                    this.updateProductColumns();
                },
                methods: {
                    hideFacet(): void {
                        document.getElementById("leftCol").classList.add("w-0-lg");
                        document.getElementById("rightCol").classList.remove("col-lg-9");
                        self.vueSearchResults.$data.FacetsVisible = FacetsVisible; // setting this will trigger the updated function above
                    },
                    showFacet(): void {
                        document.getElementById("leftCol").classList.remove("w-0-lg");
                        document.getElementById("rightCol").classList.add("col-lg-9");
                        self.vueSearchResults.$data.FacetsVisible = FacetsVisible;
                    },
                    toggleFacet(): void {
                        if (FacetsVisible) {
                            this.hideFacet();
                        }
                        else {
                            this.showFacet();
                        }
                        FacetsVisible = !FacetsVisible;
                        self.showFacetsService.setShowFacets(FacetsVisible);
                    },
                    updateProductColumns(){
                        let searchContainer = document.getElementsByClassName("search-container");
                        if (isOverriden) return;
                        if (FacetsVisible) {
                            for (let i=0; i < searchContainer.length; i++) {
                                searchContainer[i].classList.replace("col-sm-3", "col-sm-4");
                            }
                        }
                        else {
                            for (let i=0; i < searchContainer.length; i++) {
                                searchContainer[i].classList.replace("col-sm-4", "col-sm-3");
                            }
                        }
                    },
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
                        self.searchRepository.getContentSearchResults(queryString, currentTab, currentSite).then(result => {
                            this.isLoading = false;
                            self.eventHub.publish(ContentSearchEvents.SearchResultsLoaded, { data: result });
                        });
                    },
                    onSearchResultsLoaded({data}): void {
                        const { SearchResults, PagesCount, Total } = data.ActiveTab;

                        this.Pagination = this.getPagination(PagesCount);
                        this.SearchResults = [...SearchResults];
                        this.TotalCount = Total;
                        this.SelectedSortBy = data.SelectedSortBy;

                        self.sendContentSearchResultsForAnalytics(Total);
                    }
                }
            });
        }

        protected sendContentSearchResultsForAnalytics(totalCount: number): void {
            const data = {
                Keywords: SearchParams.getKeyword(),
                TotalCount: totalCount,
                CurrentTab: SearchParams.getLastSegment()
            };

            this.eventHub.publish('contentSearchResultRendered', { data });
        }
    }
}
