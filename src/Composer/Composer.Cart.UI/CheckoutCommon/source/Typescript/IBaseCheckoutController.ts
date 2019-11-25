///<reference path='../../../../Composer.UI/Source/Typings/tsd.d.ts' />
///<reference path='../../../../Composer.UI/Source/TypeScript/Mvc/Controller.ts' />

module Orckestra.Composer {
    export interface IBaseCheckoutController extends IController {

        viewModelName: string;

        unregisterController();

        renderData(checkoutContext: ICheckoutContext): Q.Promise<void>;

        getValidationPromise(): Q.Promise<boolean>;

        getUpdateModelPromise(): Q.Promise<any>;
    }
}
