///<reference path='../../../../../Composer.UI/Source/Typings/tsd.d.ts' />
///<reference path='../../../../../Composer.UI/Source/Tests/unit/Mocks/MockControllerContext.ts' />
///<reference path='../../Typescript/GoogleAnalyticsPlugin.ts' />
///<reference path='../../Typescript/AnalyticsPlugin.ts' />
///<reference path='../../../../../Composer.MyAccount.UI/Common/Source/Typescript/MyAccountEvents.ts' />

module Orckestra.Composer {
    (() => {

        'use strict';

        describe('WHEN user logs in, create account or request password recovery', () => {

            let analytics : GoogleAnalyticsPlugin;
            let eventHub: IEventHub;
            var loginType, returnUrl, viewModel = null;

            beforeEach (() => {
                analytics = new GoogleAnalyticsPlugin();
                analytics.initialize();
                eventHub = EventHub.instance();
                loginType = 'regular';
                returnUrl = '/my-account';
                viewModel = {
                    ReturnUrl : returnUrl
                };
            });

            describe('WITH user logs in', () => {

                it('SHOULD fire the userLoggedIn function', () => {
                    // arrange
                    spyOn(analytics, 'userLoggedIn');

                    // act -- publish
                    eventHub.publish(MyAccountEvents[MyAccountEvents.LoggedIn], {data: viewModel});

                    // assert -- does it match
                    expect(analytics.userLoggedIn).toHaveBeenCalledWith(loginType, returnUrl);
                });
            });

            describe('WITH user creates an account', () => {

                it('SHOULD fire the userCreated function', () => {
                    // arrange
                    spyOn(analytics, 'userCreated');

                    // act -- publish
                    eventHub.publish(MyAccountEvents[MyAccountEvents.AccountCreated], {data: viewModel});

                    // assert -- does it match
                    expect(analytics.userCreated).toHaveBeenCalledWith();
                });
            });

            describe('WITH user recovers a password', () => {

                it('SHOULD fire the recoverPassword function', () => {
                    // arrange
                    spyOn(analytics, 'recoverPassword');

                    // act -- publish
                    eventHub.publish(MyAccountEvents[MyAccountEvents.ForgotPasswordInstructionSent], {data: viewModel});

                    // assert -- does it match
                    expect(analytics.recoverPassword).toHaveBeenCalledWith();
                });
            });
        });
    })();
}