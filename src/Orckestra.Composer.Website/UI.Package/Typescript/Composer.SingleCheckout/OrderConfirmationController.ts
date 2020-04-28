///<reference path='../../Typings/tsd.d.ts' />
///<reference path='../../Typings/vue/index.d.ts' />
///<reference path='../Composer.Cart/FindMyOrder/IFindOrderService.ts' />

module Orckestra.Composer {
    'use strict';

    export class OrderConfirmationController extends Orckestra.Composer.Controller {

        private cacheProvider: ICacheProvider;
        private findOrderService: IFindOrderService;
        private orderConfirmationCacheKey = 'orderConfirmationCacheKey';
        private orderCacheKey = 'orderCacheKey';
        public VueCheckoutOrderConfirmation: Vue;

        public initialize() {
            var self: OrderConfirmationController = this;
            super.initialize();
            this.cacheProvider = CacheProvider.instance();
            this.findOrderService = new FindOrderService(this.eventHub);

            this.cacheProvider.defaultCache.get<any>(this.orderCacheKey)
                .then((result: ICompleteCheckoutResult) => {

                    this.eventHub.publish('CheckoutConfirmation', { data: result });
                    this.cacheProvider.defaultCache.clear(this.orderCacheKey).done();
                })
                .fail((reason: any) => {

                    console.error('Unable to retrieve order number from cache, attempt to redirect.');
                });

            this.cacheProvider.defaultCache.get<any>(this.orderConfirmationCacheKey)
                .then((result: ICompleteCheckoutResult) => {

                    if (result) {

                        this.VueCheckoutOrderConfirmation = new Vue({
                            el: '#vueCheckoutOrderConfirmation',
                            data: result,
                            methods: {
                                findMyOrder() {
                                    let findMyOrderRequest = {
                                        OrderNumber: this.OrderNumber,
                                        Email: this.CustomerEmail
                                    };
                                    self.findOrderAsync(findMyOrderRequest).then(result => {
                                        window.location.href = result.Url;
                                    });
                                }
                            }
                        });

                        this.eventHub.publish('checkoutStepRendered', {
                            data: { StepNumber: 'confirmation' }
                        });

                        this.cacheProvider.defaultCache.clear(this.orderConfirmationCacheKey).done();

                    } else {
                        console.error('Order was placed but it is not possible to retrieve order number from cache.');
                    }
                })
                .fail((reason: any) => {

                    console.error('Unable to retrieve order number from cache, attempt to redirect.');

                    var redirectUrl: string = this.context.container.data('redirecturl');

                    if (redirectUrl) {
                        window.location.href = redirectUrl;
                    } else {
                        console.error('Redirect url was not detected.');
                    }
                });
        }

        private findOrderAsync(request: IGetOrderDetailsUrlRequest): Q.Promise<IGuestOrderDetailsViewModel> {
            return this.findOrderService.getOrderDetailsUrl(request);
        }
    }
}
