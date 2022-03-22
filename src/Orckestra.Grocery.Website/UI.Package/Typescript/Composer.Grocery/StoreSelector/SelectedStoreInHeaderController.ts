///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Mvc/Controller.ts' />
///<reference path='../FulfillmentService.ts' />
///<reference path='../../UI/UIModal.ts' />
///<reference path='../FulfillmentEvents.ts' />
///<reference path='../../Composer.Cart/CartSummary/CartService.ts' />
///<reference path='../../Composer.Grocery/FulfillmentHelper.ts' />

module Orckestra.Composer {
    export class SelectedStoreInHeaderController extends Controller {
        public SelectedStoreInHeader: Vue;
        public SelectedStoreInSticky: Vue;
        protected fulfillmentService: IFulfillmentService = FulfillmentService.instance();
        private cartService: ICartService = CartService.getInstance();

        public initialize() {
            super.initialize();
            let getCartPromise = this.cartService.getCart();
            let getFulfilment = this.fulfillmentService.getSelectedFulfillment();
            Q.all([getCartPromise, getFulfilment])
                .spread((cart, fulfillment) => {
                    this.initializeVueComponent(cart, fulfillment);
                });
        }

        private initializeVueComponent(cart: any, fulfillment: any) {
            let self: SelectedStoreInHeaderController = this;
            let commonFulfillmentOptions =  FulfillmentHelper.getCommonSelectedFulfillmentStateOptions();
            this.SelectedStoreInHeader = new Vue({
                el: '#vueSelectedStoreInHeader',
                data: {
                     ...commonFulfillmentOptions.data,
                     Cart: cart
                },
                mounted() {
                    self.eventHub.subscribe(FulfillmentEvents.FulfillmentMethodSelected,  e => this.onMethodSelected(e.data));
                    self.eventHub.subscribe(FulfillmentEvents.StoreSelected,  e => this.onStoreSelected(e.data));
                    self.eventHub.subscribe(FulfillmentEvents.StoreUpdating, e => this.onStoreUpdating(e.data));
                    self.eventHub.subscribe(FulfillmentEvents.TimeSlotSelected,  e => this.onSlotSelected(e.data));
                    self.eventHub.subscribe(FulfillmentEvents.TimeSlotUpdating, e => this.onSlotUpdating(e.data));
                    self.eventHub.subscribe(FulfillmentEvents.TimeSlotSelectionFailed, e => this.onSlotFailed(e.data));
                    self.eventHub.subscribe(CartEvents.CartUpdated, e => this.Cart = e.data);
                    this.onFulfillmentLoaded(fulfillment);
                },
                computed: {
                    TimeSlotReservationDisplay() {
                        if(!this.TimeSlotReservationTentative||!this.TimeSlot) return '';
                        const reserveDate = moment(this.TimeSlotReservation.ReservationDate).utc().format('ddd DD MMM');
                        const startTime = moment(this.TimeSlot.SlotBeginTime, 'HH:mm:ss').format('hh:mm');
                        const endTime = moment(this.TimeSlot.SlotEndTime, 'HH:mm:ss').format('hh:mma').toLowerCase();
                        return `${reserveDate} - ${startTime}-${endTime}`
                    },
                    ...commonFulfillmentOptions.computed
                },
                methods: {
                    ...commonFulfillmentOptions.methods,
                    navigateToStoreSelector() {
                        $('html, body').animate({
                            scrollTop: ($('#vueStoreSelector').offset().top - 50)
                        }, 500);
                    }
                }
            });

            this.SelectedStoreInSticky = new Vue({
                el: '#vueSelectedStoreInSticky',
                data: {
                     ...commonFulfillmentOptions.data,
                     Cart: cart
                },
                mounted() {
                    self.eventHub.subscribe(FulfillmentEvents.FulfillmentMethodSelected,  e => this.onMethodSelected(e.data));
                    self.eventHub.subscribe(FulfillmentEvents.StoreSelected,  e => this.onStoreSelected(e.data));
                    self.eventHub.subscribe(FulfillmentEvents.StoreUpdating, e => this.onStoreUpdating(e.data));
                    self.eventHub.subscribe(FulfillmentEvents.TimeSlotSelected,  e => this.onSlotSelected(e.data));
                    self.eventHub.subscribe(FulfillmentEvents.TimeSlotUpdating, e => this.onSlotUpdating(e.data));
                    self.eventHub.subscribe(FulfillmentEvents.TimeSlotSelectionFailed, e => this.onSlotFailed(e.data));
                    this.onFulfillmentLoaded(fulfillment);
                },
                computed: {
                    TimeSlotReservationDisplay() {
                        if(!this.TimeSlotReservationTentative||!this.TimeSlot) return '';
                        const reserveDate = moment(this.TimeSlotReservation.ReservationDate).utc().format('ddd DD MMM');
                        const startTime = moment(this.TimeSlot.SlotBeginTime, 'HH:mm:ss').format('hh:mm');
                        const endTime = moment(this.TimeSlot.SlotEndTime, 'HH:mm:ss').format('hh:mma').toLowerCase();
                        return `${reserveDate} - ${startTime}-${endTime}`
                    },
                    ...commonFulfillmentOptions.computed
                },
                methods: {
                    ...commonFulfillmentOptions.methods,
                    navigateToStoreSelector() {
                        $('html, body').animate({
                            scrollTop: ($('#vueStoreSelector').offset().top - 50)
                        }, 500);
                    }
                }
            });
        }
    }
}
