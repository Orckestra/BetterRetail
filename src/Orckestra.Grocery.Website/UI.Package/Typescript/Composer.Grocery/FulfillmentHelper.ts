///<reference path='../../Typings/tsd.d.ts' />
///<reference path='./TimeSlotsHelper.ts' />
module Orckestra.Composer {
    'use strict';

    export class FulfillmentHelper {

        //get common Vue State for Selected Fulfillment
        public static getCommonSelectedFulfillmentStateOptions(fulfillment: any = undefined): any {
            return {
                data: {
                    ChangeStoreModal: new UIModal(window, "#changeStoreModal", null, this),
                    SelectedFulfillment: {
                        TimeSlot: fulfillment ? fulfillment.TimeSlot : undefined,
                        TimeSlotReservation: fulfillment ? fulfillment.TimeSlotReservation : undefined,
                        Store: fulfillment ? fulfillment.Store : undefined,
                        FulfillmentMethodType: fulfillment ? fulfillment.FulfillmentMethodType : undefined,
                        StoreLoading: false,
                        TimeSlotLoading: false
                    },
                    Errors: {
                        StoreSelectionError: false,
                        TimeSlotLoadingError: false,
                        TimeSlotSelectionError: false
                    }
                },
                computed: {
                    FulfillmentMethodType() {
                        return this.SelectedFulfillment ? this.SelectedFulfillment.FulfillmentMethodType: undefined;
                    },
                    Store() {
                        return this.SelectedFulfillment ? this.SelectedFulfillment.Store: undefined;
                    },
                    IsStoreSelected() {
                        return !!(this.SelectedFulfillment && this.SelectedFulfillment.Store);
                    },
                    TimeSlotReservation() {
                        return this.SelectedFulfillment.TimeSlotReservation;
                    },
                    TimeSlot() {
                        return this.SelectedFulfillment.TimeSlot;
                    },
                    IsTimeSlotReserved() {
                        return !!(this.SelectedFulfillment && this.SelectedFulfillment.TimeSlotReservation);
                    },
                    TimeSlotReservationExpireTime() {
                        return this.SelectedFulfillment && TimeSlotsHelper.getTimeSlotReservationExpireTime(this.SelectedFulfillment.TimeSlotReservation);
                    },
                    TimeSlotReservationExpireDate() {
                        return this.SelectedFulfillment && TimeSlotsHelper.getTimeSlotReservationExpireDate(this.SelectedFulfillment.TimeSlotReservation);
                    },
                    TimeSlotReservationExpireRelativeDayIndex() {
                        return this.SelectedFulfillment && TimeSlotsHelper.getTimeSlotReservationExpireDayIndex(this.SelectedFulfillment.TimeSlotReservation);
                    },
                    TimeSlotReservationExpired() {
                        return this.SelectedFulfillment && TimeSlotsHelper.isTimeSlotReservationExpired(this.SelectedFulfillment.TimeSlotReservation);
                    },
                    TimeSlotReservationTentative() {
                        return this.SelectedFulfillment && TimeSlotsHelper.isTimeSlotReservationTentative(this.SelectedFulfillment.TimeSlotReservation);
                    }
                    
                },
                methods: {
                    changeStoreModal(event: JQueryEventObject, changeMode: boolean) {
                        this.ChangeStoreModal.openModal(event, { ChangeMode: changeMode });
                    },
                    onMethodSelected(methodType: any) {
                        this.SelectedFulfillment.FulfillmentMethodType = methodType;
                    },
                    onFulfillmentLoaded({ FulfillmentMethodType, Store, TimeSlot, TimeSlotReservation }: any) {
                        this.SelectedFulfillment.FulfillmentMethodType = FulfillmentMethodType;
                        this.SelectedFulfillment.Store = Store;
                        this.SelectedFulfillment.TimeSlot = TimeSlot;
                        this.SelectedFulfillment.TimeSlotReservation = TimeSlotReservation;
                        this.SelectedFulfillment.TimeSlotLoading = false;
                        this.SelectedFulfillment.StoreLoading = false;
                    },
                    onStoreUpdating(data) {
                        this.SelectedFulfillment.StoreLoading = true;
                    },
                    onStoreSelected(store: any) {
                        this.SelectedFulfillment.Store = store;
                        this.SelectedFulfillment.StoreLoading = false;
                    },
                    onSlotSelected({ TimeSlot, TimeSlotReservation }) {
                        this.SelectedFulfillment.TimeSlot = TimeSlot;
                        this.SelectedFulfillment.TimeSlotReservation = TimeSlotReservation;
                        this.SelectedFulfillment.TimeSlotLoading = false;
                        this.SelectedFulfillment.StoreLoading = false;
                    },
                    onSlotUpdating(data) {
                        this.SelectedFulfillment.TimeSlotLoading = true;
                        this.Errors.TimeSlotSelectionError = false;
                    },
                    onSlotFailed(data) {
                        this.Errors.TimeSlotSelectionError = TimeSlotsHelper.getTimeSlotReservationError(data);
                    }
                }
            }
        }
    }
}