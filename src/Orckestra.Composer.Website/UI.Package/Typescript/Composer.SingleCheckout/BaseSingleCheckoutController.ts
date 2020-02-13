///<reference path='../../Typings/tsd.d.ts' />
///<reference path='../JQueryPlugins/ISerializeObjectJqueryPlugin.ts' />
///<reference path='../ErrorHandling/ErrorHandler.ts' />
///<reference path='./IBaseSingleCheckoutController.ts' />
///<reference path='../Composer.Cart/CheckoutCommon/ICheckoutContext.ts' />

module Orckestra.Composer {
    'use strict';

    export class BaseSingleCheckoutController extends Orckestra.Composer.Controller implements Orckestra.Composer.IBaseSingleCheckoutController {

       // protected formInstances: IParsley[];
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

           // this.checkoutService.unregisterController(this.viewModelName);
        }


        //public getValidationPromise(): Q.Promise<boolean> {

            //return Q.fcall(() => this.isValidForUpdate());
       // }

        public getUpdateModelPromise(): Q.Promise<any> {

            return Q.fcall(() => {
                var vm = {};
                vm[this.viewModelName] = this.getViewModelUpdated();

                return vm;
            });
        }

        protected registerSubscriptions(): void {

            this.eventHub.subscribe(`${this.viewModelName}Rendered`, () => {
                //this.formInstances = this.registerFormsForValidation($('form', this.context.container));
            });
        }

        protected getViewModelUpdated(): string {

            var formContext = $('form', this.context.container),
                viewModel = (<ISerializeObjectJqueryPlugin>formContext).serializeObject();

            return JSON.stringify(viewModel);
        }

       // protected isValidForUpdate(): boolean {

            //var isValidForUpdate = _.all(this.formInstances, formInstance => formInstance.validate(undefined, true));

            //return isValidForUpdate;
       // }

        protected onRenderDataFailed(reason: any): void {

            this.removeLoading();
            console.error(`Failed rendering the control in charge of '${this.viewModelName}'.`, reason);
            ErrorHandler.instance().outputErrorFromCode('CheckoutRenderFailed');
        }

        protected removeLoading(): void {

            this.context.container.find('.js-loading').hide();
        }
    }
}
