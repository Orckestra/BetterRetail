///<reference path='../../Typings/tsd.d.ts' />
///<reference path='../Mvc/Controller.ts' />

namespace Orckestra.Composer {
    export interface IBaseSingleCheckoutController extends IController {

        viewModelName: string;

        getUpdateModelPromise(): Promise<any>;

        getViewModelNameForUpdatePromise(): Promise<any>;
    }
}
