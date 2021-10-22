/// <reference path='../../Typings/tsd.d.ts' />

module Orckestra.Composer {

    export interface ISearchRepository {

        /**
         * Get the facets of the current search criteria.
         */
        getFacets(searchCriteria): Q.Promise<any>;

    }
}