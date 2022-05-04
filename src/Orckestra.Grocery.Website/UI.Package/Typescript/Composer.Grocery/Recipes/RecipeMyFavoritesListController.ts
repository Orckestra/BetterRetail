///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../ErrorHandling/ErrorHandler.ts' />
///<reference path='../../Composer.Grocery/Recipes/Sevices/IRecipeFavoritesService.ts' />
///<reference path='../../Composer.Grocery/Recipes/Sevices/RecipeFavoritesService.ts' />
///<reference path='../../Repositories/RecipeFavoritesRepository.ts' />
///<reference path='../../Composer.MyAccount/Common/IMembershipService.ts' />
///<reference path='../../Composer.MyAccount/Common/MembershipService.ts' />
/// <reference path='../../Composer.ContentSearch/SearchParams.ts' />
/// <reference path='../../Composer.ContentSearch/Constants/ContentSearchEvents.ts' />

module Orckestra.Composer {

    export class RecipeMyFavoritesListController extends Controller {
        public VueRecipe: Vue;

        protected recipeFavoritesRepository: IRecipeFavoritesRepository = new RecipeFavoritesRepository();
        protected recipeFavoritesService: IRecipeFavoritesService = new RecipeFavoritesService(this.recipeFavoritesRepository);
        protected membershipService: IMembershipService = new MembershipService(new MembershipRepository());
        
        public initialize() {
            super.initialize();

            let authenticatedPromise = this.membershipService.isAuthenticated();
            
            const queryString = SearchParams.currentPage();
            let favPromise = this.recipeFavoritesService.getMyFavorites(queryString);
            Q.all([authenticatedPromise, favPromise]).spread((authVm, myFavorites) => this.initializeVueComponent(authVm, myFavorites));
        }

        private initializeVueComponent(authVm, myFavorites) {
            const self = this;
            const { SearchResults, PagesCount, Total } = myFavorites;
            let difficulties = this.context.container.data('difficulties');
            difficulties = difficulties && difficulties.reduce((accum, item) => {
                item.forEach(x => accum[x.Id] = x.Title)
                return accum;
            }, {});

            this.VueRecipe = new Vue({
                el: '#vueRecipeMyFavoriteList',
                data: {
                    IsAuthenticated: authVm.IsAuthenticated,
                    TotalCount: Total,
                    Recipes: self.recipeFavoritesService.mapSearchResults(SearchResults, difficulties),
                    Pagination: self.getPagination(PagesCount),
                },
                mounted() {
                    self.eventHub.subscribe(ContentSearchEvents.SearchResultsLoaded, this.onSearchResultsLoaded);
                },
                methods: {
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
                    loadSearchResults({ queryString }): void {
                        SearchParams.pushState(queryString);
                        self.recipeFavoritesService.getMyFavorites(queryString).then(result => {
                            self.eventHub.publish(ContentSearchEvents.SearchResultsLoaded, { data: result });
                        });
                    },
                    onSearchResultsLoaded({ data }): void {
                        const { SearchResults, PagesCount, Total } = data;

                        this.Pagination = this.getPagination(PagesCount);
                        this.Recipes = self.recipeFavoritesService.mapSearchResults(SearchResults, difficulties);
                        this.TotalCount = Total;
                    },
                    removeFavorite(itemId) {
                        if (!this.IsAuthenticated) {
                            return self.recipeFavoritesService.redirectToSignIn();
                        }

                        self.recipeFavoritesRepository.removeFavorite(itemId).fin(() => {   
                            self.recipeFavoritesService.clearCache();
                            this.Recipes = this.Recipes.filter((obj => obj.id != itemId));
                            this.TotalCount = this.TotalCount -1;                   
                            if(this.Recipes.length < 1 && this.Pagination.CurrentPage != 1){
                                const queryString = SearchParams.previousPage();
                                this.loadSearchResults({ queryString });
                        }});
                    }
                }
            })
        }

        private getPagination(count): any {
            const currentPage = SearchParams.currentPage();
            return ({
                PagesCount: count,
                CurrentPage: currentPage,
                PreviousPage: currentPage > 1,
                NextPage: currentPage < count
            });
        }
    }
}