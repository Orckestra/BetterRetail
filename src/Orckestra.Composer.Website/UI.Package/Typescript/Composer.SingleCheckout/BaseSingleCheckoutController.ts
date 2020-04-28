///<reference path='../../Typings/tsd.d.ts' />
///<reference path='../JQueryPlugins/ISerializeObjectJqueryPlugin.ts' />
///<reference path='../ErrorHandling/ErrorHandler.ts' />
///<reference path='./IBaseSingleCheckoutController.ts' />
///<reference path='../Composer.Cart/CheckoutCommon/ICheckoutContext.ts' />
///<reference path='./Services/ISingleCheckoutService.ts' />
///<reference path='./Services/SingleCheckoutService.ts' />

module Orckestra.Composer {
    'use strict';

    export class BaseSingleCheckoutController extends Orckestra.Composer.Controller implements Orckestra.Composer.IBaseSingleCheckoutController {

        protected formInstances: IParsley[];
        protected formSelector: string = 'form';
        protected checkoutService: ISingleCheckoutService;

        public viewModelName: string;

        public initialize() {
            super.initialize();

            this.checkoutService = SingleCheckoutService.getInstance();

            this.registerController();

        }

        protected registerController() {

           this.checkoutService.registerController(this);
        }

        public unregisterController() {

           this.checkoutService.unregisterController(this.viewModelName);
        }

        public getUpdateModelPromise(): Q.Promise<any> {
            return Q.resolve(null);
        }

        public getViewModelNameForUpdatePromise(): Q.Promise<any> {
            return Q.resolve(null);
        }

        protected getContainer(): JQuery {
            return $(`[data-oc-controller='${this.context.container.data('oc-controller')}']`);
        }
    }
}
