///<reference path='../../../../../../Composer.UI/Source/Typings/tsd.d.ts' />
///<reference path='../../../Typescript/BaseCheckoutController.ts' />

module Orckestra.Composer.Mocks {

    export class FirstMockCheckoutController extends Orckestra.Composer.BaseCheckoutController {

        public initialize() {

            this.viewModelName = 'FirstMockCheckout';

            super.initialize();
        }
    }
}
