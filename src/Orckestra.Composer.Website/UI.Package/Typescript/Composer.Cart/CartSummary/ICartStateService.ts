/// <reference path='../../../Typings/tsd.d.ts' />

module Orckestra.Composer {

    /**
     * Separates the logic that retrieves the data and maps it to the entity model from the application services that acts on the model.
    */
    export interface ICartStateService {

        /**
         * Get the cart vue state
         */
        VueFullCart: Vue;
        /**
         * Get the cart vue mixins
         */
        VueCartMixins: any

    }
}
