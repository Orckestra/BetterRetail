///<reference path='../..//Typings/tsd.d.ts' />
///<reference path='./BaseSingleCheckoutController.ts' />
///<reference path='../Composer.MyAccount/Common/CustomerService.ts' />
///<reference path='./Services/ShippingAddressRegisteredService.ts' />
///<reference path='../Composer.Grocery/FulfillmentEvents.ts' />

module Orckestra.Composer {
    'use strict';

    export class ShippingSingleCheckoutController extends Orckestra.Composer.BaseSingleCheckoutController {

        protected customerService: ICustomerService = new CustomerService(new CustomerRepository());
        protected fulfillmentService: IFulfillmentService = FulfillmentService.instance();

        public initialize() {
            super.initialize();
            let self: ShippingSingleCheckoutController = this;
            self.viewModelName = 'ShippingMethod';

            let vueShippingMixin = {
                data: {

                },
                mounted() {
                    self.eventHub.subscribe(FulfillmentEvents.FulfillmentMethodSelected, e => this.onFulfillmentMethodSelected(e.data));
                    self.eventHub.subscribe(FulfillmentEvents.StoreSelected, e => this.onStoreSelected(e.data));
                    self.eventHub.subscribe(FulfillmentEvents.StoreUpdating, e => this.onStoreUpdating(e.data));
                    self.eventHub.subscribe(FulfillmentEvents.TimeSlotSelected, e => this.onSlotSelected(e.data));
                    self.eventHub.subscribe(FulfillmentEvents.TimeSlotUpdating, e => this.onSlotUpdating(e.data));
                    self.eventHub.subscribe(FulfillmentEvents.TimeSlotSelectionFailed, e => this.onSlotFailed(e.data));
                    this.prepareShipping();
                },
                computed: {
                    FulfilledShipping() {
                        return self.checkoutService.shippingFulfilled(this.Cart, this.IsAuthenticated) &&
                            this.IsStoreSelected;
                    },
                    SelectedMethodTypeString() {
                        return this.Cart.ShippingMethod ? this.Cart.ShippingMethod.FulfillmentMethodTypeString : '';
                    },
                    SelectedMethodType() {
                        return this.Cart.ShippingMethod &&
                            this.ShippingMethodTypes.find(
                                type => type.FulfillmentMethodTypeString === this.Cart.ShippingMethod.FulfillmentMethodTypeString
                            );
                    },
                    IsShippingMethodType() {
                        return this.Cart.ShippingMethod &&
                            this.Cart.ShippingMethod.FulfillmentMethodType === FulfillmentMethodTypes.Shipping;
                    },
                    IsPickUpMethodType() {
                        return this.Cart.ShippingMethod &&
                            this.Cart.ShippingMethod.FulfillmentMethodType === FulfillmentMethodTypes.PickUp;
                    }

                },
                methods: {
                    prepareShipping() {
                        if (!this.Cart.ShippingAddress.FirstName && !this.Cart.ShippingAddress.LastName) {
                            // this.Cart.ShippingAddress.FirstName = this.Customer.FirstName;
                            // this.Cart.ShippingAddress.LastName = this.Customer.LastName;
                        }

                        this.Mode.AddingLine2Address = !this.Cart.ShippingAddress.Line2;
                        this.Mode.AddingNewAddress = false;

                        this.ShippingMethodTypes.forEach(methodType => {
                            if (this.IsPickUpMethodType && methodType.FulfillmentMethodType === FulfillmentMethodTypes.Shipping) {
                                methodType.OldAddress = this.clearShippingAddress();
                            } else {
                                methodType.OldAddress = this.Cart.ShippingAddress;
                            }
                        });
                        this.preparePickUpAddress();
                    },
                    processShipping(): Q.Promise<boolean> {
                        if (this.IsShippingMethodType) {
                            if (this.IsAuthenticated) {
                                return this.processShippingAddressRegistered();
                            } else {
                                return this.processShippingAddress();
                            }
                        }

                        if (this.IsPickUpMethodType) {
                            return this.processPickUpAddress();
                        }

                        return Q.resolve(true);
                    },
                    processBilling() {
                        if (this.IsAuthenticated) {
                            return this.processBillingAddressRegistered();
                        } else {
                            return this.processBillingAddress();
                        }
                    },
                    onFulfillmentMethodSelected(method) {
                        this.Cart.ShippingMethod.FulfillmentMethodType = method;
                    },
                    updateShippingMethodProcess(methodEntity: any): Q.Promise<any> {
                        let oldShippingMethod = { ...this.Cart.ShippingMethod };
                        let oldPickUpLocationId = this.Cart.PickUpLocationId;

                        if (this.SelectedMethodType) {
                            this.SelectedMethodType.OldAddress = { ...this.Cart.ShippingAddress };
                        }
                        this.Cart.ShippingMethod = methodEntity;

                        if (methodEntity.ShippingProviderId === oldShippingMethod.ShippingProviderId) { return; }

                        this.Cart.ShippingAddress = this.clearShippingAddress();
                        return self.checkoutService.updateCart([self.viewModelName])
                            .then(() => {
                                this.Cart.ShippingAddress = this.SelectedMethodType.OldAddress;
                                this.Cart.PickUpLocationId = oldPickUpLocationId;
                            }).catch(() => {
                                this.Cart.ShippingMethod = oldShippingMethod;
                            });
                    },
                    clearShippingAddress() {
                        this.Mode.AddingLine2Address = true;
                        let { ShippingAddress: { FirstName, LastName, CountryCode, PhoneRegex, PostalCodeRegexPattern }, Customer } = this.Cart;

                        return {
                            FirstName: FirstName || Customer.FirstName,
                            LastName: LastName || Customer.LastName,
                            PhoneRegex,
                            PostalCodeRegexPattern,
                            CountryCode
                        };
                    }

                }
            };

            this.checkoutService.VueCheckoutMixins.push(vueShippingMixin);
        }

        public getUpdateModelPromise(): Q.Promise<any> {
            return Q.fcall(() => {
                let { Name, ShippingProviderId } = this.checkoutService.VueCheckout.Cart.ShippingMethod;
                return { [this.viewModelName]: JSON.stringify({ Name, ShippingProviderId }) };
            });
        }
    }
}
