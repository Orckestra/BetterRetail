///<reference path='../../../../Typings/tsd.d.ts' />

module Orckestra.Composer {
    export interface IShowFacetsService  {

        setShowFacets(show: any): Q.Promise<any>;

        getShowFacets(): Q.Promise<any>;

        clearShowFacets(): Q.Promise<any>;
    }
}
