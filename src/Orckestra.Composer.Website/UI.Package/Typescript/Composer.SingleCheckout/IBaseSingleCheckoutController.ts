///<reference path='../../Typings/tsd.d.ts' />
///<reference path='../Mvc/Controller.ts' />

module Orckestra.Composer {
    export interface IBaseSingleCheckoutController extends IController {

        viewModelName: string;

       // unregisterController();

       // renderData(checkoutContext: ICheckoutContext): Q.Promise<void>;

        getValidationPromise(): Q.Promise<boolean>;

        getUpdateModelPromise(): Q.Promise<any>;
    }
}
