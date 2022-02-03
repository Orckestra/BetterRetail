/// <reference path='../../Typings/tsd.d.ts' />
/// <reference path='../Mvc/ComposerClient.ts' />

module Orckestra.Composer {
    'use strict';

    export  interface IOrderRepository {

        editOrder(OrderNumber: string): Q.Promise<any>;

        saveEditOrder():  Q.Promise<any>;

        cancelEditOrder(OrderNumber: string):  Q.Promise<any>;

        getEditedOrder():  Q.Promise<any>;

        getPastOrders(options: IGetOrderParameters): Q.Promise<any>;

        getCurrentOrders(options: IGetOrderParameters):  Q.Promise<any>;
        
        cancelOrder(OrderNumber: string):  Q.Promise<any>;
    }
}
