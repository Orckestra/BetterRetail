/// <reference path='../../Typings/tsd.d.ts' />

module Orckestra.Composer {

    /**
     * Separates the logic that retrieves the data and maps it to the entity model from the application services that acts on the model.
    */
    export interface IRecipeFavoritesRepository {

        addFavorite(id: string);

        getFavorites();
        
        removeFavorite(id: string);

        getMyFavorites(queryString);
    }
}