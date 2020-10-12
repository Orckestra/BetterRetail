///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Events/EventHub.ts' />
///<reference path='../CartSummary/CartService.ts' />
///<reference path='../CartSummary/CartEvents.ts' />

module Orckestra.Composer {
    'use strict';

    export class OrderSummaryService {

        private cartService: ICartService;
        private eventHub: IEventHub;

        constructor(cartService: ICartService, eventHub: IEventHub) {

            if (!cartService) {
                throw new Error('Error: cartService is required');
            }
            if (!eventHub) {
                throw new Error('Error: eventHub is required');
            }

            this.cartService = cartService;
            this.eventHub = eventHub;
        }

        public setCheapestShippingMethodUsing(postalCode: string): Q.Promise<void> {

            this.eventHub.publish(CartEvents.CartUpdating, { data: { PostalCode: postalCode } });

            return this.cartService.updateShippingMethodPostalCode(postalCode)
                       .then(() => this.cartService.setCheapestShippingMethod())
                       .then(() => this.cartService.getCart())
                       .then(cart => this.eventHub.publish(CartEvents.CartUpdated, { data: cart }))
                       .fail((reason: any) => {
                           console.error('Error while updating the shipping method using the postal code', reason);
                           throw reason;
                       });
        }

        public cleanCart(): Q.Promise<void> {

            return this.cartService.clean();
        }
    }
}
