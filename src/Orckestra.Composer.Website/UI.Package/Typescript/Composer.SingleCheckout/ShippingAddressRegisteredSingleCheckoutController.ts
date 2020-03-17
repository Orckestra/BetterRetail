///<reference path='../..//Typings/tsd.d.ts' />
///<reference path='./BaseSingleCheckoutController.ts' />

module Orckestra.Composer {
    'use strict';

    export class ShippingAddressRegisteredSingleCheckoutController extends Orckestra.Composer.BaseSingleCheckoutController {
        protected customerService: ICustomerService = new CustomerService(new CustomerRepository());
        protected shippingAddressRegisteredService: ShippingAddressRegisteredService =
            new ShippingAddressRegisteredService(this.customerService);

        public initialize() {
            super.initialize();
            var self: ShippingAddressRegisteredSingleCheckoutController = this;
            self.viewModelName = 'ShippingAddressRegistered';

            let vueShippingAddressRegisteredMixin = {
                data: {
                    RegisteredAddresses: {},
                    AddingNewAddressMode: false,
                    SelectedShippingAddressId: null
                },
                created() {

                },
                mounted() {

                    if (this.IsAuthenticated) {
                        self.shippingAddressRegisteredService.getShippingAddresses(this.Cart)
                            .then(data => {
                                this.RegisteredAddresses = data.Addresses;
                                var isNewAddreess = !this.ShippingAddress.AddressBookId && this.ShippingAddress.PostalCode;
                                if (!isNewAddreess) {
                                    this.SelectedShippingAddressId = data.SelectedShippingAddressId;
                                } else {
                                    this.AddingNewAddressMode = true;
                                }
                            });
                    }
                },
                computed: {
                },
                methods: {

                    addNewAddressMode() {
                        this.AddingNewAddressMode = true;
                        this.SelectedShippingAddressId = undefined;
                        this.Cart.ShippingAddress = { CountryCode: this.Cart.ShippingAddress.CountryCode };
                    },
                    changeRegisteredShippingAddress(addressId) {

                        this.SelectedShippingAddressId = addressId;
                        this.AddingNewAddressMode = false;
                        if (!this.debounceChangeRegisteredShippingAddress) {
                            this.debounceChangeRegisteredShippingAddress = _.debounce(() => {
                                console.log(this.SelectedShippingAddressId);
                                self.checkoutService.updateCart(self.viewModelName).then((response: any) => {
                                    let { Cart } = response;
                                    this.Cart = Cart;
                                });
                            }, 500);
                        }

                        this.debounceChangeRegisteredShippingAddress();
                    },
                }
            };

            this.checkoutService.VueCheckoutMixins.push(vueShippingAddressRegisteredMixin);
        }

        public getUpdateModelPromise(): Q.Promise<any> {
            return Q.fcall(() => {
                var vm = {};
                var selectedAddressId = this.checkoutService.VueCheckout.SelectedShippingAddressId;
                vm[this.viewModelName] = JSON.stringify({ ShippingAddressId: selectedAddressId });
                return vm;
            });
        }

    }
}
