///<reference path='../../../../Composer.UI/Source/Typings/tsd.d.ts' />
///<reference path='../../../../Composer.UI/Source/Typescript/Mvc/IControllerContext.ts' />
///<reference path='../../../../Composer.UI/Source/Typescript/Mvc/ComposerClient.ts' />
///<reference path='./IStoreService.ts' />

module Orckestra.Composer {
    'use strict';

    export class StoreService implements IStoreService {
        private static _instance: StoreService = new StoreService();

        public static instance(): StoreService {
            return StoreService._instance;
        }

        public getStores() {
            return ComposerClient.get('/api/store/stores');
        }
    }
}
