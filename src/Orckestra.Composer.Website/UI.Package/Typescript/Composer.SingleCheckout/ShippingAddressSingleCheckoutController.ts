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
                            // TODO: Investigate how to make this work: var isValid = self.isValidForUpdate();

                            if (isValid) {
                                if (this.isAddressModified()) {
                                    this.IsLoading = true;
                                    self.checkoutService.updateCart(self.viewModelName).then(result => {
                                        this.adressBeforeEdit = { ...this.Cart.ShippingAddress };
                                        this.IsLoading = false;
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
                    isAddressModified() {
                        var formData = self.getSerializedForm();
                        var keys = _.keys(formData);
                        var isModified = _.some(keys, (key) => this.adressBeforeEdit[key] != this.Cart.ShippingAddress[key]);
                        return isModified;
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
