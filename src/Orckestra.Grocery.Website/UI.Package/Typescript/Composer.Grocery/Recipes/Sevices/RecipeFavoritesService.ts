///<reference path='../../../../Typings/tsd.d.ts' />
///<reference path='../../../Mvc/ComposerClient.ts' />
///<reference path='../../../Cache/CacheProvider.ts' />
///<reference path='../../../Cache/CacheError.ts' />
///<reference path='./IRecipeFavoritesService.ts' />
///<reference path='../../../Repositories/IRecipeFavoritesRepository.ts' />
///<reference path='../../../Repositories/RecipeFavoritesRepository.ts' />
///<reference path='../../../Events/EventScheduler.ts' />
///<reference path='../../../Utils/Utils.ts' />

module Orckestra.Composer {
    'use strict';

    export class RecipeFavoritesService implements IRecipeFavoritesService {

        private static GettingFreshRecipeFavoritesSummary: Q.Promise<any>;

        private recipeFavoritesRepository: IRecipeFavoritesRepository;
        private cacheKey: string = `IRecipeFavoritesViewModel|${Utils.getWebsiteId()}`;
        private cachePolicy: ICachePolicy = { slidingExpiration: 300 }; // 5min
        private cacheProvider: ICacheProvider;

        constructor(recipeFavoritesRepository: IRecipeFavoritesRepository) {

            if (!recipeFavoritesRepository) {
                throw new Error('Error: recipeFavoritesRepository is required');
            }

            this.recipeFavoritesRepository = recipeFavoritesRepository;
            this.cacheProvider = CacheProvider.instance();

            var loggedInScheduler = EventScheduler.instance(MyAccountEvents[MyAccountEvents.LoggedIn]);
            var loggedOutScheduler = EventScheduler.instance(MyAccountEvents[MyAccountEvents.LoggedOut]);

            loggedOutScheduler.subscribe((e: IEventInformation) => this.clearCache());
            loggedInScheduler.subscribe((e: IEventInformation) => this.clearCache());
        }

        public getRecipeFavoritesSummary(): Q.Promise<any> {
            return this.getCacheSummary()
                .fail(reason => {
                    if (this.canHandle(reason)) {
                        return this.getFreshRecipeFavoritesSummary();
                    }

                    console.error('An error occured while getting the favorites from cache.', reason);
                    throw reason;
                });
        }

        public getMyFavorites(quesryString): Q.Promise<any>{
            return this.recipeFavoritesRepository.getMyFavorites(quesryString);
        }

        public getFreshRecipeFavoritesSummary(): Q.Promise<any> {
            if (!RecipeFavoritesService.GettingFreshRecipeFavoritesSummary) {

                RecipeFavoritesService.GettingFreshRecipeFavoritesSummary =
                    this.recipeFavoritesRepository.getFavorites().then(model => {
                        return this.setToCache(model);
                    });
            }

            // to avoid getting a fresh favorites multiple times within a page session
            return RecipeFavoritesService.GettingFreshRecipeFavoritesSummary
                .fail((reason) => {
                    console.error('An error occured while getting a fresh favorites list.', reason);
                    throw reason;
                });
        }

        public addFavorite(id: string): Q.Promise<any> {
            return this.recipeFavoritesRepository.addFavorite(id)
                .fin(() => this.clearCache());
        }

        public removeFavorite(id: string): Q.Promise<any> {
            return this.recipeFavoritesRepository.removeFavorite(id)
                .fin(() => this.clearCache());
        }

        public clearCache(): Q.Promise<any> {
            RecipeFavoritesService.GettingFreshRecipeFavoritesSummary = null;
            return this.cacheProvider.defaultCache.clear(this.cacheKey);
        }

        protected getCacheSummary(): Q.Promise<any> {
            return this.cacheProvider.defaultCache.get<any>(this.cacheKey);
        }

        protected setToCache(favoritesModel: any): Q.Promise<any> {
            return this.cacheProvider.defaultCache.set(this.cacheKey, favoritesModel, this.cachePolicy);
        }

        private canHandle(reason: any): boolean {
            return reason === CacheError.Expired || reason === CacheError.NotFound;
        }

        public getSignInUrl(): Q.Promise<any> {
            return this.getRecipeFavoritesSummary()
                .then(item => {
                    return item.SignInUrl;
                });
        }

        public redirectToSignIn(): Q.Promise<any> {
            return this.getSignInUrl().then(signInUrl => {
                window.location.href = signInUrl + '?ReturnUrl=' + window.location.href;
            });
        }

        public mapSearchResults(searchResults, difficulties, favorites = []) {
            return searchResults && searchResults.map(item => {
                const hasTime = item.FieldsBag["IRecipe.CookingTime"] != null || item.FieldsBag["IRecipe.PreparationTime"] != null
                const cookingTime = Number(item.FieldsBag["IRecipe.CookingTime"]) || 0;
                const preparationTime = Number(item.FieldsBag["IRecipe.PreparationTime"]) || 0;
                const difficulty = difficulties[item.FieldsBag["IRecipe.Difficulty"]]
                const servings = item.FieldsBag["IRecipe.Servings"];
                const id = item.FieldsBag["IRecipe.Id"];
                const isFavorite = favorites.indexOf(id) > -1;
                
                return {
                    hasTime,
                    id,
                    cookingTime,
                    preparationTime,
                    difficulty,
                    servings,
                    isFavorite,
                    ...item
                }
            });
        }
    }
}