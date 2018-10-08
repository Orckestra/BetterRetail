/// <reference path='../../../../Composer.UI/Source/Typings/tsd.d.ts' />

module Orckestra.Composer {
    'use strict';

    export class UrlHelper {

        public static resolvePageType() {
           if (window.location.href.indexOf('keywords') !== -1) {
                return 'search';
            } else {
                return 'browse';
            }
        }
    }
}
