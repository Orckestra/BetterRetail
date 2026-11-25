/// <reference path='../../../Typings/tsd.d.ts' />

namespace Orckestra.Composer {

    /**
     * Separates the logic that retrieves the data and maps it to the entity model from the application services that acts on the model.
    */
    export interface ICartService {

        /**
         * Get the cart of the current customer.
         */
        getCart(): Promise<any>;

         /**
         * Get the cart of the current customer.
         * This forces the repository to get a fresh cart from Composer,
         * because the cart will contain different property values for each checkout step.
         */
        getFreshCart(force?: boolean): Promise<any>;

        /**
         * Add a line item to the cart of the current customer.
         */
        addLineItem(product: any, variantId: string, price: string, quantity: number, concern?: string,
            recurringOrderFrequencyName?: string): Promise<any>;

        /**
         * Update the quantity of a line item in the cart of the current customer.
         */
        updateLineItem(lineItemId: string, quantity: number, productId: string,
            recurringOrderFrequencyName?: string,
            recurringOrderProgramName?: string): Promise<any>;

        /**
         * Delete a line item from the cart of the current customer.
         */
        deleteLineItem(lineItemId: string, productId: string): Promise<any>;

        /**
         * Update the postal code of the billing method in cart of the current customer.
         */
        updateBillingMethodPostalCode(postalCode: string): Promise<void>;

        /**
         * Update the postal code of the shipping method in cart of the current customer.
         */
        updateShippingMethodPostalCode(postalCode: string): Promise<void>;

        /**
         * Set the cheapest shipping method in cart of the current customer.
         */
        setCheapestShippingMethod(): Promise<void>;

        /**
         * Add a coupon to the cart of the current customer.
         */
        addCoupon(couponCode: string): Promise<void>;

        /**
         * Remove a coupon from the cart of the current customer.
         */
        removeCoupon(couponCode: string): Promise<void>;

        /**
         * Cleans the cart of invalid line items.
         */
        clean(): Promise<void>;

        /**
         * Update the cart of the current customer.
         */
        updateCart(param: any): Promise<IUpdateCartResult>;

        /**
         * Complete the checkout, thereby clearing every item in the cart of the current customer.
         */
        completeCheckout(currentStep?: number): Promise<ICompleteCheckoutResult>;

        /**
         * Clear the Cart cache
         */
        invalidateCache(): Promise<void>;
    }
}
