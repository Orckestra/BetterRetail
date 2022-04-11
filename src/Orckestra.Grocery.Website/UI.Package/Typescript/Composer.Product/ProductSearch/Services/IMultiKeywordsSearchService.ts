///<reference path='../../../../Typings/tsd.d.ts' />

module Orckestra.Composer {
    export interface IMultiKeywordsSearchService  {

        setKeywords(keywords: any): Q.Promise<any>;

        getKeywords(): Q.Promise<any>;

        clearKeywords(): Q.Promise<any>;
    }
}
