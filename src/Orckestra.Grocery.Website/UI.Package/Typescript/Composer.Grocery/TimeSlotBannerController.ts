///<reference path='../../Typings/tsd.d.ts' />
///<reference path='../Mvc/Controller.ts' />
///<reference path='./SelectedStoreService.ts' />
///<reference path='./SelectedStoreEvents.ts' />
///<reference path='../../Typings/moment/moment.d.ts' />
///<reference path='../Composer.SingleCheckout/BaseSingleCheckoutController.ts' />
/// <reference path='./TimeSlotsHelper.ts' />

module Orckestra.Composer {
    export class TimeSlotBannerController extends Controller {
		protected storeService: ISelectedStoreService = SelectedStoreService.instance();
        protected VueTimeSlotBanner: Vue;

        public initialize() {
            super.initialize();

            this.storeService.getStore()
                .then(( selectedStore ) => this.initializeVueComponent(selectedStore));
        }

        private initializeVueComponent(selectedStore) {
            let self: TimeSlotBannerController = this;
            let commonTimeSlotReservationOptions =  TimeSlotsHelper.getCommonTimeSlotReservationVueConfig();
            this.VueTimeSlotBanner = new Vue({
                el: '#vueTimeSlotBanner',
                data: {
                    SelectedStore: selectedStore
                },
				mounted() {
                    this.ChangeStoreModal = new UIModal(window, "#changeStoreModal", () => {}, this);
                    self.eventHub.subscribe(SelectedStoreEvents.TimeSlotSelected,  e => this.onSlotSelected(e.data));
				},
                computed: {
                    ...commonTimeSlotReservationOptions.computed
                },
				methods: {
					onSlotSelected({ TimeSlotReservation }) {
                        this.SelectedStore.TimeSlotReservation = TimeSlotReservation;
                    },
                    changeStoreModal(event: JQueryEventObject, changeStore: boolean) {
                        this.ChangeStoreModal.openModal(event, {ChangeStore: changeStore});
                    }
				}
            });
        }
    }
}