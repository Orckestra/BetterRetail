///<reference path='../../../../Typings/tsd.d.ts' />
///<reference path='../../../Mvc/IControllerContext.ts' />
///<reference path='../../../Mvc/ComposerClient.ts' />
///<reference path='./IMultiKeywordsSearchService.ts' />
///<reference path='../../../Utils/Utils.ts' />
///<reference path='../../../Cache/CacheProvider.ts' />

module Orckestra.Composer {
    'use strict';

    export class MultiKeywordsSearchService implements IMultiKeywordsSearchService {
        private static _instance: MultiKeywordsSearchService = new MultiKeywordsSearchService();
        protected cacheMultiKeywordKey: string = `multikeywordsearch_${Utils.getWebsiteId()}`;
        protected cacheProvider: ICacheProvider = CacheProvider.instance();
        protected cachePolicy: ICachePolicy = { slidingExpiration: 300 }; // 5min
        

        public static instance(): MultiKeywordsSearchService {
            return MultiKeywordsSearchService._instance;
        }

        public setKeywords(keywords: any): Q.Promise<any>
        {
            return this.cacheProvider.sessionCache.set(this.cacheMultiKeywordKey, keywords);
        }

        public getKeywords(): Q.Promise<any>
        {
            return this.cacheProvider.sessionCache.get<any>(this.cacheMultiKeywordKey);
        }

        public clearKeywords(): Q.Promise<any>
        {
            return this.cacheProvider.sessionCache.clear(this.cacheMultiKeywordKey);
        }
    }
}
