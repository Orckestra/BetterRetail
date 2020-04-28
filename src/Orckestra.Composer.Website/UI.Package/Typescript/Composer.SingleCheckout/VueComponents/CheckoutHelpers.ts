/// <reference path='../../../Typings/tsd.d.ts' />

module Orckestra.Composer {
    'use strict';

    export class CheckoutHelpers {
        public static getFocusedElementId() {
            return document.activeElement.id;
        }

        public static getFocusedStepIndex(steps: any = []) {
            let activeId = this.getFocusedElementId();
            return steps.findIndex(step => step.tabId === activeId);
        }

        public static findElementAndFocus(elemId) {
            let elem = document.getElementById(elemId);
            if (elem) {
                elem.focus();
            }
        }

        public static isPromise(func) {
            return func.then && typeof func.then === 'function';
        }
    }
}
