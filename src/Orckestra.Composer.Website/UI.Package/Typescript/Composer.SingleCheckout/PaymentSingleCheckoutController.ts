///<reference path='../..//Typings/tsd.d.ts' />
///<reference path='./BaseSingleCheckoutController.ts' />

module Orckestra.Composer {
    'use strict';

    export class PaymentSingleCheckoutController extends Orckestra.Composer.BaseSingleCheckoutController {
        protected activePaymentProvider: BaseCheckoutPaymentProvider;

        public initialize() {
            super.initialize();
            let self: PaymentSingleCheckoutController = this;
            self.viewModelName = 'Payment';
            const SAVED_CREDIT_CARD = 'SavedCreditCard';

            this.eventHub.subscribe('cartBillingAddressUpdated', this.onBillingAddressUpdated);

            let vuePaymentMixin = {
                mounted() {
                    if (this.FulfilledBillingAddress) {
                        this.preparePayment();
                    }
                },
                computed: {
                    FulfilledPayment() {
                        return !!(this.ActivePayment && this.Steps.Billing.EnteredOnce);
                    },
                    MainPaymentMethods() {
                        return this.Payment.PaymentMethods.filter(method => !method.IsCreditCardPaymentMethod);
                    },
                    HasCreditCardProvider() {
                        return this.Payment.PaymentMethods.find(m => m.IsCreditCardPaymentMethod);
                    },
                    SavedCreditCardMethods() {
                        return this.Payment.PaymentMethods.filter(method =>
                            method.IsCreditCardPaymentMethod && method.PaymentType === SAVED_CREDIT_CARD
                        );
                    },
                    NewCreditCardMethod() {
                        return this.Payment.PaymentMethods.find(method =>
                            method.IsCreditCardPaymentMethod && method.PaymentType !== SAVED_CREDIT_CARD
                        );
                    },
                    PaymentProviderNames() {
                        return this.Payment.PaymentProviders.map(p => p.ProviderName);
                    },
                    ActivePayment() {
                        return this.Payment ? this.Payment.ActivePaymentViewModel : null;
                    },
                    CreditCardTrustImage() {
                        return this.Payment.CreditCardTrustImage;
                    },
                    SelectedPaymentMethod() {
                        return this.Payment.PaymentMethods.find(m => m.IsSelected);
                    },
                    IsSavedCreditCardSelected() {
                        return this.ActivePayment && this.ActivePayment.PaymentMethodType === SAVED_CREDIT_CARD;
                    },
                    IsCreditCardProviderSelect() {
                        return this.SelectedPaymentMethod && this.SelectedPaymentMethod.IsCreditCardPaymentMethod;
                    },
                    IsCreditCardProviderSelected() {
                        return this.ActivePayment && this.HasCreditCardProvider &&
                            this.ActivePayment.ProviderType === this.HasCreditCardProvider.PaymentProviderType;
                    },
                    Providers(): Array<BaseCheckoutPaymentProvider> {
                        return self.checkoutService.getPaymentProviders(this.Payment.PaymentProviders);
                    }
                },
                methods: {
                    selectNewCreditCardPaymentMethod() {
                        this.changePaymentMethodProcess(this.ActivePayment.Id, this.NewCreditCardMethod, this.PaymentProviderNames);
                    },
                    changeCardPaymentMethod(e: any) {
                        const { value } = e.target;
                        let selectedMethod = this.SavedCreditCardMethods.find(method => method.Id === value);

                        this.changePaymentMethodProcess(this.ActivePayment.Id, selectedMethod, this.PaymentProviderNames);
                    },
                    changePaymentMethod(e: any) {
                        const { value } = e.target;
                        let selectedMethod = this.MainPaymentMethods.find(method => method.PaymentType === value)
                            || this.SavedCreditCardMethods.find(m => m.Default)
                            || this.NewCreditCardMethod;

                        this.changePaymentMethodProcess(this.ActivePayment.Id, selectedMethod, this.PaymentProviderNames);
                    },
                    changePaymentMethodProcess(paymentId: string, paymentMethodEntity: any, providers: Array<string>) {
                        let oldPayment = this.SelectedPaymentMethod;
                        this.selectPaymentMethod(paymentMethodEntity.Id);

                        self.checkoutService.updatePaymentMethod({
                            PaymentId: paymentId,
                            PaymentProviderName: paymentMethodEntity.PaymentProviderName,
                            PaymentMethodId: paymentMethodEntity.Id,
                            PaymentType: paymentMethodEntity.PaymentType,
                            Providers: providers
                        }).then((result) => {
                            ErrorHandler.instance().removeErrors();
                            this.Payment.ActivePaymentViewModel = result;
                        }).fail((reason) => {
                            console.error('Error while changing the payment method.', reason);
                            ErrorHandler.instance().outputErrorFromCode('PaymentMethodChangeFailed');
                            this.selectPaymentMethod(oldPayment.Id);
                        });
                    },

                    selectPaymentMethod(paymentId: string) {
                        this.Payment.PaymentMethods.forEach(method => method.IsSelected = method.Id === paymentId);
                    },

                    findActivePaymentProvider(): BaseCheckoutPaymentProvider {
                        const { ProviderType } = this.Payment.ActivePaymentViewModel;
                        return self.activePaymentProvider = this.Providers.find(provider => provider.providerType === ProviderType);
                    },

                    processPayment(): Q.Promise<boolean> {
                        let activeProvider = this.findActivePaymentProvider();

                        return activeProvider.validatePayment(this.Payment.ActivePaymentViewModel)
                            .then(success => {
                                if (!success) { return Q.reject('Card information not valid'); }
                                return true;
                            });
                    },

                    submitPayment(): Q.Promise<any> {
                       return this.processPayment()
                        .then(() => {
                            console.log('Committing payment information.');
                            return self.activePaymentProvider.submitPayment(this.Payment.ActivePaymentViewModel);
                        });
                    },

                    preparePayment(): Q.Promise<boolean> {
                        if (!this.Payment) {
                            this.Steps.Payment.Loading = true;
                            return self.checkoutService.getPaymentCheckout()
                                .then(paymentVm => {
                                    this.Payment = paymentVm;
                                    this.Steps.Payment.Loading = false;
                                    return true;
                                });
                        } else {
                            return Q.resolve(true);
                        }
                    }
                }
            };

            this.checkoutService.VueCheckoutMixins.push(vuePaymentMixin);
        }

        protected onBillingAddressUpdated(e) {
            let vueData = e.data;
            vueData.preparePayment();
        }
    }
}
