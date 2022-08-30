///<reference path='../../Typings/tsd.d.ts' />
///<reference path='../Repositories/CustomerRepository.ts' />
///<reference path='../Repositories/IMembershipRepository.ts' />
///<reference path='../Repositories/MembershipRepository.ts' />
///<reference path='../Cache/CacheProvider.ts' />
///<reference path='../Utils/Utils.ts' />

module Orckestra.Composer {
    'use strict';

    export class UserMetadataService {

        private cacheKey: string = 'UserMetadata';
        private cachePolicy: ICachePolicy = { slidingExpiration: 300 }; // 5min
        private cacheProvider: ICacheProvider;
        private membershipRepository: IMembershipRepository;
        private static instance: UserMetadataService;

        constructor(membershipRepository: IMembershipRepository) {

            if (!membershipRepository) {
                throw new Error('Error: membershipRepository is required');
            }

            this.cacheProvider = CacheProvider.instance();
            this.membershipRepository = membershipRepository;
            UserMetadataService.instance = this;
        }

        public static getInstance(): UserMetadataService {

            if (!UserMetadataService.instance) {
                UserMetadataService.instance = new UserMetadataService(new MembershipRepository());
            }

            return UserMetadataService.instance;
        }

        public getUserMetadata(): Q.Promise<any> {

            return this.getFromCache()
                .fail(reason => {

                    if (this.canHandle(reason)) {
                        return this.getFreshMetadata();
                    }

                    throw reason;
                });
        }

        private canHandle(reason: any): boolean {

            return reason === CacheError.Expired || reason === CacheError.NotFound;
        }

        public getFreshMetadata(): Q.Promise<any> {

            return this.membershipRepository.userMetadata()
                .then(result => this.setToCache(result));
        }

        public buildCacheKey(): string {

            return `${this.cacheKey}.${Utils.getCulture()}.${Utils.getWebsiteId()}`;
        }

        public invalidateCache(): Q.Promise<void> {

            return this.cacheProvider.defaultCache.clear(this.buildCacheKey());
        }

        private getFromCache(): Q.Promise<any> {

            var composedKey = this.buildCacheKey();
            return this.cacheProvider.defaultCache.get<any>(composedKey);
        }

        private setToCache(cart: any): Q.Promise<any> {

            var composedKey = this.buildCacheKey();
            return this.cacheProvider.defaultCache.set(composedKey, cart, this.cachePolicy);
        }
    }
}