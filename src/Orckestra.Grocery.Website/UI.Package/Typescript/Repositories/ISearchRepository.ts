/// <reference path='../../Typings/tsd.d.ts' />

module Orckestra.Composer {

    export interface ISearchRepository {

        /**
         * Get the facets of the current search criteria.
         */
        getFacets(searchCriteria): Q.Promise<any>;

        /**
         * Get the facets for the category page and current search criteria.
         */
        getCategoryFacets(categoryId, searchCriteria): Q.Promise<any>;

        /**
       * Get the facets for the query page and current query string.
       */
        getQueryFacets(QueryName, QueryType, QueryString): Q.Promise<any>;

        /**
         * Get search results
         * @param QueryString
         * @param CategoryId
         */
        getSearchResults(QueryString, CategoryId): Q.Promise<any>;

        /**
         * Get query search results
         * @param QueryString
         * @param QueryName
         * @param QueryType
         */
        getQuerySearchResults(QueryString, QueryName, QueryType): Q.Promise<any>;

        /**
         * Get content search results
         * @param QueryString
         * @param CurrentTabPathInfo
         * @param IsCurrentSiteOnly
         */
        getContentSearchResults(QueryString, CurrentTabPathInfo, IsCurrentSiteOnly): Q.Promise<any>;

        getProductsSearchResults(QueryString, Skus): Q.Promise<any>;

        getMyUsualsSearchResults(QueryString): Q.Promise<any>;

        getMyUsualsFacets(QueryString): Q.Promise<any>;
    }
}
