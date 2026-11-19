///<reference path='../../../Typings/tsd.d.ts' />

namespace Orckestra.Composer {
    export interface IStoreService {

        getStores(): Q.Promise<any>;
    }
}