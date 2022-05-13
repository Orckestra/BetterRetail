///<reference path='../../Typings/tsd.d.ts' />
///<reference path='./Payment/ViewModels/ICheckoutPaymentViewModel.ts' />

module Orckestra.Composer {
    export interface ISingleCheckoutContext {
        IsAuthenticated: boolean;
        Cart: any;
        Regions: any;
        ShippingMethodTypes: any;
        Payment: ICheckoutPaymentViewModel;
    }
}
