/// <reference path='../../Typings/tsd.d.ts' />
/// <reference path='../Mvc/ComposerClient.ts' />

namespace Orckestra.Composer {
    'use strict';

    export  interface IOrderRepository {

        editOrder(OrderNumber: string): Promise<any>;

        saveEditOrder(OrderNumber: string):  Promise<any>;

        cancelEditOrder(OrderNumber: string):  Promise<any>;

        getEditedOrder():  Promise<any>;

        getPastOrders(options: IGetOrderParameters): Promise<any>;

        getCurrentOrders(options: IGetOrderParameters):  Promise<any>;
        
        cancelOrder(OrderNumber: string):  Promise<any>;
    }
}
