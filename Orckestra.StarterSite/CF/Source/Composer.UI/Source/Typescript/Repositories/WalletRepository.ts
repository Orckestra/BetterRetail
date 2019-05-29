///<reference path='../../../../Composer.UI/Source/Typings/tsd.d.ts' />
///<reference path='../../../../Composer.UI/Source/Typescript/Mvc/ComposerClient.ts' />
///<reference path='./IWalletRepository.ts' />

module Orckestra.Composer {
    'use strict';

    export class WalletRepository implements IWalletRepository {

        /**
        * Set default credit card for a customer
        * @param
        */
        public setDefaultCreditCard(paymentMethodId: string, paymentProviderName: string, returnUrl: string): Q.Promise<any> {

            var data = {
                PaymentMethodId: paymentMethodId,
                PaymentProviderName: paymentProviderName,
                ReturnUrl: returnUrl
            };

            return ComposerClient.post('/api/wallet/setdefaultcreditcard/', data);
        }
    }
}
