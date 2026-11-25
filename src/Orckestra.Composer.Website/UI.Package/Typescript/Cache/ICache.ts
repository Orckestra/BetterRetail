/// <reference path='../../Typings/tsd.d.ts' />
/// <reference path='./ICachePolicy.ts' />

namespace Orckestra.Composer {

    /**
     * Represents an object cache and provides services for accessing the cache.
    */
    export interface ICache {

        /**
         * If an entry with the given key is found, the promise will be fulfilled with the cached value.
         * If an entry with the given key is found but the value is null, the promise will be rejected.
         * If an entry with the given key is not found, the promise will be rejected.
         * If the entry is expired, the promise will be rejected.
         */
        get<T>(key: string): Promise<T>;

        /**
         * This will handle adding and editing a cache entry, which will never expire.
         * On success, the promise will be fulfilled with the newly cached value.
         */
        set<T>(key: string, value: T): Promise<T>;

        /**
         * This will handle adding and editing an entry in the cache.
         * The entry will expire according to the given policy.
         * On success, the promise will be fulfilled with the newly cached value.
         */
        set<T>(key: string, value: T, policy: ICachePolicy): Promise<T>;

        /**
         * Clear the cache for the specified key
         * On success, the promise will return void
         */
        clear(key: string): Promise<void>;

        /**
         * Clear the cache for the specified type
         * On success, the promise will return void
         */
        fullClear(): Promise<void>;
    }
}
