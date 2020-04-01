///<reference path='../../Typings/tsd.d.ts' />

module Orckestra.Composer {

    export interface ISingleCheckoutService {

        VueCheckout: any;
        VueCheckoutMixins: any;

        registerController(controller: IController);

        unregisterController(controllerName: string);

        getCart(): Q.Promise<any>;

        removeCartItem(id, productId): Q.Promise<any>;

        updateCartItem(id: string, quantity: number, productId: string,
            recurringOrderFrequencyName?: string,
            recurringOrderProgramName?: string): Q.Promise<any>;

        updateCart(controllerNames?: Array<string>): Q.Promise<IUpdateCartResult>;

        updatePaymentMethod(param: any): Q.Promise<IActivePaymentViewModel>;

        completeCheckout(): Q.Promise<ICompleteCheckoutResult>;

        updatePostalCode(postalCode: string): Q.Promise<void>;

        invalidateCache(): Q.Promise<void>;

        getPaymentProviders(paymentProviders: Array<any>) : Array<BaseCheckoutPaymentProvider>;

        getPaymentCheckout(): Q.Promise<ICheckoutPaymentViewModel>;

        updateBillingPostalCode(postalCode: string): Q.Promise<void>;

        ///setOrderConfirmationToCache(orderConfirmationviewModel : any) : void;

        //(): Q.Promise<any>;

        //clearOrderConfirmationFromCache(): void;

        //setOrderToCache(orderConfirmationviewModel : any) : void;
    }
}
