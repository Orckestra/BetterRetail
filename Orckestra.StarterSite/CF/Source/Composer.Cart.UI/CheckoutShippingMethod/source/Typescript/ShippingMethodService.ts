///<reference path='../../../../Composer.UI/Source/Typings/tsd.d.ts' />
///<reference path='../../../../Composer.UI/Source/Typescript/Mvc/ComposerClient.ts' />

module Orckestra.Composer {
    'use strict';

    export class ShippingMethodService {

        /**
        * Return a Promise which returns the ShippingMethods available for the cart.
        */
        public getShippingMethods(): Q.Promise<any> {

            return ComposerClient.get('/api/cart/shippingmethods');
        }
    }
}
