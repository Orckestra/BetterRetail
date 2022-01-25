///<reference path='../../../../Typings/tsd.d.ts' />
///<reference path='../../../Mvc/ComposerClient.ts' />
///<reference path='../Parameters/IGetOrderParameters.ts' />

module Orckestra.Composer {
    'use strict';

    export class OrderService {

        public editOrder(orderNumber: string, orderId: string) {
            return ComposerClient.post('/api/order/edit-order', { OrderNumber: orderNumber, OrderId: orderId });
        }

        public saveEditOrder() {
            return ComposerClient.post('/api/order/save-edited-order', {});
        }

        public cancelEditOrder() {
            return ComposerClient.post('/api/order/cancel-edit-order', null);
        }

        public getEditedOrder() {
            return ComposerClient.post('/api/order/get-edited-order', {});
        }

        public getPastOrders(options: IGetOrderParameters = { page: 1 }) {
            return ComposerClient.post('/api/order/past-orders', options);
        }

        public getCurrentOrders(options: IGetOrderParameters = { page: 1 }) {
            return ComposerClient.post('/api/order/current-orders', options);
        }
    }
}
