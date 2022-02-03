module Orckestra.Composer {

    'use strict';

    export  class OrderHelper  {
        public static  MapOrders(orders: any) : any  {
            var mappedOrders = _.map(orders, (order: any) => {
                order.OrderInfos.OrderStatusTextCssClass = order.OrderInfos.OrderStatusRaw === "InProgress" 
                ? "text-warning" 
                : (order.OrderInfos.OrderStatusRaw === "Canceled" ? "text-danger" : "text-success");
                return order;
            });
            return mappedOrders;
        }                            
    };
}