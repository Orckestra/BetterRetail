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

    export class MiniCartController extends Orckestra.Composer.Controller {

        private cartService: ICartService = CartService.getInstance();
        protected fulfillmentService: IFulfillmentService = FulfillmentService.instance();
        private orderSummaryService: OrderSummaryService = new OrderSummaryService(this.cartService, this.eventHub);

        public initialize() {

            super.initialize();
            let getCartPromise = this.cartService.getCart();
            let getFulfilment = this.fulfillmentService.getSelectedFulfillment();
            Q.all([getCartPromise, getFulfilment])
                .spread((cart, fulfillment) => {
                    this.initializeMiniCartQuantity(cart, fulfillment);
                });
            this.eventHub.subscribe(GeneralEvents.LanguageSwitched, (e: IEventInformation) => this.cartService.invalidateCache());
        }

        protected initializeMiniCartQuantity(cart, fulfillment): void {
            let self: MiniCartController = this;
            let commonFulfillmentOptions = FulfillmentHelper.getCommonSelectedFulfillmentStateOptions(fulfillment);
            new Vue({
                el: "#vueMiniCart",
                data: {
                    Cart: cart,
                    AddedCartItem: false,
                    ...commonFulfillmentOptions.data
                },
                computed: {
                    CheckoutButtonDisabled() {
                        return !this.IsStoreSelected;
                    },
                    ...commonFulfillmentOptions.computed
                },
                mounted() {
                    var loggedInScheduler = EventScheduler.instance(MyAccountEvents[MyAccountEvents.LoggedIn]);
                    var loggedOutScheduler = EventScheduler.instance(MyAccountEvents[MyAccountEvents.LoggedOut]);
                    self.eventHub.subscribe('cartUpdated', (e: IEventInformation) => this.onCartUpdated(e));
                    loggedOutScheduler.subscribe((e: IEventInformation) => self.cartService.invalidateCache());
                    loggedInScheduler.subscribe((e: IEventInformation) => self.cartService.invalidateCache());
                    self.eventHub.subscribe('lineItemAddedToCart', (e: IEventInformation) => this.AddedCartItem = e.data.ProductId + '-' + (e.data.VariantId || 'null'));
                    self.eventHub.subscribe(FulfillmentEvents.StoreSelected, e => this.onStoreSelected(e.data));
                },
                updated() {
                    if (this.AddedCartItem) {
                        this.displayMiniCart();
                    }
                },
                methods: {
                    onCartUpdated(e) {
                        this.Cart = e.data;
                    },
                    displayMiniCart(e: IEventInformation): void {
                        let miniCartContainer = $('#minicart-summary');
                        let notificationTime = parseInt(miniCartContainer.data('notificationTime'), 10);
                        let scrollToLineItemKey = this.AddedCartItem;

                        //To reset timer
                        clearTimeout(this.timer);

                        if (notificationTime > 0) {
                            miniCartContainer.addClass('displayMiniCart');

                            // Scroll to added item
                            $('.minicart-summary-products', miniCartContainer).stop().animate({
                                scrollTop: $('[data-lineitem-id="' + scrollToLineItemKey + '"]', miniCartContainer).position().top
                            }, 1000);

                            this.timer = setTimeout(function () {
                                miniCartContainer.removeClass('displayMiniCart');
                                this.AddedCartItem = false;
                            }, notificationTime);
                        }
                    },
                    onCloseMiniCart(e: IEventInformation): void {
                        let miniCartContainer = $('#minicart-summary');

                        miniCartContainer.addClass('d-none');
                        setTimeout(function () {
                            miniCartContainer.removeClass('d-none');
                        }, 250);

                        //Hide the display and cancel the display timer to not have a flickering display
                        miniCartContainer.removeClass('displayMiniCart');
                        clearTimeout(this.timer);
                    },
                    proceedToCheckout(): void {
                        let nextStepUrl = this.Cart.OrderSummary.CheckoutUrlTarget;
                        if (!nextStepUrl) {
                            throw 'No next step Url was defined.';
                        }

                        // Set origin of checkout that we will be used in AnalyticsPlugin
                        AnalyticsPlugin.setCheckoutOrigin('Mini Cart Checkout');

                        self.orderSummaryService.cleanCart().done(() => {
                            window.location.href = nextStepUrl;
                        }, reason => {
                            console.error('Error while proceeding to Checkout', reason);
                            ErrorHandler.instance().outputErrorFromCode('ProceedToCheckoutFailed');
                        });
                    },
                    ...commonFulfillmentOptions.methods
                }
            })
        }
    }
}
