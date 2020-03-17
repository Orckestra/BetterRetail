///<reference path='../..//Typings/tsd.d.ts' />
///<reference path='./BaseSingleCheckoutController.ts' />
///<reference path='../Composer.MyAccount/Common/CustomerService.ts' />
///<reference path='../Composer.Cart/CheckoutShippingAddressRegistered/ShippingAddressRegisteredService.ts' />

module Orckestra.Composer {
    'use strict';

    enum FulfillmentMethodTypes {
        Shipping = 'Shipping',
        PickUp = 'PickUp'
    }

    export class ShippingSingleCheckoutController extends Orckestra.Composer.BaseSingleCheckoutController {

        protected customerService: ICustomerService = new CustomerService(new CustomerRepository());
      

        public initialize() {
            super.initialize();
            let self: ShippingSingleCheckoutController = this;
            self.viewModelName = 'ShippingMethod';

            let vueShippingMixin = {
                data: {

                },
                mounted() {
                    this.calculateSelectedMethod();

                },
                computed: {
                    FulfilledShipping() {

                        let fulfilledAddress =
                            this.ShippingAddress.FirstName &&
                            this.ShippingAddress.LastName &&
                            this.ShippingAddress.Line1 &&
                            this.ShippingAddress.City &&
                            this.ShippingAddress.RegionCode &&
                            this.ShippingAddress.PostalCode;

                        if (this.IsAuthenticated) {
                            fulfilledAddress = fulfilledAddress || this.SelectedShippingAddressId
                        }

                        return fulfilledAddress && this.Cart.ShippingMethod && !this.IsLoading;
                    },
                    SelectedMethodTypeString() {
                        return this.Cart.ShippingMethod ? this.Cart.ShippingMethod.FulfillmentMethodTypeString : '';
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
                    processShipping() {
                        if (this.IsShippingMethodType) {
                            return this.processShippingAddress();
                        }
                        return true;
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
                                this.updateShippingMethodProcess(methodType.SelectedMethod);
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
                    updateShippingMethodProcess(methodEntity: any) {
                        let oldValue = { ...this.Cart.ShippingMethod };
                        this.Cart.ShippingMethod = methodEntity;

                        if (methodEntity.ShippingProviderId === oldValue.ShippingProviderId) return;

                        self.checkoutService.updateCart(self.viewModelName)
                            .then(({ Cart }) => {
                                this.Cart = Cart;
                            }).catch(e => {
                                this.Cart.ShippingMethod = oldValue;
                            })
                    },
                   

                    calculateSelectedMethod() {
                        let selectedProviderId = this.Cart.ShippingMethod ? this.Cart.ShippingMethod.ShippingProviderId : undefined;
                        this.ShippingMethodTypes.forEach(type => {
                            type.IsModified = type.ShippingMethods.length > 1;

                            let selectedInCart = type.ShippingMethods.find(method => method.ShippingProviderId === selectedProviderId);
                            type.SelectedMethod = selectedInCart || type.ShippingMethods.find(method => method.IsSelected)
                        });
                    }
                }
            };

            this.checkoutService.VueCheckoutMixins.push(vueShippingMixin);
        }

        public getUpdateModelPromise(): Q.Promise<any> {
            return Q.fcall(() => {
                let vm = {};
                let { Name, ShippingProviderId } = this.checkoutService.VueCheckout.Cart.ShippingMethod;
                vm[this.viewModelName] = JSON.stringify({ Name, ShippingProviderId });
                return vm;
            });
        }
    }
}
