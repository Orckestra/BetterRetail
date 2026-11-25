///<reference path='../../../../Typings/tsd.d.ts' />

namespace Orckestra.Composer {
    export interface IShowFacetsService  {

        setShowFacets(show: any): Promise<any>;

        getShowFacets(): Promise<any>;

        clearShowFacets(): Promise<any>;
    }
}
