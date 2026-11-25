///<reference path='../../../Typings/tsd.d.ts' />

namespace Orckestra.Composer {

    export interface IValidationCallback {
        (): Promise<boolean>;
    }

    export interface IUpdateCallback {
        (): Promise<any>;
    }

    export interface IRegisterOptions {
        validationCallback: Function;
        updateCallback: Function;
    }

    export interface IRegisterControlOptions extends IRegisterOptions {
        isReady: boolean;
    }
}
