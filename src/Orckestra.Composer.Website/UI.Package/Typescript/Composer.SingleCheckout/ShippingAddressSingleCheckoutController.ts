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
                              if (this.addressModified()) {
                                  this.changePostalCode().then(success => {
                                      if (success) {
                                          self.checkoutService.updateCart(self.viewModelName)
                                              .then(result => {
                                                  const { Cart } = result;
                                                  this.adressBeforeEdit = { ...this.Cart.ShippingAddress };
                                                  this.Cart = Cart;
                                                  processShipping.resolve(true);
                                              })
                                              .fail(reason => {
                                                  console.log(reason);
                                                  processShipping.resolve(false);
                                              });
                                      } else {
                                        processShipping.resolve(false);
                                      }
                                  })
                               
                            } else {
                                processShipping.resolve(true);
                            }
                        } else {
                            processShipping.resolve(false);
                        };

                        return processShipping.promise;
                    },
                    recalculateShippingFee() {
                       
                        let isValid = this.initializeParsey('#addressForm');
                        if (isValid) {
                            this.changePostalCode();
                        }

                    },
                    changePostalCode() {
                        var processPostalCode: Q.Deferred<boolean> = Q.defer<boolean>();
         
                        this.PostalCodeError = false;
                        if (this.adressBeforeEdit.PostalCode != this.Cart.ShippingAddress.PostalCode) {
                            this.IsLoading = true;
                            self.checkoutService.updatePostalCode(this.Cart.ShippingAddress.PostalCode).then((cart: any) => {
                                this.adressBeforeEdit = { ...this.Cart.ShippingAddress };
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
                                processPostalCode.resolve(true);
                            })
                            .fail(reason => {
                                console.log(reason);
                                this.PostalCodeError = true;
                                processPostalCode.resolve(false);
                            })
                            .finally(() => this.IsLoading = false);
                        } else {
                            processPostalCode.resolve(true);
                        }
                        return processPostalCode.promise;
                    },

                    addressModified() {
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
