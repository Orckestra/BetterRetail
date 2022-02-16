///<reference path='../../../../Typings/tsd.d.ts' />
///<reference path='../../../Mvc/ComposerClient.ts' />

module Orckestra.Composer {
    'use strict';

    export class ApplePayService {

        public getVueMixin() {
            return {
                mounted() {
                    if ("ApplePaySession" in window) {
                        var merchantIdentifier = 'merchant.wfecm.int.platform.orckestra.cloud';
                        var promise = ApplePaySession.canMakePaymentsWithActiveCard(merchantIdentifier);
            
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
                        this.response = 'ApplePay session is not supported in this browser.';
                    }
                },
                data: {
                    response: '',
                    CanMakeApplePayPayments: false,
                    CurrentApplePaySession: null,
                    loading: false,
                    AppleApiVersion: 0
                },
                computed: {
                },
                methods: {
                    createApplePaySession() {
                        if (!this.AppleApiVersion) {
                            this.response = 'not supported apple api version found on this client';
                            return;
                        };
            
                        this.loading = true;
                        var request = {
                            countryCode: 'CA',
                            currencyCode: 'USD',
                            supportedNetworks: ['visa', 'masterCard', 'amex', 'discover'],
                            merchantCapabilities: ['supports3DS'],
                            total: { label: 'Better Retail Order', amount: '1.00' },
                        }
            
                        this.loading = true;
                        this.CurrentApplePaySession = new ApplePaySession(this.AppleApiVersion, request);
                        this.CurrentApplePaySession.onvalidatemerchant = (event) => this.validateMerchant(event);
                        this.CurrentApplePaySession.onpaymentauthorized = (event) => this.paymentauthorized(event);

                        this.CurrentApplePaySession.begin();
                    },
                    validateMerchant(event) {
                        var url = event && event.validationURL ? event.validationURL : 'https://apple-pay-gateway-cert.apple.com/paymentservices/startSession';

                        this.response = url;

                        ComposerClient.post('/api/applepay/create', { ValidationUrl: url })
                            .then(response => {
                                this.response = response;
                                this.loading = false;
                                this.CurrentApplePaySession.completeMerchantValidation(response);
                               
                            })
                        //https://developer.apple.com/documentation/apple_pay_on_the_web/apple_pay_js_api/requesting_an_apple_pay_payment_session

                    },
                    paymentauthorized(event) {
                        // Send payment for processing...
                        const payment = event.payment;  
                        this.response = payment.token;
                        this.CurrentApplePaySession.completePayment(ApplePaySession.STATUS_SUCCESS);
                        /*ComposerClient.post('/api/bambora/authorize', { Token: payment.token })
                        .then(response => {
                            this.response = response;
                            this.loading = false;
                            this.CurrentApplePaySession.completePayment(ApplePaySession.STATUS_SUCCESS);
                        })*/
                    }
                }
            };
            
        }
    }

}