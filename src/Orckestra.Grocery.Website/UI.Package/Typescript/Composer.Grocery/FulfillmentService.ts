///<reference path='../../Typings/tsd.d.ts' />
///<reference path='../Mvc/IControllerContext.ts' />
///<reference path='../Mvc/ComposerClient.ts' />
///<reference path='./IFulfillmentService.ts' />
///<reference path='../Utils/Utils.ts' />
///<reference path='../Cache/CacheProvider.ts' />
///<reference path='../ErrorHandling/ErrorHandler.ts' />
///<reference path='../Composer.Cart/CartSummary/CartService.ts' />
///<reference path='../Composer.Grocery/TimeSlotsHelper.ts' />

module Orckestra.Composer {
    'use strict';

    export class FulfillmentService implements IFulfillmentService {
        private static _instance: FulfillmentService = new FulfillmentService();
        protected cartService = CartService.getInstance();
        protected cacheKeySelectedFulfillment: string = `Fulfillment|${Utils.getWebsiteId()}`;
        protected cacheKeyBackupSelectedFulfillment: string = `FulfillmentBackup|${Utils.getWebsiteId()}`;
        protected cacheProvider: ICacheProvider = CacheProvider.instance();
        protected cachePolicy: ICachePolicy = { slidingExpiration: 300 }; // 5min
        protected static GettingFreshFulfillment: Q.Promise<any>;
        protected cacheKeyDisabledForcingTimeSlot: string = `DisabledTimeSlotSelection|${Utils.getWebsiteId()}`;
        

        public static instance(): FulfillmentService {
            return FulfillmentService._instance;
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

        public getSelectedFulfillment(): Q.Promise<any> {
            return this.getCacheSelectedFulfillment()
                .then((store) => {
                    let { TimeSlotReservation } = store;

                    if (TimeSlotReservation && !TimeSlotsHelper.validateTimeSlotExpiration(TimeSlotReservation)) {
                        return this.getFreshSelectedFulfillment();
                    }

                    return store;
                })
                .fail(() => this.getFreshSelectedFulfillment());
        }

        public getFreshSelectedFulfillment(): Q.Promise<any> {
            if (!FulfillmentService.GettingFreshFulfillment) {
                FulfillmentService.GettingFreshFulfillment = ComposerClient.get('/api/storeandfulfillmentselection/getSelectedFulfillment')
                    .then(store => {
                        this.setFulfillmentToCache(store);
                        return store;
                    });
            }

            // to avoid getting a fresh store multiple times within a page session
            return FulfillmentService.GettingFreshFulfillment
                .fail((reason) => {
                    console.error('An error occurred while getting a fresh store.', reason);
                    throw reason;
                });
        }

        public setOrderFulfillment(OrderNumber: string): Q.Promise<any> {
            return this.cartService.getCart()
                .then(cart => {
                    if (cart.CartType != 'OrderDraft') {
                        return this.getSelectedFulfillment();
                    } else {
                        return false;
                    }
                })
                .then(fulfillment => this.setBackupFulfillmentToCache(fulfillment))
                .then(() => ComposerClient.post('/api/storeandfulfillmentselection/setOrderFulfillment', OrderNumber))
                .fail((reason) => {
                    ErrorHandler.instance().outputErrorFromCode('EditingOrderFailed');
                    throw reason;
                }).fin(() => this.invalidateCache());
        }

        public restoreFulfillment(fromBackup: boolean = false) {
            this.invalidateCache();
            if (fromBackup) {
                return this.getCacheBackupSelectedFulfillment()
                .then(storeNumber =>
                    ComposerClient.post('/api/storeandfulfillmentselection/setStoreFulfillment', storeNumber)
                )
                .fail(() => ComposerClient.get('/api/storeandfulfillmentselection/recoverFulfillment'));
            } else {
                return ComposerClient.get('/api/storeandfulfillmentselection/recoverFulfillment');
            }
        }

        protected getCacheSelectedFulfillment(): Q.Promise<any> {
            return this.cacheProvider.sessionCache.get<any>(this.cacheKeySelectedFulfillment);
        }

        protected setFulfillmentToCache(store: any): Q.Promise<any> {
            return this.cacheProvider.sessionCache.set(this.cacheKeySelectedFulfillment, store, this.cachePolicy);
        }

        protected getCacheBackupSelectedFulfillment(): Q.Promise<any> {
            return this.cacheProvider.sessionCache.get<any>(this.cacheKeyBackupSelectedFulfillment);
        }

        protected setBackupFulfillmentToCache(fulfillment: any): Q.Promise<any> {
            if (fulfillment) {
                return this.cacheProvider.sessionCache.set(this.cacheKeyBackupSelectedFulfillment, fulfillment.Store.Number, this.cachePolicy);
            }
        }

        public invalidateCache(): Q.Promise<void> {
            FulfillmentService.GettingFreshFulfillment = null;
            return this.cacheProvider.sessionCache.clear(this.cacheKeySelectedFulfillment);
        }

        public setFulfillment(storeId: any, methodType: any) {
            this.invalidateCache();
            return ComposerClient.post('/api/storeandfulfillmentselection/setFulfillment', { StoreId: storeId, FulfillmentMethodType: methodType });
        }

        public getTimeSlots(FulfillmentMethodTypeString: string, id: any, ShipmentId: any): Q.Promise<any> {
            return ComposerClient.post('/api/storeandfulfillmentselection/gettimeslots', { ShipmentId, FulfillmentMethodTypeString, StoreId: id });
        }

        public setFulFilledMethodType(fulfillmentMethodType: FulfillmentMethodTypes) {
            this.invalidateCache();
            return ComposerClient.post('/api/storeandfulfillmentselection/setselectedfulfilledmethod', { FulfillmentMethodType: fulfillmentMethodType });
        }

        public setTimeSlotId(StoreId: any, ShipmentId: any, SlotId: any, Date: any): Q.Promise<any> {
            return Q.all([
                this.invalidateCache(),
                ComposerClient.post('/api/storeandfulfillmentselection/setSelectedTimeslot', { StoreId, ShipmentId, SlotId, Date })
            ]).then(([, reservedSlot]) => reservedSlot);
        }

    }
}
