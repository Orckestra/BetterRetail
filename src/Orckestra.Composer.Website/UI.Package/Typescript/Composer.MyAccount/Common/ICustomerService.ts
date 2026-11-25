///<reference path='../../../Typings/tsd.d.ts' />

namespace Orckestra.Composer {
    export interface ICustomerService {

        /**
        * Attempt to register using Composer API.
        * @param
        */
        updateAccount(formData: any, returnUrl: string): Promise<any>;

        /**
         * Get the customer addresses.
         */
        getAddresses(): Promise<any>;

        /**
         * Get the customer addresses for a recurring cart page.
         */
        getRecurringCartAddresses(cartName: string): Promise<any>;

        /**
         * Get the customer addresses for a recurring template page.
         */
        getRecurringTemplateAddresses(id: string): Promise<any>;

        /**
        * Create a new customer address
        * @param
        */
        createAddress(formData: any, returnUrl: string): Promise<any>;

        /**
        * Update a customer address
        * @param
        */
        updateAddress(formData: any, addressId: string, returnUrl: string): Promise<any>;

        /**
        * Delete a customer address
        * @param
        */
        deleteAddress(addressId: JQuery, returnUrl: string): Promise<any>;

        /**
        * Set default address for a customer
        * @param
        */
        setDefaultAddress(addressId: string, returnUrl: string): Promise<any>;
    }
}