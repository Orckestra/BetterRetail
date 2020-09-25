///<reference path='../../../Typings/tsd.d.ts' />

module Orckestra.Composer {
    'use strict';

    export interface IStoreLocatorHistoryState {
        point?: google.maps.LatLng;
        page?: number;
        zoom?: number;
        center?: google.maps.LatLng;
        pos?: number;
    }
}
