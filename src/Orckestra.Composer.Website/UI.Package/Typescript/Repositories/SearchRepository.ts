/// <reference path='../../Typings/tsd.d.ts' />
/// <reference path='../Mvc/ComposerClient.ts' />
/// <reference path='./ISearchRepository.ts' />

module Orckestra.Composer {
    'use strict';

    export class SearchRepository implements ISearchRepository {

        public getFacets(QueryString): Q.Promise<any> {
            return ComposerClient.post('/api/search/getfacets', { QueryString });
        }

        public getCategoryFacets(CategoryId, QueryString): Q.Promise<any> {
            return ComposerClient.post('/api/search/getcategoryfacets', { QueryString, CategoryId });
        }
    }
}
