/// <reference path='../../Typings/tsd.d.ts' />

namespace Orckestra.Composer {

    'use strict';

    /**
     * Native Promise wrapper exposing resolve, reject and a pending flag.
     * Replacement for new Deferred() and promise.isPending().
     */
    export class Deferred<T = void> {

        public readonly promise: Promise<T>;
        public resolve: (value?: T | PromiseLike<T>) => void;
        public reject: (reason?: any) => void;
        private _pending = true;

        constructor() {
            this.promise = new Promise<T>((resolve, reject) => {
                this.resolve = value => { this._pending = false; resolve(value as T); };
                this.reject = reason => { this._pending = false; reject(reason); };
            });
        }

        public get isPending(): boolean {
            return this._pending;
        }
    }
}
