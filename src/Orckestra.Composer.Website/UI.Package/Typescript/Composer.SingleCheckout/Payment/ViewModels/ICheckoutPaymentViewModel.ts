///<reference path='./IPaymentMethodViewModel.ts' />
///<reference path='./IActivePaymentViewModel.ts' />

namespace Orckestra.Composer {
    'use strict';

    export interface ICheckoutPaymentViewModel {
        PaymentMethods: Array<IPaymentMethodViewModel>;
        PaymentProviders: Array<{
            ProviderName: string;
            ProviderType: string;
        }>;
        ActivePaymentViewModel: IActivePaymentViewModel;
        CreditCardTrustImage: {
            Url: string;
            Alt: string;
        };
    }
}
