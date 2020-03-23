///<reference path='../..//Typings/tsd.d.ts' />
///<reference path='./BaseSingleCheckoutController.ts' />

module Orckestra.Composer {
    'use strict';

    export class ShippingAddressRegisteredSingleCheckoutController extends Orckestra.Composer.ShippingAddressSingleCheckoutController {
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
                    SelectedShippingAddressId: null,
                    IsPreferredShipping: false,
                    AddressName: null,
                    AddressNameAlreadyInUseError: false
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
                        this.adressBeforeEdit = {};
                        this.AddressName = null;
                        this.SelectedShippingAddressId = undefined;
                        this.ComplementaryAddressAddState = true;
                        this.Cart.ShippingAddress = { 
                            FirstName: this.Cart.ShippingAddress.FirstName || this.Cart.Customer.FirstName,
                            LastName: this.Cart.ShippingAddress.LastName || this.Cart.Customer.LastName,
                            CountryCode: this.Cart.ShippingAddress.CountryCode };
                    },

                    processAddingNewShippingAddress() {
                        var processAddingNewShippingAddress: Q.Deferred<boolean> = Q.defer<boolean>();
                        let formId = '#addNewAddressForm';
                        let isValid = this.initializeParsey(formId);
                        if (isValid) {

                            let postalCode = this.Cart.ShippingAddress.PostalCode;
                            this.changePostalCode(postalCode).then(success => {
                                if (success) {
                                    let addressData = { ...this.Cart.ShippingAddress };
                                    addressData.AddressName = this.AddressName;

                                    self.customerService.createAddress(addressData, null).then(address => {
                                        address.RegionName = this.ShippingAddress.RegionName;
                                        this.RegisteredAddresses.push(address);
                                        this.changeRegisteredShippingAddress(address.Id, processAddingNewShippingAddress);

                                    }).fail((reason) => {
                                        console.log(reason);
                                        if (reason.Errors && _.find(reason.Errors, (e: any) => e.ErrorCode == 'NameAlreadyUsed')) {
                                            this.AddressNameAlreadyInUseError = true;
                                        }
                                        processAddingNewShippingAddress.resolve(false);
                                    });
                                } else {
                                    processAddingNewShippingAddress.resolve(false);
                                }
                            })

                        } else {
                            processAddingNewShippingAddress.resolve(false);
                        };

                        return processAddingNewShippingAddress.promise;
                    },

                    changeRegisteredShippingAddress(addressId, addingNewAddressPromise = null) {

                        this.SelectedShippingAddressId = addressId;
                        this.AddingNewAddressMode = false;
                        if (!this.debounceChangeRegisteredShippingAddress) {
                            this.debounceChangeRegisteredShippingAddress = _.debounce((addingNewAddressPromise) => {
                                self.checkoutService.updateCart(self.viewModelName).then((response: any) => {
                                    let { Cart } = response;
                                    this.Cart = Cart;
                                    if (addingNewAddressPromise) {
                                        addingNewAddressPromise.resolve(true);
                                    }
                                }).fail((reason) => {
                                    console.log(reason);
                                   
                                    if (addingNewAddressPromise) {
                                        addingNewAddressPromise.resolve(false);
                                    }
                                });
                            }, 500);
                        }

                        this.debounceChangeRegisteredShippingAddress(addingNewAddressPromise);
                    }

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
