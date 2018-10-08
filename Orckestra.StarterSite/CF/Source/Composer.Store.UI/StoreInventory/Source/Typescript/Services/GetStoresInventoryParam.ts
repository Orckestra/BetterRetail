///<reference path='../../../../../Composer.UI/Source/Typings/tsd.d.ts' />

module Orckestra.Composer {
    'use strict';

    export class GetStoresInventoryParam {
        Sku: string;
        SearchPoint: google.maps.LatLng;
        Page: number;
        Pagesize: number;
    }
}