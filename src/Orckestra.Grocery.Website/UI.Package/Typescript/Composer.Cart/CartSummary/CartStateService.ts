///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Cache/CacheProvider.ts' />
///<reference path='../../Cache/CacheError.ts' />
///<reference path='../../Repositories/CartRepository.ts' />
///<reference path='../../Events/EventHub.ts' />
///<reference path='../../Utils/Utils.ts' />
///<reference path='./ICartService.ts' />
///<reference path='./CartEvents.ts' />

module Orckestra.Composer {
    'use strict';

    export class CartStateService implements ICartStateService {

        private static instance: ICartStateService;
        private readonly eventHub: IEventHub;
        public VueFullCart: Vue;
        public VueCartMixins: any = [];
        protected cartService: ICartService;
        protected membershipService: IMembershipService;
       
       

        constructor() {
            this.eventHub = EventHub.instance();
            this.cartService = CartService.getInstance();
            this.membershipService = new MembershipService(new MembershipRepository());

           this.registerSubscriptions();
            
            CartStateService.instance = this;
        }

        private initialize() { 

            this.VueFullCart = new Vue({
                el: '#vueFullCart',
                mixins: this.VueCartMixins,
                data: {
                    Cart: undefined,
                    IsAuthenticated: false,
                    Mode: {
                        Loading: false,
                        Busy: true
                    },
                    Errors: {                     
                    }
                },
                computed: {
                    IsLoading() {
                        return this.Mode.Loading;
                    },
                    IsBusy() {
                        return this.Mode.Busy;
                    },
                    OrderSummary() {
                        return this.Cart.OrderSummary
                    }
                },
                methods: {
                    updateBeforeEditLineItemList() {
                        this.beforeEditLineItemList = this.Cart.LineItemDetailViewModels.map(x => ({ ...x}));
                    }
                }
            });

            let authenticatedPromise = this.membershipService.isAuthenticated();
            let getCartPromise = this.cartService.getFreshCart().fail(reason => this.loadCartFailed(reason));
            Q.all([authenticatedPromise, getCartPromise])
                .spread((authVm, cartVm) => {
                    this.publishToAnalytics(cartVm);
                    this.eventHub.publish(CartEvents.CartUpdated, { data: cartVm });
                    let vueData: any = this.VueFullCart;
                    vueData.IsAuthenticated = authVm.IsAuthenticated;
                });
       }

        private publishToAnalytics(cartVm: any) {
            var e: IEventInformation = {
                data: {
                    Cart: cartVm,
                    StepNumber: 'cart'
                }
            };

            this.eventHub.publish('checkoutStepRendered', e);
        }

        protected registerSubscriptions() {

            this.eventHub.subscribe('allControllersInitialized', () => {
                this.initialize();
            });
            this.eventHub.subscribe(CartEvents.CartUpdated, e => this.onCartUpdated(e.data));
            this.eventHub.subscribe(FulfillmentEvents.StoreUpdating, e => this.onStoreUpdating(e.data));
        }

        public static getInstance(): ICartStateService {

            if (!CartStateService.instance) {
                CartStateService.instance = new CartStateService();
            }

            return CartStateService.instance;
        }

        protected onCartUpdated(cart: any): void {

            let vueData: any = this.VueFullCart;
            if (vueData) {
                vueData.Cart = cart;
                vueData.updateBeforeEditLineItemList();
                vueData.Mode.Loading = false;
                vueData.Mode.Busy = false;
            }

            ErrorHandler.instance().removeErrors();
        }

        protected onStoreUpdating(store: any): void {
            let vueData: any = this.VueFullCart;
            if (vueData) {
                vueData.Mode.Busy = true;
            }
        }

        protected loadCartFailed(reason: any): void {
            console.error('Error while loading the cart.', reason);
            ErrorHandler.instance().outputErrorFromCode('LoadCartFailed');
        }

    }
}
