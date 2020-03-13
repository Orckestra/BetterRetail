///<reference path='../..//Typings/tsd.d.ts' />
///<reference path='./BaseSingleCheckoutController.ts' />

module Orckestra.Composer {
    'use strict';

    export class ShippingAddressSingleCheckoutController extends Orckestra.Composer.BaseSingleCheckoutController {

        public initialize() {
            super.initialize();
            var self: ShippingAddressSingleCheckoutController = this;
            self.viewModelName = 'ShippingAddress';

            let vueShippingAddressMixin = {
                data: {
                    ComplementaryAddressAddState: false,
                    PostalCodeError: false
                },
                created()  {
                    this.adressBeforeEdit = { ...this.Cart.ShippingAddress };
                    this.ComplementaryAddressAddState = !this.Cart.ShippingAddress.Line2;
                },
                mounted() {
                 
                },
                computed: {
                },
                methods: {
                    processShippingAddress() {

                        if (this.IsShippingMethodType) {
                          
                            if (!this.IsAuthenticated) {
                                return this.processGuestShippingAddress();
                            } else {
                                return this.processRegisteredShippingAddress();
                            }
                        }

                        return true;
                    },
                    processGuestShippingAddress() {
                        var processShipping: Q.Deferred<boolean> = Q.defer<boolean>();
                        let isValid = this.initializeParsey('#addressForm');
                        if (isValid) {
                            if (this.isAddressModified()) {
                                self.checkoutService.updateCart(self.viewModelName)
                                    .then(result => {
                                        const { Cart } = result;
                                        this.adressBeforeEdit = { ...this.Cart.ShippingAddress };
                                        this.Cart = Cart;
                                        processShipping.resolve(true);
                                    });
                            } else {
                                processShipping.resolve(true);
                            }
                        } else {
                            processShipping.resolve(false);
                        };
                        return processShipping.promise;
                    },
                    changePostalCode() {
                       
                        let isValid = this.initializeParsey('#addressForm');
                        this.PostalCodeError = false;
                        if (isValid) {
                            this.IsLoading = true;
                            self.checkoutService.updatePostalCode(this.Cart.ShippingAddress.PostalCode)
                                .then((cart: any) => {

                                    this.Cart = {
                                        ...this.Cart,
                                        ShippingAddress: {
                                            ...this.Cart.ShippingAddress,
                                            PostalCode: cart.ShippingAddress.PostalCode,
                                            RegionCode: cart.ShippingAddress.RegionCode,
                                            RegionName: cart.ShippingAddress.RegionName
                                        },
                                        OrderSummary: cart.OrderSummary
                                    };
                                })
                                .fail(e => {
                                    if(e && e.Errors) {
                                        this.PostalCodeError = true;
                                        console.log(e.Errors);
                                    }
                                })
                                .finally(() => this.IsLoading = false);
                        }

                    },
                    isAddressModified() {
                        var formData = self.getSerializedForm();
                        var keys = _.keys(formData);
                        var isModified = _.some(keys, (key) => this.adressBeforeEdit[key] != this.Cart.ShippingAddress[key]);
                        return isModified;
                    },
                    adjustPostalCode() {
                        this.Cart.ShippingAddress.PostalCode = this.Cart.ShippingAddress.PostalCode.toUpperCase();
                    }
                }
            };

            this.checkoutService.VueCheckoutMixins.push(vueShippingAddressMixin);
        }

        public getUpdateModelPromise(): Q.Promise<any> {
            return Q.fcall(() => {
                var vm = {};
                var vueData = this.checkoutService.VueCheckout;
                var vueAddressData = vueData.AddingNewAddressMode ? vueData.AddNewAddress : vueData.Cart.ShippingAddress;
                vm[this.viewModelName] = JSON.stringify(vueAddressData);
                return vm;
            });
        }

    }
}
