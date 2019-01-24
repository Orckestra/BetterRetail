///<reference path='../../../../Composer.UI/Source/Typings/tsd.d.ts' />
///<reference path='../../../CheckoutCommon/source/Typescript/BaseCheckoutController.ts' />

module Orckestra.Composer {
    'use strict';

    export class CheckoutOrderSummaryController extends Orckestra.Composer.BaseCheckoutController {

        public initialize() {

            this.viewModelName = 'CheckoutOrderSummary';

            super.initialize();

            this.registerSubscriptions();

            CheckoutService.checkoutStep = this.context.viewModel.CurrentStep;
        }

        protected registerSubscriptions() {

            super.registerSubscriptions();

            let handle: number;

            this.eventHub.subscribe('cartUpdating', () => {

                clearTimeout(handle);
                handle = window.setTimeout(() => this.renderLoading(), 300);
            });

            this.eventHub.subscribe('cartUpdatingFailed', () => {

                clearTimeout(handle);
                this.reRender();
            });

            this.eventHub.subscribe('cartUpdated', e => {

                clearTimeout(handle);
                e.data.LoadCheckoutOrderSummary = true;
                this.render(this.viewModelName, e.data);
            });
        }

        public renderData(checkoutContext: ICheckoutContext): Q.Promise<void> {

            return Q.fcall(() => {
                checkoutContext.cartViewModel.LoadCheckoutOrderSummary = true;
                this.render(this.viewModelName, checkoutContext.cartViewModel);
            });
        }

        private reRender(): void {

            this.renderLoading();

            this.checkoutService.getCart()
                .then(cartVm => {
                    cartVm.LoadCheckoutOrderSummary = true;
                    this.render(this.viewModelName, cartVm);
                })
                .fail((reason: any) => this.onRenderDataFailed(reason));
        }

        private renderLoading(): void {

            return this.render(this.viewModelName, { LoadCheckoutOrderSummary: true, GettingCart: true });
        }

        /**
         *  Update the cart.
         *  If errors are returned, it stay in the same page.
         *  If there is no errors it moves to the next step.
         */
        public nextStep(actionContext: IControllerActionContext) {

            let busy: UIBusyHandle = this.asyncBusy({elementContext: actionContext.elementContext});

            ErrorHandler.instance().removeErrors();

            this.checkoutService.updateCart()
                .then(result => {

                    if (result.HasErrors === false) {
                        window.location.href = result.NextStepUrl;
                    } else {
                        console.error('Error while updating the cart');
                        busy.done();
                    }
                })
                .fail(reason => {
                    console.error('Error on checkout submit.', reason);
                    ErrorHandler.instance().outputErrorFromCode('CheckoutNextStepFailed');
                    busy.done();
                });
        }
    }
}
