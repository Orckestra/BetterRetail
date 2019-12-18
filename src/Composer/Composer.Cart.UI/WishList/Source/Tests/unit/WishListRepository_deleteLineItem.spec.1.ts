///<reference path='../../../../../Composer.UI/Source/Typings/tsd.d.ts' />
///<reference path='../../../../../Composer.UI/Source/Tests/unit/Mocks/MockControllerContext.ts' />
///<reference path='../../../../../Composer.UI/Source/Typescript/Mvc/ComposerClient.ts' />
///<reference path='../../../../../Composer.UI/Source/Typescript/Events/EventHub.ts' />
///<reference path='../../../../../Composer.UI/Source/Tests/unit/Mocks/MockControllerContext.ts' />
///<reference path='../../Typescript/WishListRepository.ts' />


module Orckestra.Composer {
    'use strict';

    (() => {

        describe('WHEN delete Wish List item', () => {

            var cut: IWishListRepository;

            beforeEach(() => {
                cut = new WishListRepository();
            });

            it('SHOULD throw an error when the lineItemId is undefined', () => {
                expect(() => cut.deleteLineItem(undefined)).toThrowError(Error);
            });

            it('SHOULD throw an error when the lineItemId is empty', () => {
                expect(() => cut.deleteLineItem('')).toThrowError(Error);
            });
        });
    })();
}
