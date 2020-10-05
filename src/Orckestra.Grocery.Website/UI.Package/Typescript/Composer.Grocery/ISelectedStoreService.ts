///<reference path='../../Typings/tsd.d.ts' />
///<reference path='../Composer.SingleCheckout/Enums/FulfillmentMethodTypes.ts' />

module Orckestra.Composer {
    export interface ISelectedStoreService {
        setStore(id: any): Q.Promise<any>;

        getStore(): Q.Promise<any>;

		getFreshStore(): Q.Promise<any>;

		disableForcingToSelectStore(): Q.Promise<any>;

        disableForcingToSelectTimeSlot(): Q.Promise<any>;

        isForcingToSelectTimeSlotDisabled(): Q.Promise<boolean>;

        getTimeSlots(fulfillmentMethodTypeString: string, id: any, shipmentId: any): Q.Promise<any>;

        setFulFilledMethodType(fulfillmentMethodType: FulfillmentMethodTypes);

        setTimeSlotId(StoreId: any, shipmentId: any, slotId: any, date: any): Q.Promise<any>;

        invalidateCache(): Q.Promise<void>;

        validateTimeSlotExpiration(timeSlotReservation): boolean;
    }
}
