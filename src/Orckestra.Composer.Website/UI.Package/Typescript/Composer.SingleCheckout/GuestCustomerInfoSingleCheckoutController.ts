///<reference path='../..//Typings/tsd.d.ts' />
///<reference path='./BaseSingleCheckoutController.ts' />

module Orckestra.Composer {
    'use strict';

    export class GuestCustomerInfoSingleCheckoutController extends Orckestra.Composer.BaseSingleCheckoutController {

        public initialize() {
            let self: GuestCustomerInfoSingleCheckoutController = this;
            self.viewModelName = 'GuestCustomerInfo';
            self.formSelector = '#editCustomerForms';

            super.initialize();

            let vueUserMixin = {
                data: {
                    CheckedEmailAddress: {
                        Email: '',
                        IsExist: false
                    }
                },
                created() {
                    this.customerBeforeEdit = { ...this.Cart.Customer };
                },
                mounted() {
                    this.initializeParsey(self.formSelector);
                },
                computed: {
                    FulfilledCustomer() {
                        let { Email, Password } = this.Cart.Customer;
                        let fulfilledSignIn = Email && Password;
                        let fulfilled = self.checkoutService.customerFulfilled(this.Cart);
                        return !!(this.Mode.SignIn === SignInModes.SigningIn ? fulfilledSignIn : fulfilled);
                    },
                    BaseInformationMode() {
                        return this.Mode.SignIn === SignInModes.Base;
                    },
                    UserExistsMode() {
                        return this.Mode.SignIn === SignInModes.UserExists;
                    },
                    SigningInMode() {
                        return this.Mode.SignIn === SignInModes.SigningIn;
                    },
                    ShowSignInButton() {
                        return this.FulfilledShipping && this.SigningInMode;
                    }
                },
                methods: {
                    prepareCustomer() {
                        this.initializeParsey(self.formSelector);
                    },
                    processCustomer(): Q.Promise<boolean> {

                        if (this.ShowSignInButton) {
                            this.Mode.SignIn = SignInModes.Base;
                            this.Cart.Customer = { ...this.customerBeforeEdit };
                            return Q.resolve(true);
                        }

                        let isValid = this.validateParsey(self.formSelector);
                        if (!isValid) {
                            return Q.reject('User information is not valid');
                        }

                        if (!this.IsAuthenticated) {
                            switch (this.Mode.SignIn) {
                                case SignInModes.Base:
                                    return this.checkUserExist(this.Cart.Customer.Email).then(result => {
                                        if (result) { return !result; }
                                        return this.updateCustomer();
                                    });
                                case SignInModes.SigningIn:
                                    let {Email: Username, Password} = this.Cart.Customer;
                                    let loginData = {Username, Password};
                                    return self.checkoutService.loginUser(loginData);
                            }
                        }

                        return this.updateCustomer();
                    },

                    updateCustomer(): Q.Promise<boolean> {
                        if (!this.isCustomerModified()) {
                            return Q.resolve(true);
                        }

                        this.Steps.Information.Loading = true;
                        return self.checkoutService.updateCart([self.viewModelName])
                            .then(() => true).finally(() => this.Steps.Information.Loading = false);
                    },

                    isCustomerModified() {
                        let keys = _.keys(this.Cart.Customer);
                        let isModified = _.some(keys, (key) => this.customerBeforeEdit[key] !== this.Cart.Customer[key]);
                        return isModified;
                    },
                    signInButton() {
                        this.resetParsley(self.formSelector);
                        this.Mode.SignIn = SignInModes.SigningIn;
                    },
                    signInAndContinue() {
                        let { Email: Username, Password } = this.Cart.Customer;
                        let loginData = { Username, Password };
                        self.checkoutService.loginUser(loginData)
                            .then((success) => {
                                if (success) {
                                    this.$children[0].navigateToStep(CheckoutStepNumbers.Shipping);
                                }
                            });
                    },
                    continueAsGuestButton() {
                        this.resetParsley(self.formSelector);
                        this.Mode.SignIn = SignInModes.Base;
                        this.Errors.SignIn = false;
                        this.checkUserExist(this.Cart.Customer.Email);
                    },
                    onChangeUsername(e) {
                        if (this.UserExistsMode) {
                            this.Mode.SignIn = SignInModes.Base;
                        }
                    },
                    checkUserExist(email: string): Q.Promise<boolean> {
                        if (this.CheckedEmailAddress.Email === email) {
                            this.Mode.SignIn = this.CheckedEmailAddress.IsExist ? SignInModes.UserExists : SignInModes.Base;
                            return Q.resolve(this.CheckedEmailAddress.IsExist);
                        }

                        return self.checkoutService.checkUserExist(email)
                            .then(result => {
                                this.Mode.SignIn = result ? SignInModes.UserExists : SignInModes.Base;
                                this.CheckedEmailAddress = { Email: email, IsExist: result };
                                return result;
                            });
                    }
                }
            };

            this.checkoutService.VueCheckoutMixins.push(vueUserMixin);
        }

        public getViewModelNameForUpdatePromise(): Q.Promise<any> {
            return Q.fcall(() => {
                var vueData = this.checkoutService.VueCheckout;
                let isValid = vueData.validateParsey(this.formSelector);
                if (!isValid) {
                    return Q.reject('User information is not valid');
                }

                if (vueData.isCustomerModified()) {
                    return this.viewModelName;
                }
            });
        }

        public getUpdateModelPromise(): Q.Promise<any> {
            return Q.fcall(() => {
                let vueCustomerData = this.checkoutService.VueCheckout.Cart.Customer;
                return {[this.viewModelName]: JSON.stringify(vueCustomerData)};
            });
        }
    }
}
