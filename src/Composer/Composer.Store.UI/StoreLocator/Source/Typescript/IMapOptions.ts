///<reference path='../../../../Composer.UI/Source/Typings/tsd.d.ts' />

module Orckestra.Composer {
    'use strict';

    export interface IMapOptions {
        mapCanvas: HTMLElement;
        options: google.maps.MapOptions;
        infoWindowMaxWidth?: number;
    }
}
