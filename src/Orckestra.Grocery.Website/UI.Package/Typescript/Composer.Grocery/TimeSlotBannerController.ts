///<reference path='../../Typings/tsd.d.ts' />
///<reference path='../Mvc/Controller.ts' />
///<reference path='./FulfillmentService.ts' />
///<reference path='./FulfillmentEvents.ts' />
///<reference path='../../Typings/moment/moment.d.ts' />
///<reference path='../Composer.SingleCheckout/BaseSingleCheckoutController.ts' />

module Orckestra.Composer {
    export class TimeSlotBannerController extends Controller {
        protected fulfillmentService: IFulfillmentService = FulfillmentService.instance();
        protected VueTimeSlotBanner: Vue;

        public initialize() {
            super.initialize();

            this.fulfillmentService.getSelectedFulfillment()
                .then((fulfillment) => this.initializeVueComponent(fulfillment));
        }

        private initializeVueComponent(fulfillment) {
            let self: TimeSlotBannerController = this;
            let commonFulfillmentOptions = FulfillmentHelper.getCommonSelectedFulfillmentStateOptions();
            commonFulfillmentOptions.data.SelectedFulfillment = fulfillment;
            this.VueTimeSlotBanner = new Vue({
                el: '#vueTimeSlotBanner',
                data: {
                    ...commonFulfillmentOptions.data
                },
                mounted() {
                    self.eventHub.subscribe(FulfillmentEvents.TimeSlotSelected, e => this.onSlotSelected(e.data));
                },
                computed: {
                    ...commonFulfillmentOptions.computed
                },
                methods: {
                    ...commonFulfillmentOptions.methods
                }
            });
        }
    }
}