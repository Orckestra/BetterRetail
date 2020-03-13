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
                    AddNewAddress: { },
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
                                    this.AddNewAddress = {...this.ShippingAddress};
                                    this.AddingNewAddressMode = true;
                                }
                                this.AddNewAddress.CountryCode = this.ShippingAddress.CountryCode;
                            });
                        
                        
                    }
                },
                computed: {
                },
                methods: {

                    processRegisteredShippingAddress() {
                        if (this.AddingNewAddressMode) {
                            var processShipping: Q.Deferred<boolean> = Q.defer<boolean>();
                            let isValid = this.initializeParsey('#addNewAddressForm');
                            if (isValid) {
                                self.checkoutService.updateCart("ShippingAddress")
                                .then(result => {
                                    const { Cart } = result;
                                    this.Cart = Cart;
                                    processShipping.resolve(true);
                                });
                            } else {
                                processShipping.resolve(false);
                            };
                            return processShipping.promise;
                        } else { return true; };
                    },
                    addNewAddressMode() {
                        this.AddingNewAddressMode = true;
                        this.SelectedShippingAddressId = undefined;
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
                    changePostalCodeRegistered() {
                        let isValid = this.initializeParsey('#addNewAddressForm');
                        if (isValid) {
                            this.IsLoading = true;
                            self.checkoutService.updatePostalCode(this.AddNewAddress.PostalCode)
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
                                .finally(() => this.IsLoading = false);
                        }
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
