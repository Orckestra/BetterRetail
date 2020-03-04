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
                    ComplementaryAddressAddState: false
                },
                created()  {
                    this.adressBeforeEdit = { ...this.Cart.ShippingAddress };
                    this.ComplementaryAddressAddState = !this.Cart.ShippingAddress.Line2;
                },
                mounted() {
                 
                },
                computed: {
                    ShippingAddressPreview() {
                        return `${this.ShippingAddress.Line1}${this.ShippingAddress.Line2 ? ' ' + this.ShippingAddress.Line2: ''}, 
                        ${this.ShippingAddress.City}, 
                        ${this.ShippingAddress.RegionName}, ${this.ShippingAddress.PostalCode}`;
                    }
                },
                methods: {
                    processShippingAddress() {

                        if (this.IsShippingMethodType) {
                            var processAddress: Q.Deferred<boolean> = Q.defer<boolean>();
                            this.parsleyInit = $('#addressForm').parsley();
                            this.parsleyInit.validate();
                            let isValid = this.parsleyInit.isValid();
      
                            if (isValid) {
                                if (this.isAddressModified()) {
                                    self.checkoutService.updateCart(self.viewModelName)
                                    .then(cart => {
                                        this.adressBeforeEdit = { ...this.Cart.ShippingAddress };
                                        this.Cart = cart;
                                        processAddress.resolve(true);
                                    });
                                } else {
                                    processAddress.resolve(true);
                                }
                            } else {
                                processAddress.resolve(false);
                            };
                            return processAddress.promise;
                        }

                        return true;
                    },
                    changePostalCode() {
                        this.IsLoading = true;
                        self.checkoutService.updatePostalCode(this.Cart.ShippingAddress.PostalCode)
                        .then(cart => {
                             this.Cart = cart;
                        })
                        .finally(() => this.IsLoading = false);;

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
                var vueAddressData = this.checkoutService.VueCheckout.Cart.ShippingAddress;
                var formData: any = this.getSerializedForm();

                var keys = _.keys(formData);
                _.each(keys, key => {
                    if (vueAddressData.hasOwnProperty(key)) {
                        formData[key] = vueAddressData[key];
                    }
                });

                formData["FirstName"] = this.checkoutService.VueCheckout.Cart.Customer.FirstName;
                formData["LastName"] = this.checkoutService.VueCheckout.Cart.Customer.LastName;

                vm[this.viewModelName] = JSON.stringify(formData);
                return vm;
            });
        }

    }
}
