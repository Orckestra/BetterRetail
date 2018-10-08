///<reference path='../../../../../Composer.UI/Source/Typings/tsd.d.ts' />
///<reference path='../../../../../Composer.UI/Source/Typescript/Mvc/ComposerClient.ts' />
///<reference path='../../../../../Composer.UI/Source/Typescript/Events/EventHub.ts' />
///<reference path='../../../../../Composer.UI/Source/Tests/unit/Mocks/MockControllerContext.ts' />
///<reference path='../../Typescript/MembershipService.ts' />

module Orckestra.Composer {
    'use strict';

    (() => {
        describe('WHEN MembershipService.register', () => {
            var membershipService: IMembershipService,
                composerClientStub: SinonStub;

            beforeEach(function() {
                composerClientStub = sinon.stub(Orckestra.Composer.ComposerClient, 'post', () => {
                    return Q.fcall(() => {
                        return true;
                    });
                });
                composerClientStub.withArgs('/api/membership/register');

                membershipService = new MembershipService(new MembershipRepository());
                membershipService.register({}, '');
            });

            afterEach(() => {
                composerClientStub.restore();
            });

            it('SHOULD call the regiser web api', () => {
                expect(composerClientStub.calledOnce).toBe(true);
            });
        });
    })();
}
