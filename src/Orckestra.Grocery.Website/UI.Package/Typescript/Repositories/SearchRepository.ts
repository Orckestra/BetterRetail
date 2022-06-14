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

        public getQueryFacets(QueryName, QueryType, QueryString): Q.Promise<any> {
            return ComposerClient.post('/api/searchquery/getqueryfacets', { QueryString, QueryName, QueryType });
        }

        public getSearchResults(QueryString, CategoryId): Q.Promise<any> {
            return ComposerClient.post('/api/search/search', { QueryString, CategoryId });
        }

        public getQuerySearchResults(QueryString, QueryName, QueryType): Q.Promise<any> {
            return ComposerClient.post('/api/searchquery/search', { QueryString, QueryName, QueryType });
        }

        public getContentSearchResults(QueryString, CurrentTabPathInfo, IsCurrentSiteOnly): Q.Promise<any> {
            return ComposerClient.post('/api/contentsearch/search', { QueryString, CurrentTabPathInfo, IsCurrentSiteOnly });
        }

        public getProductsSearchResults(QueryString, Skus): Q.Promise<any> {
            return ComposerClient.post('/api/search/searchBySkus', { QueryString, Skus });
        }

        public getMyUsualsSearchResults(QueryString): Q.Promise<any> {
            return ComposerClient.post('/api/myUsuals/search', { QueryString });
        }

        public getMyUsualsFacets(QueryString): Q.Promise<any> {
            return ComposerClient.post('/api/myUsuals/getfacets', { QueryString });
        }
    }
}
