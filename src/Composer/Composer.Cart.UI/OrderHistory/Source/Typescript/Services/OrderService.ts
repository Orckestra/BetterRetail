///<reference path='../../../../../Composer.UI/Source/Typings/tsd.d.ts' />
///<reference path='../../../../../Composer.UI/Source/Typescript/Mvc/ComposerClient.ts' />
///<reference path='../Parameters/IGetOrderParameters' />

module Orckestra.Composer {
    'use strict';

    export class OrderService {
        public getPastOrders(options: IGetOrderParameters = { page: 1 }) {
            return ComposerClient.post('/api/order/past-orders', options);
        }

        public getCurrentOrders(options: IGetOrderParameters = { page: 1 }) {
            return ComposerClient.post('/api/order/current-orders', options);
        }
    }
}
