///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Mvc/Controller.ts' />
///<reference path='../CartSummary/CartService.ts' />

module Orckestra.Composer {
    export class EditOrderBannerController extends Controller {
        protected cartService: ICartService = CartService.getInstance();
        protected VueTimeSlotBanner: Vue;
        public initialize() {
            super.initialize();
            this.cartService.getCart()
                .then((currentCart) => this.initializeVueComponent(currentCart));
        }
        private initializeVueComponent(currentCart) {
            let self: EditOrderBannerController = this;
            this.VueTimeSlotBanner = new Vue({
                el: '#vueEditOrderBanner',
                data: {
                    IsDraftCart: currentCart.CartType == "OrderDraft",
                    OrderNumberForOrderDraft: currentCart.OrderSummary.OrderNumberForOrderDraft
                }
            });
        }
    }
}