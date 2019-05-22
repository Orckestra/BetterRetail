///<reference path='../../../../Composer.UI/Source/Typings/tsd.d.ts' />

module Orckestra.Composer {
    export interface IStoreService {

        getStores(): Q.Promise<any>;
    }
}