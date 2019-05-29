///<reference path='../../../../Composer.UI/Source/Typings/tsd.d.ts' />

module Orckestra.Composer {
    export interface IWalletService {

        /**
        * Set default credit card for a customer
        * @param
        */
       setDefaultCreditCard(paymentMethodId: string, paymentProviderName: string, returnUrl: string): Q.Promise<any>;
    }
}