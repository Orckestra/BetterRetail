///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Mvc/Controller.ts' />
///<reference path='../../Events/EventScheduler.ts' />
///<reference path='../Common/MyAccountEvents.ts' />
///<reference path='../../Services/UserMetadataService.ts' />

module Orckestra.Composer {
    'use strict';

    export class SignInHeaderController extends Orckestra.Composer.Controller {

        protected userMetadataService: UserMetadataService = new UserMetadataService(new MembershipRepository());

        public initialize() {

            super.initialize();

            this.initializeSignInHeader();
            this.registerSubscriptions();
        }

        private initializeSignInHeader() {
            var cultureInfo = $('html').attr('lang');
            var websiteId = $('html').data('website');
            var param = { cultureInfo, websiteId };

            this.userMetadataService.getUserMetadata(param)
            .then(vm => this.render('SignInHeader', vm));
        }

        protected registerSubscriptions() {
            var loggedInScheduler = EventScheduler.instance(MyAccountEvents[MyAccountEvents.LoggedIn]);
            var loggedOutScheduler = EventScheduler.instance(MyAccountEvents[MyAccountEvents.LoggedOut]);

            loggedOutScheduler.subscribe( e => this.onLoggedOut(e));
            loggedInScheduler.subscribe( e => this.onLoggedIn(e));
        }

        protected onLoggedOut(e: IEventInformation): Q.Promise<any> {
            return this.userMetadataService.invalidateCache();
        }

        protected onLoggedIn(e: IEventInformation): Q.Promise<any> {
             return this.userMetadataService.invalidateCache();
        }
    }
}

