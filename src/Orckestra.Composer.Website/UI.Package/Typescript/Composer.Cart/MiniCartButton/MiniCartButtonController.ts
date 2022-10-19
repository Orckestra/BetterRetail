///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Mvc/Controller.ts' />
///<reference path='../../Events/EventScheduler.ts' />
///<reference path='../../Repositories/CartRepository.ts' />
///<reference path='../../Events/EventHub.ts' />
///<reference path='../../Events/IEventInformation.ts' />
///<reference path='../CartSummary/CartService.ts' />
///<reference path='../../Composer.MyAccount/Common/MyAccountEvents.ts' />

module Orckestra.Composer {
    'use strict';

    export class MiniCartButtonController extends Orckestra.Composer.Controller {

        private cartService: ICartService = CartService.getInstance();

        public initialize() {
            super.initialize();
            this.cartService.getCart().then((cart) => {
                this.initializeMiniCartQuantity(cart);
            });
            var loggedInScheduler = EventScheduler.instance(MyAccountEvents[MyAccountEvents.LoggedIn]);
            var loggedOutScheduler = EventScheduler.instance(MyAccountEvents[MyAccountEvents.LoggedOut]);
            loggedOutScheduler.subscribe((e: IEventInformation) => this.cartService.invalidateCache());
            loggedInScheduler.subscribe((e: IEventInformation) => this.cartService.invalidateCache());
            this.eventHub.subscribe(MyAccountEvents.EditOrderStarted, (e: IEventInformation) => this.onEditOrderStarted(e));

        }

        protected onEditOrderStarted(e: IEventInformation): void {
            this.cartService.invalidateCache()
                .then(() => window.location = e.data.redirectUrl);
        }

        protected initializeMiniCartQuantity(cart): void {
            let self: MiniCartButtonController = this;
            const elements = ["#vueMiniCartButton", "#vueMiniCartButtonMobile"];
            elements.forEach(el => {
                new Vue({
                    el: el,
                    data: {
                        Cart: cart
                    },

                    mounted() {
                        self.eventHub.subscribe('cartUpdated', (e: IEventInformation) => this.onCartUpdated(e));
                    },
                    methods: {
                        onCartUpdated(e) {
                            this.Cart = e.data;
                        }
                    }
                })
            })
        }
    }
}
