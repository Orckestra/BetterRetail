///<reference path='../../../../Composer.UI/Source/Typings/tsd.d.ts' />
///<reference path='../../../../Composer.UI/Source/Typescript/Repositories/WalletRepository.ts' />
///<reference path='IWalletService.ts' />

module Orckestra.Composer {
    'use strict';

    export class WalletService  implements IWalletService {

        protected walletRepository: IWalletRepository;

        public constructor(walletRepository: IWalletRepository) {

            if (!walletRepository) {
                throw new Error('Error: walletRepository is required');
            }

            this.walletRepository = walletRepository;
        }

        /**
        * Set default credit card for a customer
        * @param
        */
        public setDefaultCreditCard(paymentMethodId: string, paymentProviderName: string, returnUrl: string): Q.Promise<any> {

            return this.walletRepository.setDefaultCreditCard(paymentMethodId, paymentProviderName, returnUrl);
        }
    }
}
