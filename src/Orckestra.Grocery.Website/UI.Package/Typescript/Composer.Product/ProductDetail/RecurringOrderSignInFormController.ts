///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Composer.MyAccount/ReturningCustomer/ReturningCustomerController.ts' />

module Orckestra.Composer {

    export class RecurringOrderSignInFormController extends ReturningCustomerController {

        protected returnUrl: string;
        public initialize() {
            super.initialize();

            this.returnUrl = this.context.container.data('product-url');
        }

        protected loginImpl(actionContext: IControllerActionContext): Q.Promise<any> {
            let formData: any = (<ISerializeObjectJqueryPlugin>actionContext.elementContext).serializeObject();

            return this.membershipService.login(formData, this.returnUrl);
        }
    }
}
