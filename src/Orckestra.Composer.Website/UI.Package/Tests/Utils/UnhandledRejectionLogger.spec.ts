/// <reference path='../../Typings/tsd.d.ts' />
/// <reference path='../../Typescript/Utils/UnhandledRejectionLogger.ts' />

namespace Orckestra.Composer {

    'use strict';

    describe('WHEN an UnhandledRejectionLogger is registered', () => {

        it('SHOULD log the reason of an unhandled rejection', () => {
            const spy = sinon.stub(console, 'error');

            UnhandledRejectionLogger.log(<PromiseRejectionEvent>{ reason: 'failure' });

            expect(spy.calledWith('Unhandled promise rejection', 'failure')).toBe(true);
            spy.restore();
        });
    });
}
