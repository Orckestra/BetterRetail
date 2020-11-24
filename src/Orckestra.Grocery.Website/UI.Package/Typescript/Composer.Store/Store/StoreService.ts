///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Mvc/IControllerContext.ts' />
///<reference path='../../Mvc/ComposerClient.ts' />
///<reference path='./IStoreService.ts' />

module Orckestra.Composer {
    'use strict';

    export class StoreService implements IStoreService {
        private static _instance: StoreService = new StoreService();

        public static instance(): StoreService {
            return StoreService._instance;
        }

        public getStores() {
            return ComposerClient.get('/api/store/stores');
        }

        public getStoresByLocation(location: any) {
            return ComposerClient.post('/api/store/stores', { SearchPoint: location });
        }

        public filterStoresByFulfillmentMethod(stores: any[], method: FulfillmentMethodTypes): any[] {
            switch (method) {
                case FulfillmentMethodTypes.PickUp:
                    return stores.filter(store => store.SupportPickUp);
                case FulfillmentMethodTypes.Delivery:
                case FulfillmentMethodTypes.Shipping:
                    return stores.filter(store => store.SupportDelivery);
                default:
                    return stores;
            }
        }
    }
}
