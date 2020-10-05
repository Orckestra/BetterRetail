///<reference path='../../Typings/tsd.d.ts' />
///<reference path='../Mvc/IControllerContext.ts' />
///<reference path='../Mvc/ComposerClient.ts' />
///<reference path='./ISelectedStoreService.ts' />
///<reference path='../Utils/Utils.ts' />
///<reference path='../Cache/CacheProvider.ts' />

module Orckestra.Composer {
    'use strict';

    export class SelectedStoreService implements ISelectedStoreService {
        private static _instance: SelectedStoreService = new SelectedStoreService();
        protected cacheKeyDefaultStore: string = `DefaultStore|${Utils.getWebsiteId()}`;
        protected cacheProvider: ICacheProvider = CacheProvider.instance();
        protected cachePolicy: ICachePolicy = { slidingExpiration: 300 }; // 5min
		protected static GettingFreshDefaultStoreCart: Q.Promise<any>;
        protected cacheKeyDisabledForcingTimeSlot: string = `DisabledTimeSlotSelection|${Utils.getWebsiteId()}`;

        public static instance(): SelectedStoreService {
            return SelectedStoreService._instance;
        }

        public disableForcingToSelectStore() {
            return ComposerClient.post('/api/storeandfulfillmentselection/disableforcingtoselect', null);
        }

        public disableForcingToSelectTimeSlot(): Q.Promise<any> {
            return this.cacheProvider.sessionCache.set(this.cacheKeyDisabledForcingTimeSlot, true);
        }

        public isForcingToSelectTimeSlotDisabled(): Q.Promise<boolean> {
            return this.cacheProvider.sessionCache.get<boolean>(this.cacheKeyDisabledForcingTimeSlot)
                .fail(() => false);
        }

        public setStore(id: any) {
            this.invalidateCache();
            return ComposerClient.post('/api/storeandfulfillmentselection/setSelectedStore', { StoreId: id });
        }

        public getStore(): Q.Promise<any> {
            return this.getCacheDefaultStore()
                .then((store) => {
                    let { TimeSlotReservation } = store;

                    if (TimeSlotReservation && !this.validateTimeSlotExpiration(TimeSlotReservation)) {
                        return this.getFreshStore();
                    }
                   
                    return store;
                })
                .fail(() => this.getFreshStore());
        }

		public getFreshStore(): Q.Promise<any> {
            if (!SelectedStoreService.GettingFreshDefaultStoreCart) {
                SelectedStoreService.GettingFreshDefaultStoreCart = ComposerClient.get('/api/storeandfulfillmentselection/getSelectedStore')
                    .then(store => {
                        this.setStoreToCache(store);
                        return store;
                    });
            }

            // to avoid getting a fresh store multiple times within a page session
            return SelectedStoreService.GettingFreshDefaultStoreCart
                .fail((reason) => {
                    console.error('An error occurred while getting a fresh store.', reason);
                    throw reason;
                });
        }

        protected getCacheDefaultStore(): Q.Promise<any> {
            return this.cacheProvider.sessionCache.get<any>(this.cacheKeyDefaultStore);
        }

        protected setStoreToCache(store: any): Q.Promise<any> {
			return this.cacheProvider.sessionCache.set(this.cacheKeyDefaultStore, store, this.cachePolicy);
        }

        public invalidateCache(): Q.Promise<void> {
            SelectedStoreService.GettingFreshDefaultStoreCart = null;
			return this.cacheProvider.sessionCache.clear(this.cacheKeyDefaultStore);
        }

        public getTimeSlots(FulfillmentMethodTypeString: string, id: any, ShipmentId: any): Q.Promise<any> {
            return ComposerClient.post('/api/storeandfulfillmentselection/gettimeslots', { ShipmentId, FulfillmentMethodTypeString, StoreId: id });
        }

        public setFulFilledMethodType(fulfillmentMethodType: FulfillmentMethodTypes) {
            this.invalidateCache();
            return ComposerClient.post('/api/storeandfulfillmentselection/setselectedfulfilledmethod', { FulfillmentMethodTypeString: fulfillmentMethodType });
        }

        public setTimeSlotId(StoreId: any, ShipmentId: any, SlotId: any, Date: any): Q.Promise<any>  {
			return Q.all([
				this.invalidateCache(),
				ComposerClient.post('/api/storeandfulfillmentselection/setSelectedTimeslot', { StoreId, ShipmentId, SlotId, Date })
			]).then(([, reservedSlot]) => reservedSlot);
        }

        public validateTimeSlotExpiration(timeSlotReservation): boolean {
            let slotTime = new Date(Date.parse(timeSlotReservation.ExpiryDateTime));
            let now = new Date();
            return !(slotTime < now)
        }
    }
}
