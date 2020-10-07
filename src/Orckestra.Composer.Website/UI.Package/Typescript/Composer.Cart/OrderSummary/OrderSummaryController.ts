///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Mvc/Controller.ts' />
///<reference path='../../Mvc/IControllerActionContext.ts' />
///<reference path='../../Repositories/CartRepository.ts' />
///<reference path='../../JQueryPlugins/ISerializeObjectJqueryPlugin.ts' />
///<reference path='../../ErrorHandling/ErrorHandler.ts' />
///<reference path='../../Composer.Analytics/Analytics/GoogleAnalyticsPlugin.ts' />
///<reference path='../CartSummary/CartService.ts' />
///<reference path='../CartSummary/CartStateService.ts' />
///<reference path='./OrderSummaryService.ts' />

module Orckestra.Composer {

    export class OrderSummaryController extends Orckestra.Composer.Controller {

        private cacheProvider: ICacheProvider = CacheProvider.instance();
        private cartService: ICartService = CartService.getInstance();
        private cartStateService: ICartStateService = CartStateService.getInstance();
        private orderSummaryService: OrderSummaryService = new OrderSummaryService(this.cartService, this.eventHub);
        private postalCodeModal : any;
        private postalCodeInput : any;

        public initialize() {
            super.initialize();
            let self: OrderSummaryController = this;

            let cartOrderSummaryMixins = {
                methods: {
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


        //TODO:
        public openModal(actionContext: IControllerActionContext) {

            this.postalCodeModal = $('#postalCodeModal');
            this.postalCodeInput = $('#postalCode');
            this.clearForm();

            //Due to how HTML5 defines its semantics, the autofocus HTML attribute has no effect in Bootstrap modals
            //http://getbootstrap.com/javascript/#modals
            this.postalCodeModal.on('shown.bs.modal', () => {
                this.postalCodeInput.focus();
            });

            this.postalCodeModal.modal('show');
        }

        private clearForm() {

            this.postalCodeInput.val('');
            this.render('EstimateShippingValidationForm', { PostalCodeMalformed: false, PostalCodeEmpty: false });
            return;
        }

        private closeModal(actionContext: IControllerActionContext) {

            this.postalCodeModal.modal('hide');
            this.postalCodeModal.off('shown.bs.modal');
        }

        public estimateShipping(actionContext: IControllerActionContext) {

            var formContext = actionContext.elementContext,
                formValues = (<ISerializeObjectJqueryPlugin>formContext).serializeObject(),
                postalCode = formValues.postalCode.toUpperCase(),
                postalCodePattern = formContext.data('regex'),
                postalCodeRegexPattern = new RegExp(postalCodePattern.toString()),
                result = postalCodeRegexPattern.test(postalCode),
                busyHandle: UIBusyHandle;

            actionContext.event.preventDefault();

            if (!result) {
                if (postalCode === '') {
                    return this.render('EstimateShippingValidationForm', { PostalCodeEmpty: true });
                } else {
                    return this.render('EstimateShippingValidationForm', { PostalCodeMalformed: true, PostalCode: formValues.postalCode });
                }
            }

            busyHandle = this.asyncBusy();

            this.orderSummaryService.setCheapestShippingMethodUsing(postalCode)
                .then((data: any) => {
                    this.closeModal(actionContext);
                    return data;
                }, (reason: any) => {
                    ErrorHandler.instance().outputErrorFromCode('PostalCodeUpdateFailed');
                })
                .fin(() => busyHandle.done());
        }
    }
}
