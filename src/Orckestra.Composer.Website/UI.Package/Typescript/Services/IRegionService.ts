///<reference path='../../Typings/tsd.d.ts' />

namespace Orckestra.Composer {
    'use strict';

    export interface IRegionService {

        /**
         * Return a Promise which returns an array of region.
         */
        getRegions(): Promise<any>;
    }
}
