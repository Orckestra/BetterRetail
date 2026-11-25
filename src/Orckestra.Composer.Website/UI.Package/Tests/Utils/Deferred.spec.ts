/// <reference path='../../Typings/tsd.d.ts' />
/// <reference path='../../Typescript/Utils/Deferred.ts' />

namespace Orckestra.Composer {

    'use strict';

    describe('WHEN using a Deferred', () => {

        it('SHOULD be pending until resolved', async () => {
            const deferred = new Deferred<number>();
            expect(deferred.isPending).toBe(true);

            deferred.resolve(42);

            expect(await deferred.promise).toBe(42);
            expect(deferred.isPending).toBe(false);
        });

        it('SHOULD stop being pending once rejected', async () => {
            const deferred = new Deferred<number>();

            deferred.reject('failure');

            const reason = await deferred.promise.catch(r => r);

            expect(reason).toBe('failure');
            expect(deferred.isPending).toBe(false);
        });
    });
}
