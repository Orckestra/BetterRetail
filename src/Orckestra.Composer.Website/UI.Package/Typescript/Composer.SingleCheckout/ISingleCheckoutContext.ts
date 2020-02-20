///<reference path='../../Typings/tsd.d.ts' />

module Orckestra.Composer {
    export interface ISingleCheckoutContext {
        IsAuthenticated: boolean;
        Cart: any;
        Regions: any;
        ShippingMethods: any;
    }
}
