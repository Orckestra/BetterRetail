///<reference path='../../../../Composer.UI/Source/Typings/tsd.d.ts' />
///<reference path='../../../../Composer.UI/Source/TypeScript/Mvc/Controller.ts' />
///<reference path='../../../../Composer.UI/Source/TypeScript/Mvc/IControllerActionContext.ts' />
///<reference path='../../../MyAccount/Source/Typescript/MyAccountController.ts' />
///<reference path='../../../../Composer.Cart.UI/MonerisPaymentProvider/Source/TypeScript/MonerisPaymentService.ts' />

module Orckestra.Composer {
    export class WalletController extends Orckestra.Composer.MyAccountController {

        protected monerisPaymentService: MonerisPaymentService = new MonerisPaymentService();

        /**
        * Requires the element in action context to have a data-creditcard-id and data-paymentmethod-provider.
        */
        public setDefaultCreditCard(actionContext: IControllerActionContext): void {

            var $paymentMethodId: JQuery = $(actionContext.elementContext).closest('[data-paymentmethod-id]');
            var paymentMethodId = $paymentMethodId.data('paymentmethod-id');

            var $paymentMethodProviderName: JQuery = $(actionContext.elementContext).closest('[data-paymentmethod-provider]');
            var paymentMethodProviderName = $paymentMethodProviderName.data('paymentmethod-provider');

            this.monerisPaymentService.setDefaultCustomerPaymentMethod({
                PaymentProviderName: paymentMethodProviderName,
                PaymentMethodId: paymentMethodId
            })
                .then(result => location.reload(), reason => console.error(reason))
                .done();
        }

        /**
        * Requires the element in action context to have a data-creditcard-id and data-paymentmethod-provider.
        */
        public removeCreditCard(actionContext: IControllerActionContext): void {
            var $paymentMethodId: JQuery = $(actionContext.elementContext).closest('[data-paymentmethod-id]');
            var paymentMethodId = $paymentMethodId.data('paymentmethod-id');

            var $paymentMethodProviderName: JQuery = $(actionContext.elementContext).closest('[data-paymentmethod-provider]');
            var paymentMethodProviderName = $paymentMethodProviderName.data('paymentmethod-provider');

            this.monerisPaymentService.removePaymentMethod(paymentMethodId, paymentMethodProviderName)
                .then(result => location.reload(), reason => console.error(reason))
                .done();
        }
    }
}