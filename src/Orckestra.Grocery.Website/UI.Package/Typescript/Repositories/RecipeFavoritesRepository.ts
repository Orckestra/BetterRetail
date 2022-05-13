/// <reference path='../../Typings/tsd.d.ts' />
/// <reference path='../Mvc/ComposerClient.ts' />
/// <reference path='./IRecipeFavoritesRepository.ts' />

module Orckestra.Composer {
    'use strict';

    export class RecipeFavoritesRepository implements IRecipeFavoritesRepository {

        public addFavorite(id: string) {
            return ComposerClient.post(`/api/recipes/addfavorite`, id);
        }

        public getFavorites() {
            return ComposerClient.get(`/api/recipes/favorites`);
        }

        public removeFavorite(id: string) {
            return ComposerClient.remove(`/api/recipes/removefavorite`, id);
        }

        public getMyFavorites(queryString): Q.Promise<any> {
            return ComposerClient.post('/api/recipes/myfavorites', queryString);
        }
    }
}