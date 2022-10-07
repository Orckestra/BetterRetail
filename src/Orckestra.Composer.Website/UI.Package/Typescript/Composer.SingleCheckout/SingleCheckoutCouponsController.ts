///<reference path='../..//Typings/tsd.d.ts' />
///<reference path='./BaseSingleCheckoutController.ts' />
///<reference path='../Composer.Cart/Coupons/CouponService.ts' />

module Orckestra.Composer {
    'use strict';

    export class SingleCheckoutCouponsController extends Orckestra.Composer.BaseSingleCheckoutController {
        private couponService: CouponService = new CouponService(CartService.getInstance(), this.eventHub);
        public initialize() {
            super.initialize();
            let self: SingleCheckoutCouponsController = this;
            let couponsMixins = {
                data: {
                    CouponCode: undefined,
                    Mode: {
                        ApplyingCoupon: false
                    }
                },
                mounted() {
                    self.eventHub.subscribe(CartEvents.CartUpdated, this.onCartUpdated);
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
                        this.Mode.Loading = true;

                        self.couponService.addCoupon(this.CouponCode)
                            .fin(() => {
                                if (!this.HasCouponsErrorMessage) this.CouponCode = undefined;
                                this.Mode.ApplyingCoupon = false;
                                this.Mode.Loading = false;
                            });
                    },
                    removeCoupon(couponCode) {

                        if (!couponCode || 0 === couponCode.length) {
                            console.log('The coupon code may not be null');
                            return;
                        }
                        this.Mode.Loading = true;
                        self.couponService.removeCoupon(couponCode.toString())
                            .fin(() => this.Mode.Loading = false);
                    },
                    onCartUpdated(cart) {
                        this.Cart = cart.data;
                    }
                }
            };
            this.checkoutService.VueCheckoutMixins.push(couponsMixins);
        }
    }
}
