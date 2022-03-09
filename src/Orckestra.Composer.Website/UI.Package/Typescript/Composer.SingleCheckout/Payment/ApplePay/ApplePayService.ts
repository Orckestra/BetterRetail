///<reference path='../../../../Typings/tsd.d.ts' />
///<reference path='../../../Mvc/ComposerClient.ts' />

module Orckestra.Composer {
    'use strict';

    export class ApplePayService {

        public getVueMixin(checkoutService: any) {
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
                    IsApplePayMethod() {
                        //temp solution as payment type = CreditCard
                        return this.ActivePayment && this.ActivePayment.PaymentMethodId === '084dbf29-e00d-4709-ad4e-c8c7562cfefd';
                    }
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

                        ComposerClient.post('/api/cart/tokenizepayment', { Token: JSON.stringify(payment.token.paymentData), PaymentId: this.ActivePayment.Id })
                        .then(() => this.CurrentApplePaySession.completePayment(ApplePaySession.STATUS_SUCCESS))
                        .then(() => checkoutService.completeCheckout())
                        .fail(reason => {
                            this.CurrentApplePaySession.completePayment(ApplePaySession.STATUS_FAILURE);
                            console.error('An error occurred while completing the checkout.', reason);
                            ErrorHandler.instance().outputErrorFromCode('CompleteCheckoutFailed');
                        })
                        .finally(() =>  this.Mode.Loading = false);;

                       /* ComposerClient.post('/api/bambora/authorize', {Token: JSON.stringify(payment.token.paymentData), Amount: this.Cart.Payment.Amount })
                            .then(response => {
                               console.log(JSON.stringify(event.payment));
                                if (response.approved === '1') { //approved
                                    this.CurrentApplePaySession.completePayment(ApplePaySession.STATUS_SUCCESS);
                                } else {
                                    this.response = response.approved + ': ' + response.message;
                                    this.CurrentApplePaySession.completePayment(ApplePaySession.STATUS_FAILURE);
                                    this.CurrentApplePaySession.abort();
                                }
                            }).finally(() =>  this.Mode.Loading = false);*/
                    },
                    testCompleteApplePay() {
                        var payment = { "paymentData": { "data": "pvPQqj9/RQJfZOLfUD4E7D4krfjCdubhQoJxGg002ygNSaKkvQ6NngLIcXWpWE+xD0fuAnx+3QxkOL+i3qGvFSBXiOPzR8Y633ZJe0zv4zHlgLF+sS2jDdM44gFvQiGu35KysOY898vDkiLSXFqQIOrTNOnf1A+4EalBHKwVXRGtDSW4c80NLHC97xFRinMuWEjETweV6nRVpIK4gXMLKX67DANQ3C0/XVcPWzEdOOAsc+3KhsSOX6Z3ajUqFES976vqwAsI+Kp2Q7EWfsSOrLtB/c+btoaaMJzmRtdexUCoAc9JqdEomiNKW9L482FNuJ5g9RAM3Or7GaGweCNB1H3zPVjbHZY9MmwneHGhI+YkIEYYIhUz+X49qWxBiA/oiWisNBF/q3dhK5Cq/JGRkiloxkP0qzpwS3UAYE73Qw==", "signature": "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", "header": { "publicKeyHash": "S6XraCsJhcnnj9SkrjhkcDW/I1yNdp/wi2vyNjypCBE=", "ephemeralPublicKey": "MFkwEwYHKoZIzj0CAQYIKoZIzj0DAQcDQgAEWM3Ts0fCwocWp3C3EAyeUK/yv5Tw5bO7AuY7PjAF2PthVdBBBg/dmrucxQ9fC30xEKdtsFWGanRVJQ36LQDz0A==", "transactionId": "45a5decfa423822ec63e4b5c008aa29b1f7191bb9b1250472f4b3577f61cae9b" }, "version": "EC_v1" }, "paymentMethod": { "displayName": "Visa 0326", "network": "Visa", "type": "debit" }, "transactionIdentifier": "45A5DECFA423822EC63E4B5C008AA29B1F7191BB9B1250472F4B3577F61CAE9B" };
                        ComposerClient.post('/api/cart/tokenizepayment', { Token: JSON.stringify(payment.paymentData), PaymentId: this.ActivePayment.Id })
                            .then(() => checkoutService.completeCheckout())
                            .fail(reason => {
                                console.error('An error occurred while completing the checkout.', reason);
                                ErrorHandler.instance().outputErrorFromCode('CompleteCheckoutFailed');
                            })
                            .fail(reason => console.log(reason));
                    }
                }
            };
        }

    }
}
