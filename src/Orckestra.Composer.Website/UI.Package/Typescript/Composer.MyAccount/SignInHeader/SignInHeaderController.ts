///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Mvc/Controller.ts' />
///<reference path='../../Events/EventScheduler.ts' />
///<reference path='../Common/MyAccountEvents.ts' />
///<reference path='../../Services/UserMetadataService.ts' />

module Orckestra.Composer {
    'use strict';

    export class SignInHeaderController extends Orckestra.Composer.Controller {

        protected userMetadataService: UserMetadataService = UserMetadataService.getInstance();
        public VueSignInHeader: Vue;
        public VueSignInSticky: Vue;

        public initialize() {

            super.initialize();

            this.initializeSignInHeader();
            this.registerSubscriptions();
        }

        private initializeSignInHeader() {
            let self: SignInHeaderController = this;

            this.userMetadataService.getUserMetadata()
                .then(vm => {
                    this.VueSignInHeader = new Vue({
                        el: '#vueSignInHeader',
                        data: vm
                    });
                    this.VueSignInSticky = new Vue({
                        el: '#vueSignInSticky',
                        data: vm
                    });
                });
        }

        protected registerSubscriptions() {
            var loggedInScheduler = EventScheduler.instance(MyAccountEvents[MyAccountEvents.LoggedIn]);
            var loggedOutScheduler = EventScheduler.instance(MyAccountEvents[MyAccountEvents.LoggedOut]);

            loggedOutScheduler.subscribe(e => this.onLoggedOut(e));
            loggedInScheduler.subscribe(e => this.onLoggedIn(e));
        }

        protected onLoggedOut(e: IEventInformation): Q.Promise<any> {
            return this.userMetadataService.invalidateCache();
        }

        protected onLoggedIn(e: IEventInformation): Q.Promise<any> {
            return this.userMetadataService.invalidateCache();
        }
    }
}

