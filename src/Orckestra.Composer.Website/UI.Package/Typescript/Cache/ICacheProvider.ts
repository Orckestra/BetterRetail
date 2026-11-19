/// <reference path='../../Typings/tsd.d.ts' />

namespace Orckestra.Composer {

    export interface ICacheProvider {

        defaultCache: ICache;
        sessionCache: ICache;

        localStorage: Storage;
        sessionStorage: Storage;
    }
}