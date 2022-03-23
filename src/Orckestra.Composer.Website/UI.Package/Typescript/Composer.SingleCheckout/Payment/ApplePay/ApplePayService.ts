///<reference path='../../../../Typings/tsd.d.ts' />
///<reference path='../../../Mvc/ComposerClient.ts' />

module Orckestra.Composer {
    'use strict';

    export class ApplePayService {

        public getVueMixin(checkoutService: any) {
            return {
                mounted() {
                    if ("ApplePaySession" in window) {
                        var { merchantidentifier, paymentmethodid } = document.getElementById('storeContext').dataset;
                        if (!merchantidentifier) {
                            console.log('Apple Pay Merchant Identifier is not configured');
                            return;
                        }

                        this.PaymentMethodId = paymentmethodid;
  
                        var promise = ApplePaySession.canMakePaymentsWithActiveCard(merchantidentifier);
            
                        promise.then((canMakePayments) => {
                            this.CanMakeApplePayPayments = canMakePayments;
                            if(!canMakePayments) {
                                console.log('Can not make payments with any active card on this device. Checkout device Wallet.');
                            }
                        });
            
                        let version = 12;
                        let supported = false;
                        while (version > 4 && !supported) {
                             if (ApplePaySession.supportsVersion(version)) {
                                this.AppleApiVersion = version;
                                supported = true;
                            }
                            version--;
                        }
            
                    } else {
                        console.log( 'ApplePay Session is not supported in this browser.');
                    }
                },
                data: {
                    CanMakeApplePayPayments: false,
                    PaymentMethodId: null,
                    CurrentApplePaySession: null,
                    AppleApiVersion: 0
                },
                computed: {
                    IsApplePayMethod() {
                        return this.ActivePayment && this.ActivePayment.PaymentMethodId === this.PaymentMethodId;
                    }
                },
                methods: {
                    createApplePaySession() {
                        if (!this.AppleApiVersion) {
                            console.log('Any supported Apple JS api version is not found on this client.');
                            return;
                        };

                        this.Mode.Loading = true;

                        var { storename, currency, country} = document.getElementById('storeContext').dataset;
                        var { PostalCode, CountryCode, PhoneNumber, FirstName, LastName } = this.Cart.Payment.BillingAddress;
                        var total = this.Cart.Payment.Amount;

                        var billingContact = {
                            phoneNumber: PhoneNumber,
                            givenName: FirstName,
                            familyName: LastName,
                            postalCode: PostalCode,
                            countryCode: CountryCode
                        }

                        var request: any = {
                            countryCode: country,
                            currencyCode: currency,
                            supportedNetworks: ['visa', 'masterCard', 'amex', 'discover'],
                            merchantCapabilities: ['supports3DS'],
                            total: { label: storename, amount: total },
                            billingContact: billingContact
                        }
            
                        this.CurrentApplePaySession = new ApplePaySession(this.AppleApiVersion, request);
                        this.CurrentApplePaySession.onvalidatemerchant = (event) => this.validateMerchant(event);
                        this.CurrentApplePaySession.onpaymentauthorized = (event) => this.paymentauthorized(event);

                        this.CurrentApplePaySession.begin();
                    },
                    validateMerchant(event) {
                        console.log('validateMerchant is started with url ' + event.validationURL);
                        ComposerClient.post('/api/applepay/create', { ValidationUrl: event.validationURL })
                            .then(response => {
                                console.log('Apple pay create session response: ' + JSON.stringify(response))
                                this.CurrentApplePaySession.completeMerchantValidation(response);
                            }).finally(() => this.Mode.Loading = false);
                    },
                    paymentauthorized(event) {
                        // Send payment for processing...
                        const payment = event.payment;
                        console.log('paymentauthorized is started...');
                        ComposerClient.post('/api/cart/tokenizepayment', { Token: JSON.stringify(payment.token.paymentData), PaymentId: this.ActivePayment.Id })
                        .then(() => this.CurrentApplePaySession.completePayment(ApplePaySession.STATUS_SUCCESS))
                        .then(() => checkoutService.completeCheckout())
                        .fail(reason => {
                            this.CurrentApplePaySession.completePayment(ApplePaySession.STATUS_FAILURE);
                            console.error('An error occurred while completing the checkout.', reason);
                            ErrorHandler.instance().outputErrorFromCode('CompleteCheckoutFailed');
                        })
                        .finally(() =>  this.Mode.Loading = false);
                    }
                }
            };
        }
    }
}
