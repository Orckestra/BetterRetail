///<reference path='../..//Typings/tsd.d.ts' />
///<reference path='./BaseSingleCheckoutController.ts' />
///<reference path='./ShippingAddressSingleCheckoutController.ts' />
///<reference path='../Composer.Cart/CheckoutShippingAddressRegistered/ShippingAddressRegisteredService.ts' />

module Orckestra.Composer {
    'use strict';

    export class ShippingAddressRegisteredSingleCheckoutController extends Orckestra.Composer.ShippingAddressSingleCheckoutController {
        protected customerService: ICustomerService = new CustomerService(new CustomerRepository());
        protected shippingAddressRegisteredService: ShippingAddressRegisteredService =
            new ShippingAddressRegisteredService(this.customerService);

        public initialize() {
            super.initialize();
            let self: ShippingAddressRegisteredSingleCheckoutController = this;
            self.viewModelName = 'ShippingAddressRegistered';

            let vueShippingAddressRegisteredMixin = {
                data: {
                    RegisteredAddresses: {},
                    SelectedShippingAddressId: null,
                    AddressName: null,
                },
                created() {

                },
                mounted() {

                    if (this.IsAuthenticated) {
                        self.shippingAddressRegisteredService.getShippingAddresses(this.Cart)
                            .then(data => {
                                this.RegisteredAddresses = data.Addresses;
                            });
                    }
                },
                computed: {
                },
                methods: {
                    processShippingAddressRegistered() {
                        if (this.shippingAddressModified()) {
                            return self.checkoutService.updateCart([self.viewModelName])
                                .then(() => {
                                    this.Steps.EnteredOnce.Shipping = true;
                                    return true;
                                });
                        } else {
                            return true;
                        }
                    },

                    addNewAddressMode() {
                        this.Mode.AddingNewAddress = true;
                        this.adressBeforeEdit = {};
                        this.AddressName = null;
                        this.SelectedShippingAddressId = undefined;
                        this.clearShippingAddress();
                    },

                    addShippingAddressToMyAddressBook() {
                        let formId = '#addNewAddressForm';
                        let isValid = this.initializeParsey(formId);
                        if (!isValid) {
                            return Q.reject('Shipping Address information is not valid');
                        }

                        let postalCode = this.Cart.ShippingAddress.PostalCode;
                        this.changePostalCode(postalCode)
                            .then(success => {
                                if (success) {
                                    let addressData = { ...this.Cart.ShippingAddress };
                                    addressData.AddressName = this.AddressName;

                                    self.checkoutService.saveAddressToMyAccountAddressBook(addressData)
                                        .then(address => {
                                            address.RegionName = this.ShippingAddress.RegionName;
                                            this.changeRegisteredShippingAddress(address.Id);
                                        })
                                        .fail((reason) => {
                                            console.log(reason);
                                            if (reason.Errors && _.find(reason.Errors, (e: any) => e.ErrorCode == 'NameAlreadyUsed')) {
                                                this.Errors.AddressNameAlreadyInUseError = true;
                                            }
                                            if (reason.Errors && _.find(reason.Errors, (e: any) => e.ErrorCode == 'InvalidPhoneFormat')) {
                                                this.Errors.InvalidPhoneFormatError = true;
                                            }
                                        });
                                } else {
                                    //
                                }
                            })
                    },

                    changeRegisteredShippingAddress(addressId, addingNewAddressPromise = null) {

                        this.SelectedShippingAddressId = addressId;
                        this.Mode.AddingNewAddress = false;
                        if (!this.debounceChangeRegisteredShippingAddress) {
                            this.debounceChangeRegisteredShippingAddress = _.debounce((addingNewAddressPromise) => {
                                //WHEN CHANGING SHIPPING, WE ALSO NEED UPDATE BILLING
                                let controllersToUpdate = [self.viewModelName, 'BillingAddressRegistered'];
                                self.checkoutService.updateCart(controllersToUpdate).then((response: any) => {
                                    let { Cart } = response;
                                    this.Cart.ShippingAddress = Cart.ShippingAddress;
                                    this.Cart.Payment.BillingAddress = Cart.Payment.BillingAddress;
                                    if (addingNewAddressPromise) {
                                        this.Steps.EnteredOnce.Shipping = true;
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
                let vm = {};
                let selectedAddressId = this.checkoutService.VueCheckout.SelectedShippingAddressId;
                vm[this.viewModelName] = JSON.stringify({ ShippingAddressId: selectedAddressId });

                return vm;
            });
        }

    }
}
