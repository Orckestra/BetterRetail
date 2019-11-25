/// <reference path='../../../../Composer.UI/Source/Typings/tsd.d.ts' />

module Orckestra.Composer {

    export interface ICacheProvider {

        defaultCache: ICache;
        customCache: ICache;

        localStorage: Storage;
        sessionStorage: Storage;
    }
}