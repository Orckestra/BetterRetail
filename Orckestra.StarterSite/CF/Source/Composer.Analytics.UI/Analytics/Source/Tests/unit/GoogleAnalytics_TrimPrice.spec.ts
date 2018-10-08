///<reference path='../../../../../Composer.UI/Source/Typings/tsd.d.ts' />
///<reference path='../../../../../Composer.UI/Source/Typescript/Mvc/ComposerClient.ts' />
///<reference path='../../../../../Composer.UI/Source/Typescript/Events/EventHub.ts' />
///<reference path='../../../../../Composer.UI/Source/Tests/unit/Mocks/MockControllerContext.ts' />
///<reference path='../../../../../Composer.UI/Source/Typings/tsd.d.ts' />
///<reference path='../../../../../Composer.UI/Source/Typescript/Mvc/ComposerClient.ts' />
///<reference path='../../../../../Composer.UI/Source/Typescript/Events/EventHub.ts' />
///<reference path='../../../../../Composer.UI/Source/Tests/unit/Mocks/MockControllerContext.ts' />
///<reference path='../../Typescript/AnalyticsPlugin.ts' />

module Orckestra.Composer {
    (() => {

        'use strict';

        describe('WHEN trimming and localizing an english price', () => {
            it('SHOULD convert the litteral \'$10.55\' to \'10.55\'', () => {
                // assemble
                var plugin = new Orckestra.Composer.AnalyticsPlugin();
                var price = '$10.55';

                // act -- send to trim
                var trimmedPrice = plugin.trimPriceAndUnlocalize(price);

                // assert -- does it match
                expect(trimmedPrice).toEqual(10.55);
            });
        });

    describe('WHEN trimming and localizing a french price', () => {
            it('SHOULD convert the litteral \'10,55 $\' to \'10.55\'', () => {
                // assemble
                var plugin = new Orckestra.Composer.AnalyticsPlugin();
                var price = '10,55 $';

                // act -- send to trim
                var trimmedPrice = plugin.trimPriceAndUnlocalize(price);

                // assert -- does it match
                expect(trimmedPrice).toEqual(10.55);
            });
        });

        describe('WHEN trimming and localizing an english price over 1000', () => {
            it('SHOULD convert the litteral \'$1,000.55\' to \'1000.55\'', () => {
                // assemble
                var plugin = new Orckestra.Composer.AnalyticsPlugin();
                var price = '$1,000.55';

                // act -- send to trim
                var trimmedPrice = plugin.trimPriceAndUnlocalize(price);

                // assert -- does it match
                expect(trimmedPrice).toEqual(1000.55);
            });
        });

        describe('WHEN trimming and localizing a french price over 1000', () => {
            it('SHOULD convert the litteral \'1 000,55 $\' to \'1000.55\'', () => {
                // assemble
                var plugin = new Orckestra.Composer.AnalyticsPlugin();
                var price = '1 000,55 $';

                // act -- send to trim
                var trimmedPrice = plugin.trimPriceAndUnlocalize(price);

                // assert -- does it match
                expect(trimmedPrice).toEqual(1000.55);
            });
        });
    })();
}