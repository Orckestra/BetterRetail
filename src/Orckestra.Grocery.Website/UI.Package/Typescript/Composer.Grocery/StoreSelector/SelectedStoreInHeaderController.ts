///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Mvc/Controller.ts' />
///<reference path='../SelectedStoreService.ts' />
///<reference path='../../UI/UIModal.ts' />
///<reference path='../SelectedStoreEvents.ts' />
/// <reference path='../TimeSlotsHelper.ts' />

module Orckestra.Composer {
    export class SelectedStoreInHeaderController extends Controller {
        public SelectedStoreInHeader: Vue;
        protected selectedStoreService: ISelectedStoreService = SelectedStoreService.instance();

        public initialize() {
            super.initialize();

            this.selectedStoreService.getStore()
                .then(defaultStore => {
                    this.initializeVueComponent(defaultStore);
                });
        }

        private initializeVueComponent(selectedStore: any) {
            let self: SelectedStoreInHeaderController = this;
            let commonTimeSlotReservationOptions =  TimeSlotsHelper.getCommonTimeSlotReservationVueConfig();
            this.SelectedStoreInHeader = new Vue({
                el: '#vueSelectedStoreInHeader',
                data: {
                    SelectedStore: {
                        TimeSlot: undefined,
                        TimeSlotReservation: undefined,
                        Store: undefined,
                        ...selectedStore,
                        StoreLoading: false, TimeSlotLoading: false
                    },
                    ChangeStoreModal: null,
                    Errors: {
                        TimeSlotSelectionError: false
                    }
                },
                mounted() {
                    this.ChangeStoreModal = new UIModal(window, "#changeStoreModal", this.selectStore, this);
                    self.eventHub.subscribe(SelectedStoreEvents.StoreSelected,  e => this.onStoreSelected(e.data));
                    self.eventHub.subscribe(SelectedStoreEvents.StoreUpdating, e => this.onStoreUpdating(e.data));
                    self.eventHub.subscribe(SelectedStoreEvents.TimeSlotSelected,  e => this.onSlotSelected(e.data));
                    self.eventHub.subscribe(SelectedStoreEvents.TimeSlotUpdating, e => this.onSlotUpdating(e.data));
                    self.eventHub.subscribe(SelectedStoreEvents.TimeSlotSelectionFailed, e => this.onSlotFailed(e.data));
                },
                computed: {
                    Store() {
                        return this.SelectedStore.Store;
                    },
                    TimeSlotReservation() {
                        return this.SelectedStore.TimeSlotReservation;
                    },
                    TimeSlot() {
                        return this.SelectedStore.TimeSlot;
                    },
					...commonTimeSlotReservationOptions.computed,
                    TimeSlotReservationDisplay() {
                        if(!this.TimeSlotReservationTentative||!this.TimeSlot) return '';

                        const reserveDate = moment(this.TimeSlotReservation.ReservationDate).utc().format('ddd DD MMM');
                        const startTime = moment(this.TimeSlot.SlotBeginTime, 'HH:mm:ss').format('hh:mm');
                        const endTime = moment(this.TimeSlot.SlotEndTime, 'HH:mm:ss').format('hh:mma').toLowerCase();
                        return `${reserveDate} - ${startTime}-${endTime}`
                    }
                },
                methods: {
                    changeStoreModal(event: JQueryEventObject, changeStore: boolean) {
                        this.ChangeStoreModal.openModal(event, {ChangeStore: changeStore});
                    },
                    selectStore(event: JQueryEventObject): Q.Promise<any> {
                        let element = $(event.target);

                        return Q.resolve(1);
                    },
                    onStoreSelected(store: any) {
                        this.SelectedStore.Store = store;
                        this.SelectedStore.StoreLoading = false;
                    },
                    onSlotSelected({ TimeSlot, TimeSlotReservation }) {
                        this.SelectedStore.TimeSlot = TimeSlot;
                        this.SelectedStore.TimeSlotReservation = TimeSlotReservation;
                        this.SelectedStore.TimeSlotLoading = false;
                    },
                    onSlotUpdating(data) {
                        this.SelectedStore.TimeSlotLoading = true;
                        this.Errors.TimeSlotSelectionError = false;
                    },
                    onStoreUpdating(data) {
                        this.SelectedStore.StoreLoading = true;

                    },
                    onSlotFailed(data) {
                        this.Errors.TimeSlotSelectionError = TimeSlotsHelper.getTimeSlotReservationError(data);
                    }
                }
            });
        }
    }
}
