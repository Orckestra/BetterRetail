///<reference path='../../Typings/tsd.d.ts' />
///<reference path='../Mvc/Controller.ts' />

module Orckestra.Composer {
    export interface IBaseSingleCheckoutController extends IController {

        viewModelName: string;

        getUpdateModelPromise(): Q.Promise<any>;

        getViewModelNameForUpdatePromise(): Q.Promise<any>;
    }
}
