///<reference path='../../../../Typings/tsd.d.ts' />
///<reference path='../../../Mvc/ComposerClient.ts' />

module Orckestra.Composer {
    'use strict';

    export class ApplePayService {

        public getVueMixin() {
            return {
                mounted() {
                    if ("ApplePaySession" in window) {
                        var { merchantidentifier }= document.getElementById('storeContext').dataset;
                        if(!merchantidentifier) {
                            this.response = 'Apple Pay Merchant Identifier is not configured';
                            return;
                        }
  
                        var promise = ApplePaySession.canMakePaymentsWithActiveCard(merchantidentifier);
            
                        promise.then((canMakePayments) => {
                            this.CanMakeApplePayPayments = canMakePayments;
                            if(!canMakePayments) {
                                this.response = 'Can not make payments with any active card on this device.';
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
                        this.response = 'ApplePay Session is not supported in this browser.';
                    }
                },
                data: {
                    response: '',
                    CanMakeApplePayPayments: false,
                    CurrentApplePaySession: null,
                    AppleApiVersion: 0
                },
                computed: {
                },
                methods: {
                    createApplePaySession() {
                        if (!this.AppleApiVersion) {
                            this.response = 'Supported Apple JS api version is not found on this client.';
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
                        this.response = event.validationURL;

                        ComposerClient.post('/api/applepay/create', { ValidationUrl: event.validationURL })
                            .then(response => {
                                console.log('Apple pay create response: ' + JSON.stringify(response))
                                this.CurrentApplePaySession.completeMerchantValidation(response);
                            }).finally(() => this.Mode.Loading = false);
                    },
                    paymentauthorized(event) {
                        // Send payment for processing...
                        const payment = event.payment;
                        ComposerClient.post('/api/bambora/authorize', {Token: JSON.stringify(payment.token.paymentData), Amount: this.Cart.Payment.Amount })
                            .then(response => {
                               console.log(JSON.stringify(event.payment));
                                if (response.approved === '1') { //approved
                                    this.CurrentApplePaySession.completePayment(ApplePaySession.STATUS_SUCCESS);
                                } else {
                                    this.response = response.approved + ': ' + response.message;
                                    this.CurrentApplePaySession.completePayment(ApplePaySession.STATUS_FAILURE);
                                    this.CurrentApplePaySession.abort();
                                }
                            }).finally(() =>  this.Mode.Loading = false);
                    }
                }
            };
        }

    }
}
