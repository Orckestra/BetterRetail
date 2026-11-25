///<reference path='./ICreateVaultTokenOptions.ts' />
///<reference path='./ISetDefaultCustomerPaymentMethodViewModel.ts' />
///<reference path='./IMonerisAddVaultProfileViewModel.ts' />
///<reference path='../ViewModels/IPaymentMethodViewModel.ts' />
///<reference path='../../../../Typings/tsd.d.ts' />
///<reference path='../../../Mvc/ComposerClient.ts' />

namespace Orckestra.Composer {
    export class MonerisPaymentService {

        /**
         * Adds a token from Moneris to the current payment.
         * @param  {ICreateVaultTokenOptions} request Request to add the Vault Token.
         * @return {Promise<any>}                   Promise of the AJAX request.
         */
        public addCreditCard(request: ICreateVaultTokenOptions): Promise<IMonerisAddVaultProfileViewModel> {
            return ComposerClient.post('/api/vaultprofile/addprofile', request);
        }

        public setDefaultCustomerPaymentMethod(request: ISetDefaultCustomerPaymentMethodViewModel): Promise<IPaymentMethodViewModel> {
            return ComposerClient.put('/api/cart/setdefaultpaymentmethod', request);
        }

        public removePaymentMethod(paymentMethodId: string, paymentProviderName: string): Promise<void> {
            return <Promise<void>>
                ComposerClient.remove('/api/payment/removemethod',
                                        {
                                            PaymentMethodId: paymentMethodId,
                                            PaymentProviderName: paymentProviderName
                                        });
        }

        public removeRecurringCartPaymentMethod(paymentMethodId: string, paymentProviderName: string, cartName: string): Promise<void> {
            return <Promise<void>>
                ComposerClient.remove('/api/payment/recurringcartremovemethod',
                                        {
                                            PaymentMethodId: paymentMethodId,
                                            PaymentProviderName: paymentProviderName,
                                            CartName: cartName
                                        });
        }
    }
}
