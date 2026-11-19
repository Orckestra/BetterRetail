///<reference path='../../../Typings/tsd.d.ts' />

namespace Orckestra.Composer {
    'use strict';

    export interface IMapOptions {
        mapCanvas: HTMLElement;
        options: google.maps.MapOptions;
        infoWindowMaxWidth?: number;
    }
}
