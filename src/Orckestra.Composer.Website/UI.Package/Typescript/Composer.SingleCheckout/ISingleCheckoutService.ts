///<reference path='../../Typings/tsd.d.ts' />

module Orckestra.Composer {

    export interface ISingleCheckoutService {

        VueCheckout: any;
        VueCheckoutMixins: any;

        registerController(controller: IController);

        unregisterController(controllerName: string);

        getCart(): Q.Promise<any>;

        updateCart(): Q.Promise<IUpdateCartResult>;

        completeCheckout(): Q.Promise<ICompleteCheckoutResult>;

        updatePostalCode(postalCode: string): Q.Promise<void>;

        invalidateCache(): Q.Promise<void>;

        ///setOrderConfirmationToCache(orderConfirmationviewModel : any) : void;

        //(): Q.Promise<any>;

        //clearOrderConfirmationFromCache(): void;

        //setOrderToCache(orderConfirmationviewModel : any) : void;
    }
}
