///<reference path='../../../../Typings/tsd.d.ts' />

module Orckestra.Composer {
    export interface IRecipeFavoritesService {

        addFavorite(id: string): Q.Promise<any>;

        removeFavorite(id: string): Q.Promise<any>;

        getRecipeFavoritesSummary(): Q.Promise<any>;

        clearCache(): Q.Promise<any>;

        redirectToSignIn(): Q.Promise<any>;
        
        getMyFavorites(quesryString): Q.Promise<any>;
        
        mapSearchResults(searchResults, difficulties, favorites? : string[]);
    }
}
