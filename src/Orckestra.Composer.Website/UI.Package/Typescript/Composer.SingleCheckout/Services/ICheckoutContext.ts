///<reference path='../../../Typings/tsd.d.ts' />

namespace Orckestra.Composer {
    export interface ICheckoutContext {
        authenticationViewModel: IAuthenticationViewModel;
        cartViewModel: any;
        regionsViewModel: any;
        shippingMethodsViewModel: any;
    }

    export interface IAuthenticationViewModel {
        IsAuthenticated: boolean;
    }

    export interface ICartViewModel {
        IsAuthenticated: boolean;
    }
}
