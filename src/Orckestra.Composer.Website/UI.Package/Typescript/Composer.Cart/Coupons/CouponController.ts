///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Mvc/Controller.ts' />
///<reference path='./CouponService.ts' />
///<reference path='../../Composer.Cart/CartSummary/CartEvents.ts' />
///<reference path='../../Composer.Cart/CartSummary/CartStateService.ts' />

module Orckestra.Composer {
    /**
     * Controller for the Coupons section.
     */
    export class CouponController extends Orckestra.Composer.Controller {

        private couponService: CouponService = new CouponService(CartService.getInstance(), this.eventHub);
        private cartStateService: ICartStateService = CartStateService.getInstance();

        public initialize() {

            super.initialize();
            let self: CouponController = this;

            let couponsMixins = {
                data: {
                    CouponCode: undefined,
                    Mode: {
                        ApplyingCoupon: false
                    },
                    ShowAlert: false
                },
                computed: {
                    Coupons() {
                        return this.Cart.Coupons;
                    },
                    HasCouponsErrorMessage() {
                        return _.some(this.Coupons.Messages, m => { return (<any>m).Level === 'danger'; })
                    }
                },
                methods: {
                    applyCoupon() {
                        if (!this.CouponCode) return;
                        this.Mode.ApplyingCoupon = true;

                        self.couponService.addCoupon(this.CouponCode)
                            .fin(() => {
                                if (!this.HasCouponsErrorMessage) this.CouponCode = undefined;
                                this.Mode.ApplyingCoupon = false;
                                this.ShowAlert = true;
                            });
                    },
                    removeCoupon(couponCode) {

                        if (!couponCode || 0 === couponCode.length) {
                            console.log('The coupon code may not be null');
                            return;
                        }

                        self.couponService.removeCoupon(couponCode.toString());
                    }
                }
            };

            this.cartStateService.VueCartMixins.push(couponsMixins);
        }
    }
}
