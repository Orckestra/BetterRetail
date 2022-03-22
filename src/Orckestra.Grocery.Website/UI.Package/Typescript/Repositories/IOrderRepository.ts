/// <reference path='../../Typings/tsd.d.ts' />
/// <reference path='../Mvc/ComposerClient.ts' />

module Orckestra.Composer {
  'use strict';

  export interface IOrderRepository {

    editOrder(OrderNumber: string): Q.Promise<any>;

    saveEditOrder(OrderNumber: string): Q.Promise<any>;

    cancelEditOrder(OrderNumber: string): Q.Promise<any>;

    getEditedOrder(): Q.Promise<any>;

    getPastOrders(options: IGetOrdersParameter): Q.Promise<any>;

    getCurrentOrders(options: IGetOrdersParameter): Q.Promise<any>;

    cancelOrder(OrderNumber: string): Q.Promise<any>;

    getOrderByNumber(orderNumber: string);

    getGuestOrderByNumber(orderNumber: string, email: string);
  }
}
