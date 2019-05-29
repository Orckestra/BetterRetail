///<reference path='../../../../Composer.UI/Source/Typings/tsd.d.ts' />
///<reference path='../../../../Composer.UI/Source/TypeScript/Mvc/Controller.ts' />
///<reference path='../../../../Composer.UI/Source/TypeScript/Mvc/IControllerActionContext.ts' />
///<reference path='../../../MyAccount/Source/Typescript/MyAccountController.ts' />
///<reference path='../../../../Composer.Cart.UI/MonerisPaymentProvider/Source/TypeScript/MonerisCanadaPaymentProvider.ts' />

module Orckestra.Composer {
    export class WalletController extends Orckestra.Composer.MyAccountController {

        protected monerisPaymentService: MonerisPaymentService = new MonerisPaymentService();

        /**
            * Requires the element in action context to have a data-creditcard-id.
            */
        public setDefaultCreditCard(actionContext: IControllerActionContext): void {

            var $paymentMethodId: JQuery = $(actionContext.elementContext).closest('[data-paymentmethod-id]');
            var paymentMethodId = $paymentMethodId.data('paymentmethod-id');

            var $paymentMethodProviderName: JQuery = $(actionContext.elementContext).closest('[data-paymentmethod-provider]');
            var paymentMethodProviderName = $paymentMethodProviderName.data('paymentmethod-provider');

            var busy = this.asyncBusy({ elementContext: actionContext.elementContext, containerContext: $paymentMethodId });

            this.monerisPaymentService.setDefaultCustomerPaymentMethod({
                PaymentProviderName: paymentMethodProviderName,
                PaymentMethodId: paymentMethodId
            })
            .then(result => location.reload(), reason => console.error(reason))
            .fin(() => busy.done())
            .done();
        }
    }
}