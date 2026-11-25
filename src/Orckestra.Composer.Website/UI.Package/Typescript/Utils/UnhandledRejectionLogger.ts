/// <reference path='../../Typings/tsd.d.ts' />

namespace Orckestra.Composer {

    'use strict';

    /**
     * Logs promise rejections nobody handled.
     * Replaces the error rethrow that Q performed in done().
     */
    export class UnhandledRejectionLogger {

        public static register(window: Window): void {
            window.addEventListener('unhandledrejection', UnhandledRejectionLogger.log);
        }

        public static log(event: PromiseRejectionEvent): void {
            console.error('Unhandled promise rejection', event.reason);
        }
    }
}
