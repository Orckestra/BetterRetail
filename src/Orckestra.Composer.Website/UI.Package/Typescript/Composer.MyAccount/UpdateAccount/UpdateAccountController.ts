///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Mvc/Controller.ts' />
///<reference path='../../Mvc/IControllerActionContext.ts' />
///<reference path='../../Utils/UrlHelper.ts' />
///<reference path='../Common/CustomerService.ts' />
///<reference path='../../Services/UserMetadataService.ts' />
///<reference path='../Common/MyAccountEvents.ts' />
///<reference path='../MyAccount/MyAccountController.ts' />

module Orckestra.Composer {
    enum UpdateAccountStates {
        Sucsess = 'sucsess',
        Failed = 'failed'
    }

    export class UpdateAccountController extends Orckestra.Composer.MyAccountController {

        protected customerService: ICustomerService = new CustomerService(new CustomerRepository());
        protected userService: UserMetadataService = new UserMetadataService(new MembershipRepository());
        protected VueUpdateAccount: Vue;

        public initialize() {

            super.initialize();
            this.registerSubscriptions();
            let userData = this.context.viewModel;
            const self = this;
            const updateAccountFormId = "#updateAccountForm";

            this.VueUpdateAccount = new Vue({
                el: '#vueUpdateAccount',

                data: {
                    ...userData,
                    UpdateAccountState: "",
                    IsLoading: false,
                    Errors: []
                },
                mounted() {
                    self.initializeParsey(updateAccountFormId);
                },
                computed: {
                    SaveChangesFailed() {
                        return this.UpdateAccountState === UpdateAccountStates.Failed;
                    },
                    SaveChangesSucceeded() {
                        return this.UpdateAccountState === UpdateAccountStates.Sucsess;
                    }
                },
                methods: {
                    enableSubmitButton() {
                        this.UpdateAccountState = "";
                        let parsleyInit = self.getParsleyInit(updateAccountFormId);
                        $('#UpdateAccountSubmit').prop('disabled', !parsleyInit.isValid());
                    },
                    updateAccount() {
                        let parsleyInit = self.getParsleyInit(updateAccountFormId);
                        if (parsleyInit && !parsleyInit.validate()) { return; }

                        var formData: any = {
                            FirstName: this.FirstName,
                            Username: this.Username,
                            LastName: this.LastName,
                            Email: this.Email,
                            PreferredLanguage: this.Language
                        };

                        var returnUrlQueryString: string = 'ReturnUrl=';
                        var returnUrl: string = '';

                        if (window.location.href.indexOf(returnUrlQueryString) > -1) {
                            returnUrl = urlHelper.getURLParameter(location.search, 'ReturnUrl');
                        }

                        this.IsLoading = true;
                        self.customerService.updateAccount(formData, returnUrl)
                            .then((result) => {
                                self.userService.invalidateCache();
                                this.UpdateAccountState = UpdateAccountStates.Sucsess;
                                return result;
                            })
                            .then(result => self.onUpdateAccountFulfilled(result))
                            .fail((reason) => {
                                console.error('Error updating the account.', reason);
                                if(reason && reason.Errors) {
                                    this.Errors = reason.Errors;
                                }
                                this.UpdateAccountState = UpdateAccountStates.Failed;

                            })
                            .fin(() => this.IsLoading = false);
                    }
                }
            });
        }

        protected registerSubscriptions() {
            this.eventHub.subscribe(MyAccountEvents[MyAccountEvents.AccountUpdated], e => this.onAccountUpdated(e));
        }

        private initializeParsey(formId: any): void {
            $(formId).parsley({ trigger: 'focusout change' });
        };

        private getParsleyInit(formId: any): IParsley {
            return $(formId).parsley();
        }

        private onAccountUpdated(result: any): void {
            if (result.ReturnUrl) {
                window.location.replace(decodeURIComponent(result.ReturnUrl));
            }
        }

        protected onUpdateAccountFulfilled(result: any): Q.Promise<any> {

            this.eventHub.publish(MyAccountEvents[MyAccountEvents.AccountUpdated], { data: result });

            return Q(result);
        }
    }
}
