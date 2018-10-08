///<reference path='../../../../../Composer.UI/Source/Typings/tsd.d.ts' />
///<reference path='../../Typescript/GuestCustomerInfoCheckoutController.ts' />
///<reference path='../../../../../Composer.UI/Source/Tests/unit/Mocks/MockControllerContext.ts' />
///<reference path='../../../../../Composer.UI/Source/Typescript/IComposerConfiguration.ts' />

(() => {

    'use strict';

    var composerContext: Orckestra.Composer.IComposerContext = {
        language: 'en-CA'
    };
    var composerConfiguration: Orckestra.Composer.IComposerConfiguration = {
        controllers: []
    };

    describe('WHEN initializing the GuestCustomerInfoCheckoutController', () => {

        var controller: Orckestra.Composer.GuestCustomerInfoCheckoutController,
            eventHub: Orckestra.Composer.IEventHub,
            baseControllerInitStub: SinonStub;

        beforeEach(() => {

            baseControllerInitStub = sinon.stub(Orckestra.Composer.BaseCheckoutController.prototype, 'initialize', () => { return; });
            eventHub = Orckestra.Composer.EventHub.instance();
            controller = new Orckestra.Composer.GuestCustomerInfoCheckoutController(
                Orckestra.Composer.Mocks.MockControllerContext,
                eventHub,
                composerContext,
                composerConfiguration);
            controller.initialize();

        });

        afterEach(() => {
            baseControllerInitStub.restore();
        });

        it('SHOULD viewModelName be GuestCustomerInfo', () => {

            expect(controller.viewModelName).toBe('GuestCustomerInfo');
        });
    });
})();
