///<reference path='../../Typings/tsd.d.ts' />
///<reference path='../Repositories/CustomerRepository.ts' />
///<reference path='../Cache/CacheProvider.ts' />

module Orckestra.Composer {
    'use strict';

    export class UserMetadataService {

        private cacheKey: string = 'UserMetadata';
        private cachePolicy: ICachePolicy = { slidingExpiration: 300 }; // 5min
        private cacheProvider: ICacheProvider;
        private membershipRepository: IMembershipRepository;

        constructor(membershipRepository: IMembershipRepository) {

            if (!membershipRepository) {
                throw new Error('Error: membershipRepository is required');
            }

            this.cacheProvider = CacheProvider.instance();
            this.membershipRepository = membershipRepository;
        }

        public getUserMetadata(param: any): Q.Promise<any> {

            return this.getFromCache(param)
                .fail(reason => {

                    if (this.canHandle(reason)) {
                        return this.getFreshMetadata(param);
                    }

                    throw reason;
                });
        }

        private canHandle(reason: any): boolean {

            return reason === CacheError.Expired || reason === CacheError.NotFound;
        }

        public getFreshMetadata(param: any): Q.Promise<any> {

            return this.membershipRepository.userMetadata()
                .then(result => this.setToCache(param, result));
        }

        public buildCacheKey(param: any): string {

            return this.cacheKey + '.' + param.cultureInfo + '.' + param.isAuthenticated + '.' + param.encryptedCustomerId + '.' + param.websiteId;
        }

        public invalidateCache(): Q.Promise<void> {

            return this.cacheProvider.sessionCache.fullClear();
        }

        private getFromCache(param: any): Q.Promise<any> {

            var composedKey = this.buildCacheKey(param);

            return this.cacheProvider.sessionCache.get<any>(composedKey);
        }

        private setToCache(param: any, cart: any): Q.Promise<any> {

            var composedKey = this.buildCacheKey(param);

            return this.cacheProvider.sessionCache.set(composedKey, cart, this.cachePolicy);
        }
    }
}