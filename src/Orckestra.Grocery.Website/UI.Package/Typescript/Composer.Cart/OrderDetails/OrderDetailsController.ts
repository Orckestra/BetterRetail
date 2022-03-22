/// <reference path='../../../Typings/tsd.d.ts' />
/// <reference path='../../JQueryPlugins/ISerializeObjectJqueryPlugin.ts' />
/// <reference path='../../Mvc/Controller.ts' />
/// <reference path='../../Mvc/IControllerContext.ts' />
/// <reference path='../../Mvc/IControllerActionContext.ts' />
///<reference path='../OrderHistory/Services/OrderService.ts' />
///<reference path='../../UI/UIModal.ts' />
///<reference path='../../Composer.Grocery/FulfillmentEvents.ts' />
///<reference path='../../Composer.Grocery/FulfillmentService.ts' />
///<reference path='../../Composer.MyAccount/Common/MyAccountEvents.ts' />

module Orckestra.Composer {
    'use strict';

    export class OrderDetailsController extends Controller {
        protected orderService = new OrderService();
        protected fulfillmentService: IFulfillmentService = FulfillmentService.instance();
        protected VueOrderDetails: Vue;

        public initialize() {
            super.initialize();
            var cancelModalElementSelector = '#cancelOrderModal';

            let self = this;
            self.eventHub.subscribe(MyAccountEvents.EditOrderCanceled, () => window.location.reload());

            this.VueOrderDetails = new Vue({
                el: '#vueOrderDetails',
                data: {
                    Loading: false,
                    Modal: {
                        cancelOrderModal: null,
                    },
                    OrderNumber: null
                },
                mounted() {
                    this.Modal.cancelOrderModal = new Composer.UIModal(window, cancelModalElementSelector, this.cancelOrder, this);
                    self.eventHub.subscribe(MyAccountEvents.OrderCanceled, () => this.reload());
                },
                methods: {
                    editOrder(orderNumber: string) {
                        if (this.Loading) return;
                        this.Loading = true;
                        self.eventHub.publish(MyAccountEvents.StartEditOrder, { data: orderNumber });
                        self.fulfillmentService.setOrderFulfillment(orderNumber)
                        .then(fulfillment => {
                            self.eventHub.publish(FulfillmentEvents.StoreSelected, { data: fulfillment.Store });
                            self.eventHub.publish(FulfillmentEvents.TimeSlotSelected, {
                                data: { TimeSlot: fulfillment.TimeSlot, TimeSlotReservation: fulfillment.TimeSlotReservation }
                            });
                            return fulfillment;
                        })
                        .then(() => self.orderService.editOrder(orderNumber));
                    },
                    cancelEditingOrder(orderNumber: string) {
                        if (this.Loading) return;
                        this.Loading = true;
                        self.orderService.cancelEditOrder(orderNumber)
                            .fail(() => {
                                this.Loading = false;
                            });
                    },
                    reload() {
                        window.location.reload()
                    },
                    cancelOrderConfirm(event: JQueryEventObject, orderNumber: string) {
                        this.OrderNumber = orderNumber;
                        this.Modal.cancelOrderModal.openModal(event);
                    },
                    cancelOrder() {
                        if (this.Loading) return;
                        this.Loading = true;
                        self.orderService.cancelOrder(this.OrderNumber).fail(() => this.Loading = false);
                    }
                }
            });
        }
    }
}

