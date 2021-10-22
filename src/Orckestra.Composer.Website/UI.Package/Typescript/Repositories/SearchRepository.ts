/// <reference path='../../Typings/tsd.d.ts' />
/// <reference path='../Mvc/ComposerClient.ts' />
/// <reference path='./ICartRepository.ts' />

module Orckestra.Composer {
    'use strict';

    export class SearchRepository implements ISearchRepository {

        public getFacets(queryString): Q.Promise<any> {
            return ComposerClient.post('/api/search/getfacets', { QueryString: queryString });
        }
    }
}