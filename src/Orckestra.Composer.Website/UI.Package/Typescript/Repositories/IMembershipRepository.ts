///<reference path='../../Typings/tsd.d.ts' />

namespace Orckestra.Composer {

    export interface IMembershipRepository {

        /**
        * Attempt to Log In using Composer API.
        * @param
        */
        login(formData: any, returnUrl: string): Promise<any>;

        /**
        * Logout using Composer API.
        * @param
        */
        logout(returnUrl: string, preserveCustomerInfo: boolean): Promise<any>;

        /**
        * Attempt to register using Composer API.
        * @param
        */
        register(formData: any, returnUrl: string): Promise<any>;

        /**
        * Attempt to trigger the forgot password email using Composer API.
        * @param
        */
        forgotPassword(formData: any): Promise<any>;

        /**
        * Attempt to reset password for the Customer identified by the ticket using Composer API.
        * @param
        */
        resetPassword(formData: any, ticket: string, returnUrl: string): Promise<any>;

        /**
        * Attempt to change the password for the connected Customer using Composer API.
        * @param
        */
        changePassword(formData: any, returnUrl: string): Promise<any>;

        /**
         * If the current user is authenticated or not
         */
        isAuthenticated(): Promise<any>;

        /**
        * If user with this username exist
        */
        isUserExist(email: string): Promise<any>;

        /**
        * Get user meta data to dispay status/name andurls
        */
        userMetadata(): Promise<any>
    }
}
