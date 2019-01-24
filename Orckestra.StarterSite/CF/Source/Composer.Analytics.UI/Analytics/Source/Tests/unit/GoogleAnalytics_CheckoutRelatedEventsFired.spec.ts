///<reference path='../../../../../Composer.UI/Source/Typings/tsd.d.ts' />
///<reference path='../../../../../Composer.UI/Source/Tests/unit/Mocks/MockControllerContext.ts' />
///<reference path='../../Typescript/GoogleAnalyticsPlugin.ts' />
///<reference path='../../Typescript/AnalyticsPlugin.ts' />

module Orckestra.Composer {
    (() => {

        'use strict';

        function CreateOrderWithCouponResultSample() {
            let order = {
                BillingCurrency: 'CAD',
                    Coupons: [{
                        CouponCode: '30DOFF',
                        BillingCurrency: 'CAD',
                        Amount: '50.56',
                        PromotionName: 'Summer Sale'
                    }]
                };

            return order;
        }

        function CreateOrderResultSample() {
            let order = {
                    OrderNumber: '123456',
                    Affiliation: 'Montreal store',
                    Revenu: '123.45',
                    Tax: '12.12',
                    Shipping: '2.56',
                    ShippingOptions: 'Pickup',
                    BillingCurrency: 'CAD'
            };

            return order;
        }

        function CreateExpectedAnalyticsCoupon(data: any) {

            let coupons: IAnalyticsCoupon[] = [];

            _.each(data.Coupons, (coupon: any) => {
                let analyticsCoupon: IAnalyticsCoupon = {
                    code: coupon.CouponCode,
                    discountAmount: coupon.Amount,
                    currencyCode: coupon.BillingCurrency,
                    promotionName: coupon.PromotionName
                };

                coupons.push(analyticsCoupon);
            });

            return coupons;
        }

        function CreateExpectedAnalyticsOrder(data: any) {
            let analyticsOrder : IAnalyticsOrder = {
                id: data.OrderNumber,
                affiliation: data.Affiliation,
                revenue: data.Revenu,
                tax: data.Tax,
                shipping: data.Shipping,
                coupon: '',
                currencyCode: data.BillingCurrency
                };

            return analyticsOrder;
        }

        function CreateExpectedAnalyticsTransaction(data: any, checkoutOrigin: string) {
            let analyticsTransaction : IAnalyticsTransaction = {
                checkoutOrigin: checkoutOrigin,
                shippingType: data.ShippingOptions
            };

            return analyticsTransaction;
        }

        describe('WHEN order is created', () => {

            let checkoutOrigin = 'checkoutOrigin';
            Orckestra.Composer.AnalyticsPlugin.setCheckoutOrigin(checkoutOrigin);

            let analytics : GoogleAnalyticsPlugin;
            let eventHub: IEventHub;

            beforeEach (() => {
                analytics = new GoogleAnalyticsPlugin();
                analytics.initialize();
                eventHub = EventHub.instance();
            });

            describe('WITH coupon code used', () => {

                let order = CreateOrderWithCouponResultSample();

                let expectedAnalyticsCoupon = CreateExpectedAnalyticsCoupon(order);

                it('SHOULD fire the purchase trigger', () => {
                    // arrange
                    spyOn(analytics, 'couponsUsed');

                    // act -- publish
                    eventHub.publish('CheckoutConfirmation', {data: order});

                    // assert -- does it match
                    _.each(expectedAnalyticsCoupon, c => {
                        expect(analytics.couponsUsed).toHaveBeenCalledWith(c);
                    });
                });
            });

            describe('WITH pickup shipping option selected', () => {

                let order = CreateOrderResultSample();

                let expectedAnalyticsOrder = CreateExpectedAnalyticsOrder(order);
                let expectedAnalyticsTransaction = CreateExpectedAnalyticsTransaction(order, checkoutOrigin);

                it('SHOULD fire the purchase trigger', () => {
                    // arrange
                    spyOn(analytics, 'purchase');

                    // act -- publish
                    eventHub.publish('CheckoutConfirmation', {data: order});

                    // assert -- does it match
                    const emptyProducts : IAnalyticsProduct[] = [];
                    expect(analytics.purchase).toHaveBeenCalledWith(expectedAnalyticsOrder, expectedAnalyticsTransaction, emptyProducts);
                });
            });

            describe('WITH checkout origin specified', () => {

                let order = CreateOrderResultSample();
                let checkoutOrigin = 'checkoutOrigin';

                let expectedAnalyticsOrder = CreateExpectedAnalyticsOrder(order);
                let expectedAnalyticsTransaction = CreateExpectedAnalyticsTransaction(order, checkoutOrigin);

                it('SHOULD fire the purchase trigger', () => {
                    // arrange
                    spyOn(analytics, 'purchase');

                    // act -- publish
                    eventHub.publish('CheckoutConfirmation', {data: order});

                    // assert -- does it match
                    const emptyProducts : IAnalyticsProduct[] = [];
                    expect(analytics.purchase).toHaveBeenCalledWith(expectedAnalyticsOrder, expectedAnalyticsTransaction, emptyProducts);
                });
            });
        });
    })();
}