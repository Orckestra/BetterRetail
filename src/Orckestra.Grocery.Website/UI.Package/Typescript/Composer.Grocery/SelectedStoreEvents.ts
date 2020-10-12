///<reference path='../../Typings/tsd.d.ts' />


module Orckestra.Composer {

    export enum SelectedStoreEvents {
        LocationSelected = 'locationSelected',
        CheckAvailability = 'checkAvailability',
        SubscribeForAvailability = 'subscribeForAvailability',
        StoreUpdating = 'storeUpdating',
        StoreSelected = 'storeSelected',
        TimeSlotSelected = 'timeSlotSelected',
        TimeSlotUpdating = 'timeSlotUpdating',
        TimeSlotSelectionFailed = 'timeSlotSelectionFailed'
    }
}
