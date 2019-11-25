///<reference path='../../../../../Composer.UI/Source/Typings/tsd.d.ts' />
///<reference path='../../Typescript/ShippingMethodCheckoutController.ts' />
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

    describe('WHEN initializing the ShippingMethodCheckoutController', () => {

        var controller: Orckestra.Composer.ShippingMethodCheckoutController,
            eventHub: Orckestra.Composer.IEventHub,
            baseControllerInitStub: SinonStub;

        beforeEach(() => {

            baseControllerInitStub = sinon.stub(Orckestra.Composer.BaseCheckoutController.prototype, 'initialize', () => { return; });
            eventHub = Orckestra.Composer.EventHub.instance();
            controller = new Orckestra.Composer.ShippingMethodCheckoutController(
                Orckestra.Composer.Mocks.MockControllerContext,
                eventHub,
                composerContext,
                composerConfiguration);
            controller.initialize();

        });

        afterEach(() => {
            baseControllerInitStub.restore();
        });

        it('SHOULD viewModelName be ShippingMethod', () => {

            expect(controller.viewModelName).toBe('ShippingMethod');
        });
    });
})();
