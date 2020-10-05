///<reference path='../../../Typings/tsd.d.ts' />

module Orckestra.Composer {
    export interface IStoreService {

        getStores(): Q.Promise<any>;

        getStoresByLocation(location: any): Q.Promise<any>;

        filterStoresByFulfillmentMethod(stores: any[], method: FulfillmentMethodTypes): any[];
    }
}