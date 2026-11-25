///<reference path='../../../Typings/tsd.d.ts' />

namespace Orckestra.Composer {

    export interface ISingleCheckoutService {

        VueCheckout: any;
        VueCheckoutMixins: any;

        registerController(controller: IController);

        unregisterController(controllerName: string);

        getCart(): Promise<any>;

        calculateStartStep(cart: any, isAuthenticated: boolean): number;

        customerFulfilled(cart: any): boolean;

        shippingFulfilled(cart: any, isAuthenticated: boolean): boolean;

        billingFulfilled(cart: any, isAuthenticated: boolean): boolean;

        removeCartItem(id, productId): Promise<any>;

        updateCartItem(id: string, quantity: number, productId: string,
            recurringOrderFrequencyName?: string,
            recurringOrderProgramName?: string): Promise<any>;

        updateCart(controllerNames?: Array<string>): Promise<IUpdateCartResult>;

        updatePaymentMethod(param: any): Promise<IActivePaymentViewModel>;

        completeCheckout(): Promise<ICompleteCheckoutResult>;

        updatePostalCode(postalCode: string): Promise<void>;

        invalidateCache(): Promise<void>;

        getPaymentProviders(paymentProviders: Array<any>) : Array<BaseCheckoutPaymentProvider>;

        getPaymentCheckout(): Promise<ICheckoutPaymentViewModel>;

        updateBillingPostalCode(postalCode: string): Promise<void>;

        collectViewModelNamesForUpdateCart():  Promise<any>;

        setOrderConfirmationToCache(orderConfirmationviewModel : any) : void;

        clearOrderConfirmationFromCache(): void;

        setOrderToCache(orderConfirmationviewModel : any) : void;

        saveAddressToMyAccountAddressBook(address: any): Promise<any>;

        updateAddressInMyAccountAddressBook(address: any): Promise<any>;

        deleteAddress(addressId: any): Promise<any>;

        loginUser(formData: any): Promise<any>;

        checkUserExist(username: string): Promise<boolean>;

        loadUserAddresses(): Promise<any>;

        isAddressBookIdEmpty(id): boolean;
    }
}
