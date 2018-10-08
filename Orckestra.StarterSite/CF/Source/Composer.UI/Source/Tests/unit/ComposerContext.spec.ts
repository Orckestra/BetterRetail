/// <reference path='../../Typings/tsd.d.ts' />
/// <reference path='../../Typescript/Mvc/ControllerRegistry.ts' />
/// <reference path='../../Typescript/IComposerContext.ts' />
/// <reference path='../../Typescript/ComposerContext.ts' />

'use strict';

// TODO: Move this in to it's own file once the composer.mocks.js is built.


(() => {
    // Visit http://jasmine.github.io for more information on unit testing with Jasmine.
    // For more info on the Karma test runner, visit http://karma-runner.github.io

    describe('ComposerContext.spect.ts', () => {
        describe('When instantiating the ComposerContext the language property', () => {
            var composerContext: Orckestra.Composer.IComposerContext;

            beforeEach(() => {
                document.getElementsByTagName('html')[0].setAttribute('lang', 'en-CA');
                composerContext = new Orckestra.Composer.ComposerContext();
            });

            it('SHOULD return the current locale', () => {
                var expected = 'en-CA';

                expect(composerContext.language).toBe(expected);
            });
        });
    });
})();
