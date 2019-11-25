///<reference path='../../../../../../Composer.UI/Source/Typings/tsd.d.ts' />
///<reference path='../../../Typescript/BaseCheckoutController.ts' />

module Orckestra.Composer.Mocks {

    export class SecondMockCheckoutController extends Orckestra.Composer.BaseCheckoutController {

        public initialize() {

            this.viewModelName = 'SecondMockCheckout';

            super.initialize();
        }

        public renderData(checkoutContext: ICheckoutContext): Q.Promise<any> {

            //Render some datas...

            return Q(null);
        }

        protected getViewModelUpdated(): string {

            return 'FirstName: "toto", LastName: "tata"';
        }

        protected isValidForUpdate(): boolean {

            return false;
        }
    }
}
