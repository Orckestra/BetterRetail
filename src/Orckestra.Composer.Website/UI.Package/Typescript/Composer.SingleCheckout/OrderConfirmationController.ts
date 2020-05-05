///<reference path='../../Typings/tsd.d.ts' />
///<reference path='../../Typings/vue/index.d.ts' />
///<reference path='../Composer.Cart/FindMyOrder/IFindOrderService.ts' />
///<reference path='../Composer.Cart/FindMyOrder/FindOrderService.ts' />
///<reference path='../Cache/CacheProvider.ts' />
///<reference path='../Repositories/ICartRepository.ts' />
///<reference path='../Composer.MyAccount/Common/IMembershipService.ts' />
///<reference path='../Composer.MyAccount/Common/MembershipService.ts' />
///<reference path='../Composer.MyAccount/Common/MyAccountEvents.ts' />
///<reference path='../Composer.MyAccount/Common/MyAccountStatus.ts' />
///<reference path='../Utils/PasswordCheckService.ts' />
///<reference path='../ErrorHandling/ErrorHandler.ts' />

module Orckestra.Composer {
    'use strict';

    export class OrderConfirmationController extends Orckestra.Composer.Controller {

        private cacheProvider: ICacheProvider;
        private findOrderService: IFindOrderService;
        private membershipService: IMembershipService;
        private passwordCheckService: PasswordCheckService;

        private orderConfirmationCacheKey = 'orderConfirmationCacheKey';
        private orderCacheKey = 'orderCacheKey';
        public VueCheckoutOrderConfirmation: Vue;

        public initialize() {
            let self: OrderConfirmationController = this;
            super.initialize();
            this.cacheProvider = CacheProvider.instance();
            this.findOrderService = new FindOrderService(this.eventHub);
            this.membershipService = new MembershipService(new MembershipRepository());
            this.passwordCheckService = new PasswordCheckService({
                passwordPattern: RegExp(this.context.container.data('password-pattern')),
                minimumLength: this.context.container.data('password-length')
            });

            this.cacheProvider.defaultCache.get<any>(this.orderCacheKey)
                .then((result: ICompleteCheckoutResult) => {

                    this.eventHub.publish('CheckoutConfirmation', { data: result });
                    this.cacheProvider.defaultCache.clear(this.orderCacheKey).done();
                })
                .fail((reason: any) => {

                    console.error('Unable to retrieve order number from cache, attempt to redirect.');
                });

            this.cacheProvider.defaultCache.get<any>(this.orderConfirmationCacheKey)
                .then((result: ICompleteCheckoutResult) => {
                    if (!result) {
                        console.error('Order was placed but it is not possible to retrieve order number from cache.');
                        return;
                    }

                    this.VueCheckoutOrderConfirmation = new Vue({
                        el: '#vueCheckoutOrderConfirmation',
                        data: {
                            Password: null,
                            IsUserExist: true,
                            IsLoading: false,
                            IsAuthenticated: false,
                            PasswordStrength: '',
                            ShowPassword: false,
                            ...result
                        },
                        mounted() {
                            self.findUserAsync(result.CustomerEmail).then(isExist => {
                                this.IsUserExist = isExist;
                            });

                            self.IsAuthenticated().then(isAuthenticated => {
                                this.IsAuthenticated = isAuthenticated;
                            });
                        },
                        computed: {
                            ShowCreateAccountForm() {
                                return !this.IsUserExist && !this.IsAuthenticated
                            }
                        },
                        methods: {
                            getCreateAccountForm(): JQuery {
                                return $("#formCreateAccount");
                            },
                            findMyOrder() {
                                let findMyOrderRequest = {
                                    OrderNumber: this.OrderNumber,
                                    Email: this.CustomerEmail
                                };
                                self.findOrderAsync(findMyOrderRequest).then(result => {
                                    window.location.href = result.Url;
                                });
                            },
                            createAccount() {
                                let parsleyInit = this.getCreateAccountForm().parsley();
                                if (parsleyInit && !parsleyInit.validate()) { return; }

                                this.IsLoading = true;
                                self.createCustomer(this.CustomerFirstName, this.CustomerLastName, this.CustomerEmail, this.Password)
                                    .then(result => self.findOrderService.addOrderToCurrentUser(this.OrderNumber)
                                        .then(() => {
                                            if (result.ReturnUrl) {
                                                window.location.replace(decodeURIComponent(result.ReturnUrl));
                                            }
                                        })
                                    )
                                    .finally(() => {
                                        this.IsLoading = false;
                                    });
                            },
                            onChangePassword(e: any) {
                                let { value } = e.target;
                                this.PasswordStrength = self.passwordCheckService.checkPasswordStrength(value);
                            },
                            showPasswordToggle() {
                                this.ShowPassword = !this.ShowPassword;
                            }
                        }
                    });

                    this.eventHub.publish('checkoutStepRendered', {
                        data: { StepNumber: 'confirmation' }
                    });

                    this.cacheProvider.defaultCache.clear(this.orderConfirmationCacheKey).done();
                })
                .fail((reason: any) => {

                    console.error('Unable to retrieve order number from cache, attempt to redirect.');

                    let redirectUrl: string = this.context.container.data('redirecturl');

                    if (redirectUrl) {
                        window.location.href = redirectUrl;
                    } else {
                        console.error('Redirect url was not detected.');
                    }
                });
        }

        private findOrderAsync(request: IGetOrderDetailsUrlRequest): Q.Promise<IGuestOrderDetailsViewModel> {
            return this.findOrderService.getOrderDetailsUrl(request);
        }

        private findUserAsync(email: string): Q.Promise<boolean> {
            return this.membershipService.isUserExist(email)
                .then(result => result.IsExist);
        }

        private createCustomer(FirstName: string, LastName: string, Email: string, Password: string): Q.Promise<any> {
            let formData = {FirstName, LastName, Email, Password};
            return this.membershipService.register(formData, null).then(result => {
                this.eventHub.publish(MyAccountEvents[MyAccountEvents.AccountCreated], { data: result });
                if (result.Status === MyAccountStatus[MyAccountStatus.Success]) {
                    this.eventHub.publish(MyAccountEvents[MyAccountEvents.LoggedIn], { data: result });
                }

               return result;
            }).fail(({Errors: [error]}) => {
                ErrorHandler.instance().outputErrorFromCode(error.ErrorCode);
                throw error;
            });
        }

        private IsAuthenticated(): Q.Promise<boolean> {
            return this.membershipService.isAuthenticated().then(result => result.IsAuthenticated);
        }
    }
}
