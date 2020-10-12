///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Mvc/Controller.ts' />
///<reference path='../SelectedStoreService.ts' />
/// <reference path='../../Composer.Cart/CartSummary/CartService.ts' />
/// <reference path='../../Composer.Cart/CartSummary/ICartStateService.ts' />
///<reference path='../../UI/UIModal.ts' />
///<reference path='../SelectedStoreEvents.ts' />
/// <reference path='../TimeSlotsHelper.ts' />

module Orckestra.Composer {
    'use strict';

    export class SelectedStoreInCartController extends Controller {

        protected storeService: ISelectedStoreService = SelectedStoreService.instance();
        protected cartService = CartService.getInstance();
        protected cartStateService: ICartStateService = CartStateService.getInstance();
        public initialize() {
            super.initialize();
            this.initializeVueComponent();

			this.storeService.getFreshStore().then((selectedStore) => {
                let vueData: any = this.cartStateService.VueFullCart;
				vueData.SelectedStore = {...selectedStore, StoreLoading: false, TimeSlotLoading: false };
			}); 
        }

        private initializeVueComponent() {
            let self: SelectedStoreInCartController = this;
            let commonTimeSlotReservationOptions =  TimeSlotsHelper.getCommonTimeSlotReservationVueConfig();
            let storeSummaryMixins = {
                data: {
                    SelectedStore: {
                        TimeSlotReservation: undefined,
                        Store: undefined,
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
                    ...commonTimeSlotReservationOptions.computed
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
            };

            this.cartStateService.VueCartMixins.push(storeSummaryMixins);
        }
    }
}
