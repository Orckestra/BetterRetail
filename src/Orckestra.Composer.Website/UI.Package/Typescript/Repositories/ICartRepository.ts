/// <reference path='../../Typings/tsd.d.ts' />

namespace Orckestra.Composer {

    /**
     * Separates the logic that retrieves the data and maps it to the entity model from the application services that acts on the model.
    */
    export interface ICartRepository {

        /**
         * Get the cart of the current customer.
         */
        getCart(): Promise<any>;

        /**
         * Add a line item to the cart of the current customer.
         */
        addLineItem(productId: string, variantId?: string, quantity?: number,
            recurringOrderFrequencyName?: string,
            recurringOrderProgramName?: string): Promise<any>;

        /**
         * Update the quantity of a line item in the cart of the current customer.
         */
        updateLineItem(lineItemId: string, quantity: number,
            recurringOrderFrequencyName?: string,
            recurringOrderProgramName?: string): Promise<any>;

        /**
         * Delete a line item from the cart of the current customer.
         */
        deleteLineItem(lineItemId: string): Promise<any>;

        /**
         * Update the postal code of the billing method in cart of the current customer.
         */
        updateBillingMethodPostalCode(postalCode: string): Promise<any>;

        /**
         * Update the postal code of the shipping method in cart of the current customer.
         */
        updateShippingMethodPostalCode(postalCode: string): Promise<any>;

        /**
         * Set the cheapest shipping method in cart of the current customer.
         */
        setCheapestShippingMethod(): Promise<any>;

        /**
         * Add a coupon to the cart of the current customer.
         */
        addCoupon(couponCode: string): Promise<any>;

        /**
         * Remove a coupon from the cart of the current customer.
         */
        removeCoupon(couponCode: string): Promise<any>;

        /**
         * Cleans the cart of invalid line items.
         */
        clean(): Promise<any>;

        /**
         * Update the cart of the current customer.
         */
        updateCart(param: any): Promise<IUpdateCartResult>;

        /**
         * Complete the checkout, thereby clearing every item in the cart of the current customer.
         */
        completeCheckout(currentStep: number): Promise<ICompleteCheckoutResult>;
    }

    export interface IUpdateCartResult {

        HasErrors: boolean;

        NextStepUrl: string;

        Cart: any;
    }

    export interface ICompleteCheckoutResult {

        OrderNumber: string;

        CustomerEmail: string;

        CustomerFirstName: string;

        CustomerLastName: string;

        NextStepUrl: string;

        IsUpdatedOrder?: boolean;
    }
}
