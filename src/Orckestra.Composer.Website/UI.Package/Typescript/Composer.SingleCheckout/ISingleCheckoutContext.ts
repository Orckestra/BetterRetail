///<reference path='../../Typings/tsd.d.ts' />
///<reference path='./Payment/ViewModels/ICheckoutPaymentViewModel.ts' />

namespace Orckestra.Composer {
    export interface ISingleCheckoutContext {
        IsAuthenticated: boolean;
        UseEmailAsUsername: boolean;
        Cart: any;
        Regions: any;
        ShippingMethodTypes: any;
        Payment: ICheckoutPaymentViewModel;
    }
}
