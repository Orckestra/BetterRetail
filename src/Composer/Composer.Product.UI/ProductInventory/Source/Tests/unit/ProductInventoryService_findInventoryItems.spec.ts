/// <reference path='../../../../../Composer.UI/Source/Typings/tsd.d.ts' />
/// <reference path='../../../../../Composer.UI/Source/TypeScript/Events/EventHub.ts' />
/// <reference path='../../../../../Composer.UI/Source/TypeScript/Events/IEventHub.ts' />
/// <reference path='../../../../../Composer.UI/Source/Tests/Unit/Mocks/MockControllerContext.ts' />
/// <reference path='../../../../../Composer.UI/Source/Tests/unit/Mocks/MockJQueryEventObject.ts' />
/// <reference path='../../../../../Composer.UI/Source/Typescript/IComposerConfiguration.ts' />
/// <reference path='../../../../../Composer.UI/Source/Typescript/Mvc/IControllerContext.ts' />

// (() => {
//     'use strict';
//
//     var spy: jasmine.Spy,
//         eventHub: Orckestra.Composer.IEventHub,
//         productInventoryService: Orckestra.Composer.ProductInventoryService;
//
//     describe('WHEN calling the ProductInventoryService.findInventoryItems method', () => {
//         beforeEach((done) => {
//             spy = jasmine.createSpy('inventoryChanged');
//             eventHub = Orckestra.Composer.EventHub.instance();
//
//             productInventoryService = new Orckestra.Composer.ProductInventoryService(eventHub);
//
//             eventHub.subscribe('inventoryChanged', (eventInformation: Orckestra.Composer.IEventInformation) => {
//                 spy(eventInformation);
//                 done();
//             });
//
//             var promise = Q.fcall(() => {
//                     return ['SKU-Dummy'];
//                 });
//
//             spyOn(Orckestra.Composer.ComposerClient, 'post').and.returnValue(promise);
//
//             productInventoryService.findInventoryItems(['SKU-Dummy'], 'test');
//         });
//
//     it('SHOULD publish the inventoryChanged event.', () => {
//             expect(Orckestra.Composer.ComposerClient.post).toHaveBeenCalled();
//             expect(spy).toHaveBeenCalledWith({ data: ['SKU-Dummy'] });
//         });
//     });
// })();
