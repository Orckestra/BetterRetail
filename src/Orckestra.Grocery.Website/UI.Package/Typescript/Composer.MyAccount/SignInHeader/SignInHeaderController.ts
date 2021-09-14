///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Mvc/Controller.ts' />
///<reference path='../../Events/EventScheduler.ts' />
///<reference path='../Common/MyAccountEvents.ts' />
///<reference path='../../Services/UserMetadataService.ts' />

module Orckestra.Composer {
    'use strict';

    export class SignInHeaderController extends Orckestra.Composer.Controller {

        protected userMetadataService: UserMetadataService = new UserMetadataService(new MembershipRepository());
        protected membershipService: IMembershipService = new MembershipService(new MembershipRepository());
        public VueSignInHeader: Vue;
        public vueSignInHeaderMobile: Vue;

        public initialize() {

            super.initialize();

            this.initializeSignInHeader();
            this.registerSubscriptions();
        }

        private initializeSignInHeader() {
            var cultureInfo = $('html').attr('lang');
            var websiteId = $('html').data('website');
            var param = { cultureInfo, websiteId };
            let self: SignInHeaderController = this;

            this.userMetadataService.getUserMetadata(param)
                .then(vm => {
                    this.VueSignInHeader = new Vue({
                        el: '#vueSignInHeader',
                        data: vm,
                        methods: {
                            fullLogout() {
                                self.fullLogout();
                            }
                        }
                    });
                    this.vueSignInHeaderMobile = new Vue({
                        el: '#vueSignInHeaderMobile',
                        data: vm,
                        methods: {
                            fullLogout() {
                                self.fullLogout();
                            }
                        }
                    });
                });
        }

        protected registerSubscriptions() {
            var loggedInScheduler = EventScheduler.instance(MyAccountEvents[MyAccountEvents.LoggedIn]);
            var loggedOutScheduler = EventScheduler.instance(MyAccountEvents[MyAccountEvents.LoggedOut]);

            loggedInScheduler.subscribe(e => this.onLoggedIn(e));
            loggedOutScheduler.setPostEventCallback((data: any) => this.onLoggedOut(data));
        }

        protected onLoggedOut(data: any): Q.Promise<any> {
            var promise: Q.Promise<any> = Q.fcall(() => {
                var newLocation = decodeURIComponent(data.ReturnUrl) || window.location.href;
                window.location.replace(newLocation);
            });

            return promise;
        }

        protected onLoggedIn(e: IEventInformation): Q.Promise<any> {
            return this.userMetadataService.invalidateCache();
        }

        public fullLogout() {
            this.userMetadataService.invalidateCache();
            var returnUrlQueryString: string = 'ReturnUrl=';
            var returnUrl: string = '';

            if (window.location.href.indexOf(returnUrlQueryString) > -1) {
                returnUrl = window.location.href.substring(window.location.href.indexOf(returnUrlQueryString)
                    + returnUrlQueryString.length);
            }

            this.membershipService.logout(returnUrl, false)
                .then(result => this.eventHub.publish(MyAccountEvents[MyAccountEvents.LoggedOut], { data: result }))
                .done();
        }
    }
}
