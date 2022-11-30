///<reference path='../../../../Typings/tsd.d.ts' />
///<reference path='../../../Mvc/IControllerContext.ts' />
///<reference path='../../../Mvc/ComposerClient.ts' />
///<reference path='./IShowFacetsService.ts' />
///<reference path='../../../Utils/Utils.ts' />
///<reference path='../../../Cache/CacheProvider.ts' />

module Orckestra.Composer {
    'use strict';

    export class ShowFacetsService implements IShowFacetsService {
        private static _instance: ShowFacetsService = new ShowFacetsService();
        protected cacheShowFacetKey: string = `showfacet_${Utils.getWebsiteId()}`;
        protected cacheProvider: ICacheProvider = CacheProvider.instance();
        protected cachePolicy: ICachePolicy = { slidingExpiration: 300 }; // 5min
        

        public static instance(): ShowFacetsService {
            return ShowFacetsService._instance;
        }

        public setShowFacets(show: any): Q.Promise<any>
        {
            return this.cacheProvider.sessionCache.set(this.cacheShowFacetKey, show);
        }

        public getShowFacets(): Q.Promise<any>
        {
            return this.cacheProvider.sessionCache.get<any>(this.cacheShowFacetKey);
        }

        public clearShowFacets(): Q.Promise<any>
        {
            return this.cacheProvider.sessionCache.clear(this.cacheShowFacetKey);
        }
    }
}
