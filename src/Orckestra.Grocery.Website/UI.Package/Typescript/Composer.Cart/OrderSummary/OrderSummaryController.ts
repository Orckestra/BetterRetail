///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Mvc/Controller.ts' />
///<reference path='../../Repositories/CartRepository.ts' />
///<reference path='../../ErrorHandling/ErrorHandler.ts' />
///<reference path='../../Composer.Analytics/Analytics/GoogleAnalyticsPlugin.ts' />
///<reference path='../CartSummary/CartService.ts' />
///<reference path='../CartSummary/CartStateService.ts' />
///<reference path='./OrderSummaryService.ts' />
///<reference path='../OrderHistory/Services/OrderService.ts' />

module Orckestra.Composer {

    export class OrderSummaryController extends Orckestra.Composer.Controller {

        private cartService: ICartService = CartService.getInstance();
        private cartStateService: ICartStateService = CartStateService.getInstance();
        protected orderService = new OrderService();
        private orderSummaryService: OrderSummaryService = new OrderSummaryService(this.cartService, this.eventHub);

        public initialize() {
            super.initialize();
            let self: OrderSummaryController = this;

            let cartOrderSummaryMixins = {
                mounted() {
                },
                data: {
                    EstimateShippingPostalCode: undefined,
                    PostalCodeEmpty: false,
                    PostalCodeMalformed: false
                },
                computed: {
                    CheckoutButtonDisabled() {
                        return !this.IsStoreSelected;
                    }
                },
                methods: {
                    openEstimateShippingModal() {
                        this.postalCodeModal = $('#postalCodeModal');
                        this.postalCodeModal.modal('show');
                    },
                    closeModal() {
                        this.postalCodeModal.modal('hide');
                        this.postalCodeModal.off('shown.bs.modal');
                    },
                    estimateShipping(postalCodePattern) {

                        if (!this.EstimateShippingPostalCode) {
                            this.PostalCodeEmpty = true;
                            return;
                        }

                        let postalCode = this.EstimateShippingPostalCode.toUpperCase();

                        if (postalCodePattern) {
                            let postalCodeRegexPattern = new RegExp(postalCodePattern.toString());
                            this.PostalCodeMalformed = postalCodeRegexPattern.test(postalCode);
                            if (!this.PostalCodeMalformed) {
                                return;
                            }
                        }

                        this.Mode.Loading = true;
                        self.orderSummaryService.setCheapestShippingMethodUsing(postalCode)
                            .then((data: any) => {
                                this.closeModal();
                                return data;
                            }, (reason: any) => {
                                ErrorHandler.instance().outputErrorFromCode('PostalCodeUpdateFailed');
                            })
                            .fin(() => this.Mode.Loading = false);

                    },
                    cancelEditOrder() {
                        this.Mode.Loading = true;
                        self.orderService.cancelEditOrder(this.Cart.OrderSummary.OrderNumberForOrderDraft)
                            .fin(() => this.Mode.Loading = false);
                    },
                    saveEditOrder() {
                        this.Mode.Loading = true;
                        self.orderService.saveEditOrder(this.Cart.OrderSummary.OrderNumberForOrderDraft)
                            .fin(() => this.Mode.Loading = false);
                    },
                    proceedToCheckout() {
                        let nextStepUrl = this.OrderSummary.CheckoutUrlTarget;
                        if (!nextStepUrl) {
                            throw 'No next step Url was defined.';
                        }

                        AnalyticsPlugin.setCheckoutOrigin('Checkout');

                        this.Mode.Loading = true;
                        self.orderSummaryService.cleanCart().done(() => {
                            window.location.href = nextStepUrl;
                        }, reason => {
                            console.error('Error while proceeding to Checkout', reason);
                            ErrorHandler.instance().outputErrorFromCode('ProceedToCheckoutFailed');
                        });
                    }
                }
            };

            this.cartStateService.VueCartMixins.push(cartOrderSummaryMixins);
        }
    }
}
