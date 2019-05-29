///<reference path='../../../../Composer.UI/Source/Typings/tsd.d.ts' />

module Orckestra.Composer {

    export interface IWalletRepository {
        /**
        * Set default credit card for a customer
        * @param
        */
        setDefaultCreditCard(paymentMethodId: string, paymentProviderName: string, returnUrl: string): Q.Promise<any>;
    }
}