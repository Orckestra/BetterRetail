///<reference path='../../Typings/tsd.d.ts' />
///<reference path='../Mvc/ComposerClient.ts' />
///<reference path='../Events/EventHub.ts' />
///<reference path='./IMembershipRepository.ts' />

namespace Orckestra.Composer {
    'use strict';

    export class MembershipRepository implements IMembershipRepository {

        /**
        * Attempt to Log In using Composer API.
        * @param
        */
        public login(formData: any, returnUrl: string): Promise<any> {
            var data = _.extend({ ReturnUrl: returnUrl }, formData);

            return ComposerClient.post('/api/membership/login', data);
        }

        /**
        * Logout using Composer API.
        * @param
        */
        public logout(returnUrl: string = '', preserveCustomerInfo: boolean = false): Promise<any> {
            var data = {
                ReturnUrl: returnUrl,
                PreserveCustomerInfo: preserveCustomerInfo
            };

            return ComposerClient.post('/api/membership/logout', data);
        }

        /**
        * Attempt to register using Composer API.
        * @param
        */
        public register(formData: any, returnUrl: string): Promise<any> {
            var data = _.extend({ ReturnUrl: returnUrl }, formData);

            return ComposerClient.post('/api/membership/register', data);
        }

        /**
        * Attempt to trigger the forgot password email using Composer API.
        * @param
        */
        public forgotPassword(formData: any): Promise<any> {
            var data = _.extend({}, formData);

            return ComposerClient.post('/api/membership/forgotpassword', data);
        }

        /**
        * Attempt to reset password for the Customer identified by the ticket using Composer API.
        * @param
        */
        public resetPassword(formData: any, ticket: string, returnUrl: string): Promise<any> {
            var data = _.extend({ ReturnUrl: returnUrl, ticket: ticket }, formData);

            return ComposerClient.post('/api/membership/resetpassword', data);
        }

        /**
        * Attempt to change the password for the connected Customer using Composer API.
        * @param
        */
        public changePassword(formData: any, returnUrl: string): Promise<any> {
            var data = _.extend({ ReturnUrl: returnUrl }, formData);

            return ComposerClient.post('/api/membership/changepassword', data);
        }

        /**
         * If the current user is authenticated or not
         */
        public isAuthenticated(): Promise<any> {

            return ComposerClient.get('/api/membership/isAuthenticated');
        }


        /**
        * If user with this username exist
        */
        public isUserExist(email: string): Promise<any> {

            return ComposerClient.get('/api/membership/isExist?email=' + email);
        }

        /**
        * Get user meta data to dispay status/name andurls
        */
        public userMetadata(): Promise<any> {

            return ComposerClient.get('/api/membership/usermetadata');
        }
    }
}
