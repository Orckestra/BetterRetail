///<reference path='../../../Typings/tsd.d.ts' />

module Orckestra.Composer {

    export interface ISingleCheckoutService {

        VueCheckout: any;
        VueCheckoutMixins: any;

        registerController(controller: IController);

        unregisterController(controllerName: string);

        getCart(): Q.Promise<any>;

        calculateStartStep(cart: any, isAuthenticated: boolean): number;

        customerFulfilled(cart: any): boolean;

        shippingFulfilled(cart: any, isAuthenticated: boolean): boolean;

        billingFulfilled(cart: any, isAuthenticated: boolean): boolean;

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

        collectViewModelNamesForUpdateCart():  Q.Promise<any>;

        setOrderConfirmationToCache(orderConfirmationviewModel : any) : void;

        clearOrderConfirmationFromCache(): void;

        setOrderToCache(orderConfirmationviewModel : any) : void;

        saveAddressToMyAccountAddressBook(address: any): Q.Promise<any>;

        deleteAddress(addressId: any): Q.Promise<any>;

        loginUser(formData: any): Q.Promise<any>;

        checkUserExist(username: string): Q.Promise<boolean>;

        loadUserAddresses(): Q.Promise<any>;
    }
}
