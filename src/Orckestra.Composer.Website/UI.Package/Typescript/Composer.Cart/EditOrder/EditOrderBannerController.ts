///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Mvc/Controller.ts' />
///<reference path='../CartSummary/CartService.ts' />
///<reference path='../OrderHistory/Services/OrderService.ts' />
///<reference path='../CartSummary/CartEvents.ts' />


module Orckestra.Composer {
    export class EditOrderBannerController extends Controller {
        protected cartService: ICartService = CartService.getInstance();
        protected orderService = new OrderService();
        protected VueEditOrderBanner: Vue;
        public initialize() {
            super.initialize();
            this.cartService.getCart()
                .then((currentCart) => this.initializeVueComponent(currentCart));
        }
        private initializeVueComponent(currentCart) {
            let self: EditOrderBannerController = this;
            this.VueEditOrderBanner = new Vue({
                el: '#vueEditOrderBanner',
                data: {
                    Cart: currentCart
               },
                computed:  {
                    IsDraftCart() { return this.Cart.CartType == "OrderDraft" },
                    OrderNumberForOrderDraft() { return this.Cart.OrderSummary.OrderNumberForOrderDraft; }
                },
                methods: {
                    cancelEditOrder() {
                        self.orderService.cancelEditOrder(this.OrderNumberForOrderDraft)
                        .then(() => {
                            return self.cartService.getFreshCart(true);
                        })
                            .then(cart => {
                                this.Cart = cart;
                                self.eventHub.publish(CartEvents.CartUpdated, { data: cart });
                            })
                        .fail(reason => console.log(reason));
                    }
                }
            });
        }
    }
}