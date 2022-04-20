///<reference path='../../Typings/tsd.d.ts' />


module Orckestra.Composer {

    export enum FulfillmentEvents {
        LocationSelected = 'locationSelected',
        CheckAvailability = 'checkAvailability',
        SubscribeForAvailability = 'subscribeForAvailability',
        FulfillmentMethodSelected = 'fulfillmentMethodSelected',
        StoreUpdating = 'storeUpdating',
        StoreSelected = 'storeSelected',
        StoreSelectionFailed = 'storeSelectionFailed',
        TimeSlotSelected = 'timeSlotSelected',
        TimeSlotUpdating = 'timeSlotUpdating',
        TimeSlotSelectionFailed = 'timeSlotSelectionFailed'
    }
}