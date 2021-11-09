///<reference path='../../../../Typings/tsd.d.ts' />
///<reference path='../../../Mvc/ComposerClient.ts' />
///<reference path='../Parameters/IGetOrdersParameter.ts' />

module Orckestra.Composer {
    'use strict';

    export class OrderService {
        public getPastOrders(options: IGetOrdersParameter = { page: 1 }) {
            return ComposerClient.post('/api/order/past-orders', options);
        }

        public getCurrentOrders(options: IGetOrdersParameter = { page: 1 }) {
            return ComposerClient.post('/api/order/current-orders', options);
        }

        public getOrderByNumber(orderNumber: string) {
            return ComposerClient.post('/api/order/orderbynumber', { OrderNumber: orderNumber });
        }

        public getGuestOrderByNumber(orderNumber: string, email: string) {
            return ComposerClient.post('/api/order/guestorderbynumber', { OrderNumber: orderNumber, Email: email });
        }
    }
}
