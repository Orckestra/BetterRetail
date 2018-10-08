///<reference path='../../../../Composer.UI/Source/Typings/tsd.d.ts' />
///<reference path='../../../../Composer.UI/Source/TypeScript/Mvc/Controller.ts' />
///<reference path='../../../../Composer.UI/Source/TypeScript/Events/EventScheduler.ts' />
///<reference path='../../../Common/Source/Typescript/MyAccountEvents.ts' />
///<reference path='./SignInHeaderService.ts' />

module Orckestra.Composer {
    'use strict';

    export class SignInHeaderController extends Orckestra.Composer.Controller {

        protected signInHeaderService: SignInHeaderService = new SignInHeaderService(new SignInHeaderRepository());

        public initialize() {

            super.initialize();

            this.initializeSignInHeader();
            this.registerSubscriptions();
        }

        private initializeSignInHeader() {
            var cultureInfo = $('html').attr('lang');
            var param = { cultureInfo };

            this.signInHeaderService.getSignInHeader(param)
                .then(signInHeader => this.render('SignInHeader', signInHeader));
        }

        protected registerSubscriptions() {
            var loggedInScheduler = EventScheduler.instance(MyAccountEvents[MyAccountEvents.LoggedIn]);
            var loggedOutScheduler = EventScheduler.instance(MyAccountEvents[MyAccountEvents.LoggedOut]);

            loggedOutScheduler.subscribe( e => this.onLoggedOut(e));
            loggedInScheduler.subscribe( e => this.onLoggedIn(e));
        }

        protected onLoggedOut(e: IEventInformation): Q.Promise<any> {
            return this.signInHeaderService.invalidateCache();
        }

        protected onLoggedIn(e: IEventInformation): Q.Promise<any> {
             return this.signInHeaderService.invalidateCache();
        }
    }
}

