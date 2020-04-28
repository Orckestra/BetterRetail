///<reference path='../..//Typings/tsd.d.ts' />
///<reference path='./BaseSingleCheckoutController.ts' />
///<reference path='../Composer.MyAccount/Common/CustomerService.ts' />
///<reference path='./Services/ShippingAddressRegisteredService.ts' />

module Orckestra.Composer {
    'use strict';

    export class ShippingSingleCheckoutController extends Orckestra.Composer.BaseSingleCheckoutController {

        protected customerService: ICustomerService = new CustomerService(new CustomerRepository());


        public initialize() {
            super.initialize();
            let self: ShippingSingleCheckoutController = this;
            self.viewModelName = 'ShippingMethod';

            let vueShippingMixin = {
                mounted() {
                    this.calculateSelectedMethod();
                    this.Steps.Shipping.EnteredOnce = this.FulfilledShipping;
                    this.prepareShipping();
                },
                computed: {
                    FulfilledShipping() {
                        return self.checkoutService.shippingFulfilled(this.Cart, this.IsAuthenticated);
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
                            this.Cart.ShippingMethod.FulfillmentMethodTypeString === FulfillmentMethodTypes.Shipping;
                    },
                    IsPickUpMethodType() {
                        return this.Cart.ShippingMethod &&
                            this.Cart.ShippingMethod.FulfillmentMethodTypeString === FulfillmentMethodTypes.PickUp;
                    },

                },
                methods: {
                    prepareShipping() {
                        if (!this.Cart.ShippingAddress.FirstName && !this.Cart.ShippingAddress.LastName) {
                            this.Cart.ShippingAddress.FirstName = this.Customer.FirstName;
                            this.Cart.ShippingAddress.LastName = this.Customer.LastName;
                        }

                        this.Mode.AddingLine2Address = !this.Cart.ShippingAddress.Line2;
                        this.Mode.AddingNewAddress = false;

                        this.ShippingMethodTypes.forEach(methodType => {
                            if (this.IsPickUpMethodType && methodType.FulfillmentMethodTypeString === FulfillmentMethodTypes.Shipping) {
                                methodType.OldAddress = this.clearShippingAddress();
                            } else {
                                methodType.OldAddress = this.Cart.ShippingAddress;
                            }
                        });

                        this.preparePickUpAddress();
                    },
                    processShipping() {

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

                        this.Steps.EnteredOnce.Shipping = true;
                        return true;
                    },
                    processBilling() {
                        if (this.IsAuthenticated) {
                            return this.processBillingAddressRegistered();
                        } else {
                            return this.processBillingAddress();
                        }
                    },
                    selectShippingMethod(methodEntity: any) {
                        this.ShippingMethodTypes = this.ShippingMethodTypes.map(x =>
                            x.FulfillmentMethodTypeString === methodEntity.FulfillmentMethodTypeString ? { ...x, SelectedMethod: methodEntity } : x
                        );

                        this.changeMethodsCollapseState(methodEntity.FulfillmentMethodTypeString, 'hide');
                        this.updateShippingMethodProcess(methodEntity);
                    },
                    changeShippingMethodType(e: any) {
                        const { value } = e.target;
                        let shippingMethodType = this.ShippingMethodTypes.find(method =>
                            method.FulfillmentMethodTypeString === value
                        );

                        if (this.Cart.ShippingMethod) {
                            this.changeMethodsCollapseState(this.Cart.ShippingMethod.FulfillmentMethodTypeString, 'hide');
                        }

                        if (!this.debounceUpdateShippingMethod) {
                            this.debounceUpdateShippingMethod = _.debounce(methodType => {
                                this.updateShippingMethodProcess(methodType.SelectedMethod)
                                    .then(() => {
                                        if (this.IsPickUpMethodType) { this.onSelectPickUpMethod(); }
                                    });
                            }, 800);
                        }

                        this.debounceUpdateShippingMethod(shippingMethodType);
                    },

                    changeMethodsCollapseState(shippingMethodType: string, command: string) {
                        let shippingMethodCollapse = $(`#ShippingMethod${shippingMethodType}`);
                        if (shippingMethodCollapse) {
                            shippingMethodCollapse.collapse(command);
                        }
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
                    },
                    calculateSelectedMethod() {
                        let selectedProviderId = this.Cart.ShippingMethod ? this.Cart.ShippingMethod.ShippingProviderId : undefined;
                        this.ShippingMethodTypes.forEach(type => {
                            type.IsModified = type.ShippingMethods.length > 1;

                            let selectedInCart = type.ShippingMethods.find(method => method.ShippingProviderId === selectedProviderId);
                            type.SelectedMethod = selectedInCart || type.ShippingMethods.find(method => method.IsSelected);
                        });
                    }
                }
            };

            this.checkoutService.VueCheckoutMixins.push(vueShippingMixin);
        }

        public getUpdateModelPromise(): Q.Promise<any> {
            return Q.fcall(() => {
                let { Name, ShippingProviderId } = this.checkoutService.VueCheckout.Cart.ShippingMethod;
                return {[this.viewModelName]: JSON.stringify({ Name, ShippingProviderId })};
            });
        }
    }
}
