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
///<reference path='../Composer.Cart/OrderHistory/Services/OrderService.ts' />
///<reference path='../Composer.Grocery/TimeSlotsHelper.ts' />
///<reference path='../Composer.Grocery/FulfillmentEvents.ts' />
///<reference path='../Composer.Grocery/FulfillmentService.ts' />

module Orckestra.Composer {
    'use strict';

    export class OrderConfirmationController extends Orckestra.Composer.Controller {

        private cacheProvider: ICacheProvider;
        private findOrderService: IFindOrderService;
        private membershipService: IMembershipService;
        private passwordCheckService: PasswordCheckService;
        protected orderService: OrderService;
        protected fulfillmentService: IFulfillmentService = FulfillmentService.instance();

        private orderConfirmationCacheKey = 'orderConfirmationCacheKey';
        private orderCacheKey = 'orderCacheKey';
        public VueCheckoutOrderConfirmation: Vue;

        public initialize() {
            let self: OrderConfirmationController = this;
            var culture: string = $('html').attr('lang');
            super.initialize();
            this.cacheProvider = CacheProvider.instance();
            this.findOrderService = new FindOrderService(this.eventHub);
            this.orderService = new OrderService();
            this.membershipService = new MembershipService(new MembershipRepository());
            let form = this.context.container.find('form');
            this.passwordCheckService = new PasswordCheckService({
                passwordPattern: RegExp(form.data('password-pattern')),
                minimumLength: form.data('password-length')
            });

            this.fulfillmentService.invalidateCache();

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
                            OrderDetails: null,
                            OrderSummary: null,
                            Fulfillment: null,
                            TimeslotInfo: null,
                            IsUpdatedOrder: result.IsUpdatedOrder ? result.IsUpdatedOrder : false,
                            ...result
                        },
                        mounted() {
                            self.findUserAsync(result.CustomerEmail).then(isExist => {
                                this.IsUserExist = isExist;
                            });

                            self.IsAuthenticated().then(isAuthenticated => {
                                this.IsAuthenticated = isAuthenticated;
                            });

                            let request = this.IsUserExist && this.IsAuthenticated ? self.orderService.getOrderByNumber(result.OrderNumber) :
                                self.orderService.getGuestOrderByNumber(result.OrderNumber, result.CustomerEmail);

                            request.then(orderData => {
                                this.OrderDetails = orderData;
                                this.OrderSummary = orderData.OrderSummary;
                                this.TimeslotInfo = TimeSlotsHelper.getTimeSlotLocalizations(orderData.Shipments[0].TimeSlotReservation.ReservationDate,
                                    orderData.Shipments[0].TimeSlot.SlotBeginTime, orderData.Shipments[0].TimeSlot.SlotEndTime, culture);
                            });

                            self.fulfillmentService.getSelectedFulfillment()
                                .then(fulfillment => {
                                    this.Fulfillment = fulfillment;
                                    return self.fulfillmentService.restoreFulfillment(this.IsUpdatedOrder)
                                })
                                .then(() => self.fulfillmentService.getSelectedFulfillment())
                                .then(fulfillment => {
                                    self.eventHub.publish(FulfillmentEvents.StoreSelected, { data: fulfillment.Store });
                                    self.eventHub.publish(FulfillmentEvents.TimeSlotSelected, {
                                        data: { TimeSlot: fulfillment.TimeSlot, TimeSlotReservation: fulfillment.TimeSlotReservation }
                                    });
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
            let formData = { FirstName, LastName, Email, Password };
            return this.membershipService.register(formData, null).then(result => {
                this.eventHub.publish(MyAccountEvents[MyAccountEvents.AccountCreated], { data: result });
                if (result.Status === MyAccountStatus[MyAccountStatus.Success]) {
                    this.eventHub.publish(MyAccountEvents[MyAccountEvents.LoggedIn], { data: result });
                }

                return result;
            }).fail(({ Errors: [error] }) => {
                ErrorHandler.instance().outputErrorFromCode(error.ErrorCode);
                throw error;
            });
        }

        private IsAuthenticated(): Q.Promise<boolean> {
            return this.membershipService.isAuthenticated().then(result => result.IsAuthenticated);
        }
    }
}
