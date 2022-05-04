/// <reference path='../../Typings/tsd.d.ts' />
/// <reference path='../../Typings/vue/index.d.ts' />
///<reference path='../Repositories/ISearchRepository.ts' />
///<reference path='../Repositories/SearchRepository.ts' />
/// <reference path='../Utils/UrlHelper.ts' />
/// <reference path='./SearchParams.ts' />
/// <reference path='./Constants/ContentSearchEvents.ts' />
///<reference path='../Composer.Grocery/Recipes/Sevices/IRecipeFavoritesService.ts' />
///<reference path='../Composer.Grocery/Recipes/Sevices/RecipeFavoritesService.ts' />
///<reference path='../Repositories/RecipeFavoritesRepository.ts' />
///<reference path='../Composer.MyAccount/Common/IMembershipService.ts' />
///<reference path='../Composer.MyAccount/Common/MembershipService.ts' />

module Orckestra.Composer {
    'use strict';

    export class ContentSearchResultsController extends Orckestra.Composer.Controller {
        protected vueSearchResults: Vue;
        protected searchRepository: ISearchRepository = new SearchRepository();
        protected recipeFavoritesService: IRecipeFavoritesService = new RecipeFavoritesService(new RecipeFavoritesRepository());
        protected membershipService: IMembershipService = new MembershipService(new MembershipRepository());

        public initialize() {
            super.initialize();

            let authenticatedPromise = this.membershipService.isAuthenticated();
            let favPromise = this.recipeFavoritesService.getRecipeFavoritesSummary();
            Q.all([authenticatedPromise, favPromise]).spread((authVm, { FavoriteIds }) => this.initializeVueComponent(authVm, FavoriteIds));
        }

        private initializeVueComponent(authVm, favorites) {
            const { SearchResults, PagesCount, Total, DataTypes } = this.context.viewModel;
            const SelectedSortBy = this.context.container.data('selected-sort');
            const AvailableSortBys = this.context.container.data('available-sort');
            const itemsCount = this.context.container.data('items-count');
            const currentSite = this.context.container.data('current-site') === 'True';
            const isRecipe = this.context.container.data('is-recipes') === 'True';
            let difficulties = this.context.container.data('difficulties');
            difficulties = difficulties && difficulties.reduce((accum, item) => {
                item.forEach(x => accum[x.Id] = x.Title)
                return accum;
            }, {});

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
                    Pagination: {
                        PagesCount: 1,
                        CurrentPage: 1,
                        PreviousPage: false,
                        NextPage: false,
                    },
                    isLoading: false,
                    RecipeFavorites: favorites,
                    IsAuthenticated: authVm.IsAuthenticated,
                },
                mounted() {
                    this.Pagination = this.getPagination(PagesCount);

                    this.SearchResults = this.mapSearchResults(SearchResults);
                    self.eventHub.subscribe(ContentSearchEvents.SearchResultsLoaded, this.onSearchResultsLoaded);
                },
                methods: {
                    getPagination(count): any {
                        const currentPage = SearchParams.currentPage();
                        return ({
                            PagesCount: count,
                            CurrentPage: currentPage,
                            PreviousPage: currentPage > 1,
                            NextPage: currentPage < count
                        });
                    },
                    previousPage(): void {
                        const queryString = SearchParams.previousPage();
                        this.loadSearchResults({ queryString });
                    },
                    nextPage(): void {
                        const queryString = SearchParams.nextPage();
                        this.loadSearchResults({ queryString });
                    },
                    toPage(page: any): void {
                        const queryString = SearchParams.toPage(page);
                        this.loadSearchResults({ queryString });
                    },
                    sortingChanged(sortBy, sortOrder): void {
                        const queryString = SearchParams.changeSorting(sortBy, sortOrder);
                        this.loadSearchResults({ queryString });
                    },
                    loadSearchResults({ queryString }): void {
                        SearchParams.pushState(queryString);
                        const currentTab = SearchParams.getLastSegment();

                        this.isLoading = true;
                        self.searchRepository.getContentSearchResults(queryString, currentTab, currentSite).then(result => {
                            this.isLoading = false;
                            self.eventHub.publish(ContentSearchEvents.SearchResultsLoaded, { data: result });
                        });
                    },
                    onSearchResultsLoaded({ data }): void {
                        const { SearchResults, PagesCount, Total } = data.ActiveTab;

                        this.Pagination = this.getPagination(PagesCount);

                        this.SearchResults = this.mapSearchResults(SearchResults);
                        this.TotalCount = Total;
                        this.SelectedSortBy = data.SelectedSortBy;

                        self.sendContentSearchResultsForAnalytics(Total);
                    },
                    mapSearchResults(searchResults) {
                        if (isRecipe) {
                            return self.recipeFavoritesService.mapSearchResults(searchResults, difficulties, this.RecipeFavorites);
                        }
                        return searchResults;
                    },
                    setFavorite(itemId, isFavorite) {
                        if (!this.IsAuthenticated) {
                            return self.recipeFavoritesService.redirectToSignIn();
                        }

                        const elementIndex = this.SearchResults.findIndex((obj => obj.id == itemId));
                        this.SearchResults[elementIndex].isFavorite = !isFavorite;
                        if (isFavorite) {
                            self.recipeFavoritesService.removeFavorite(itemId);
                            this.RecipeFavorites = this.RecipeFavorites.filter(function (item) {
                                return item != itemId
                            });
                        } else {
                            self.recipeFavoritesService.addFavorite(itemId);
                            this.RecipeFavorites.push(itemId);
                        }
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
