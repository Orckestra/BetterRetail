///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Mvc/Controller.ts' />
///<reference path='../FulfillmentService.ts' />
/// <reference path='../../Composer.Cart/CartSummary/CartService.ts' />
/// <reference path='../../Composer.Cart/CartSummary/CartStateService.ts' />
/// <reference path='../../Composer.Cart/CartSummary/ICartStateService.ts' />
///<reference path='../../UI/UIModal.ts' />
///<reference path='../FulfillmentEvents.ts' />
///<reference path='../FulfillmentHelper.ts' />

module Orckestra.Composer {
    'use strict';

    export class SelectedStoreInCartController extends Controller {

        protected storeService: IFulfillmentService = FulfillmentService.instance();
        protected cartService = CartService.getInstance();
        protected cartStateService: ICartStateService = CartStateService.getInstance();
        public initialize() {
            super.initialize();
            this.initializeVueComponent();

			this.storeService.getFreshSelectedFulfillment().then((fulfillment) => {
                let vueData: any = this.cartStateService.VueFullCart;
                vueData.onFulfillmentLoaded(fulfillment);
			}); 
        }

        private initializeVueComponent() {
            let self: SelectedStoreInCartController = this;
            let commonFulfillmentOptions =  FulfillmentHelper.getCommonSelectedFulfillmentStateOptions();
            let fulfillmentSummaryMixins = {
          
                data: {
                    ...commonFulfillmentOptions.data
                },
                computed: {
                    ...commonFulfillmentOptions.computed
                },
                mounted() {
                    self.eventHub.subscribe(FulfillmentEvents.FulfillmentMethodSelected,  e => this.onMethodSelected(e.data));
                    self.eventHub.subscribe(FulfillmentEvents.StoreSelected,  e => this.onStoreSelected(e.data));
                    self.eventHub.subscribe(FulfillmentEvents.StoreUpdating, e => this.onStoreUpdating(e.data));
                    self.eventHub.subscribe(FulfillmentEvents.TimeSlotSelected,  e => this.onSlotSelected(e.data));
                    self.eventHub.subscribe(FulfillmentEvents.TimeSlotUpdating, e => this.onSlotUpdating(e.data));
                    self.eventHub.subscribe(FulfillmentEvents.TimeSlotSelectionFailed, e => this.onSlotFailed(e.data));
                },
                methods: {
                    ...commonFulfillmentOptions.methods
                },
            };

            this.cartStateService.VueCartMixins.push(fulfillmentSummaryMixins);
        }
    }
}
