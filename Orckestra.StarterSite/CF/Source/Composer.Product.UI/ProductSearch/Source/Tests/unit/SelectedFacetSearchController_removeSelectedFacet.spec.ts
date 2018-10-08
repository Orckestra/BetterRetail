/// <reference path='../../../../../Composer.UI/Source/Typings/tsd.d.ts' />
/// <reference path='../../TypeScript/SelectedFacetSearchController.ts' />
/// <reference path='../../../../../Composer.UI/Source/Tests/unit/Mocks/MockControllerContext.ts' />
/// <reference path='../../../../../Composer.UI/Source/Tests/unit/Mocks/MockJQueryEventObject.ts' />
/// <reference path='../../../../../Composer.UI/Source/TypeScript/Events/EventHub.ts' />
/// <reference path='../../../../../Composer.UI/Source/TypeScript/Events/IEventHub.ts' />
/// <reference path='../../../../../Composer.UI/Source/TypeScript/Events/IEventInformation.ts' />
/// <reference path='../../../../../Composer.UI/Source/Typescript/IComposerConfiguration.ts' />

(() => {
    'use strict';

    var composerContext: Orckestra.Composer.IComposerContext = {
        language: 'en-CA'
    };
    var composerConfiguration: Orckestra.Composer.IComposerConfiguration = {
        controllers: []
    };
    // Visit http://jasmine.github.io for more information on unit testing with Jasmine.
    // For more info on the Karma test runner, visit http://karma-runner.github.io
    var controller: Orckestra.Composer.SelectedFacetSearchController,
        eventHub: Orckestra.Composer.IEventHub,
        spy: SinonSpy,
        controllerActionContext: Orckestra.Composer.IControllerActionContext;

    describe('WHEN calling the SelectedFacetSearchController.removeSelectedFacet method', () => {
        beforeEach(() => {
            spy = sinon.spy();
            eventHub = Orckestra.Composer.EventHub.instance();
            controller = new Orckestra.Composer.SelectedFacetSearchController(
                Orckestra.Composer.Mocks.MockControllerContext,
                eventHub,
                composerContext,
                composerConfiguration);
            controllerActionContext = {
                elementContext: $(''),
                event: Orckestra.Composer.Mocks.MockJqueryEventObject
            };
        });

        it('SHOULD publish the facetRemoved event.', () => {
            eventHub.subscribe('facetRemoved', (eventInformation: Orckestra.Composer.IEventInformation) => {
                spy();
            });

            controller.removeSelectedFacet(controllerActionContext);
            expect(spy.called).toBe(true);
        });
    });

    describe('WHEN calling the SelectedFacetSearchController.addSingleSelectCategory method', () => {
       beforeEach(() => {
           spy = sinon.spy();
           eventHub = Orckestra.Composer.EventHub.instance();
           controller = new Orckestra.Composer.SelectedFacetSearchController(
               Orckestra.Composer.Mocks.MockControllerContext,
               eventHub,
               composerContext,
               composerConfiguration);
           controllerActionContext = {
               elementContext: $(''),
               event: Orckestra.Composer.Mocks.MockJqueryEventObject
           };
       });

       it('SHOULD publish the singleCategoryAdded event.', () => {
          eventHub.subscribe('singleCategoryAdded', (eventInformation: Orckestra.Composer.IEventInformation) => {
              spy();
          });

          controller.addSingleSelectCategory(controllerActionContext);
          expect(spy.called).toBe(true);
       });
    });
})();
