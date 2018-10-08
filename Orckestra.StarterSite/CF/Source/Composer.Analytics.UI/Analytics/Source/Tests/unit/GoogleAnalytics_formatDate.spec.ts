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

        describe('WHEN a date is given in any format', () => {
            it('SHOULD convert to yyyy-mm-dd', () => {
                // assemble
                var plugin = new Orckestra.Composer.AnalyticsPlugin();
                var date = 'Fri Sep 22,2016';

                // act -- send to formatDate
                var formattedDate = plugin.formatDate(date);

                // assert -- does it match
                expect(formattedDate).toEqual('2016-09-22');
            });
        });
    })();
}