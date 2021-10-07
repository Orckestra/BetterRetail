///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Mvc/Controller.ts' />
///<reference path='../../Events/EventScheduler.ts' />
///<reference path='../../Repositories/CartRepository.ts' />
///<reference path='../../Events/EventHub.ts' />
///<reference path='../../Events/IEventInformation.ts' />
///<reference path='../CartSummary/CartService.ts' />
///<reference path='../../Composer.MyAccount/Common/MyAccountEvents.ts' />
///<reference path='../../Composer.Grocery/FulfillmentHelper.ts' />
///<reference path='../../Composer.Grocery/FulfillmentService.ts' />
///<reference path='../../Composer.Grocery/FulfillmentEvents.ts' />
///<reference path='../../GeneralEvents.ts' />

module Orckestra.Composer {
    'use strict';

    export class MiniCartButtonController extends Orckestra.Composer.Controller {

        private cartService: ICartService = CartService.getInstance();
        protected fulfillmentService: IFulfillmentService = FulfillmentService.instance();

        public initialize() {
            super.initialize();
            let getCartPromise = this.cartService.getCart();
            let getFulfilment = this.fulfillmentService.getSelectedFulfillment();
            Q.all([getCartPromise, getFulfilment])
                .spread((cart, fulfillment) => {
                    this.initializeMiniCartQuantity(cart, fulfillment);
                });
            this.eventHub.subscribe(GeneralEvents.LanguageSwitched, (e: IEventInformation) => this.cartService.invalidateCache());
            var loggedInScheduler = EventScheduler.instance(MyAccountEvents[MyAccountEvents.LoggedIn]);
            var loggedOutScheduler = EventScheduler.instance(MyAccountEvents[MyAccountEvents.LoggedOut]);
            loggedOutScheduler.subscribe((e: IEventInformation) => this.cartService.invalidateCache());
            loggedInScheduler.subscribe((e: IEventInformation) => this.cartService.invalidateCache());
        }

        protected initializeMiniCartQuantity(cart, fulfillment): void {
            let self: MiniCartButtonController = this;
            let commonFulfillmentOptions = FulfillmentHelper.getCommonSelectedFulfillmentStateOptions(fulfillment);
            const elements = ["#vueMiniCartButton", "#vueMiniCartButtonMobile"];
            elements.forEach(el => {
                new Vue({
                    el: el,
                    data: {
                        Cart: cart,
                        ...commonFulfillmentOptions.data
                    },

                    mounted() {
                        self.eventHub.subscribe('cartUpdated', (e: IEventInformation) => this.onCartUpdated(e));
                        self.eventHub.subscribe(FulfillmentEvents.StoreSelected, e => this.onStoreSelected(e.data));
                    },
                    methods: {
                        onCartUpdated(e) {
                            this.Cart = e.data;
                        },
                        ...commonFulfillmentOptions.methods
                    }
                })
            })
        }
    }
}
