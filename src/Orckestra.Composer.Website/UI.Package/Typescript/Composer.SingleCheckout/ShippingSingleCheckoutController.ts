///<reference path='../..//Typings/tsd.d.ts' />
///<reference path='./BaseSingleCheckoutController.ts' />

module Orckestra.Composer {
    'use strict';

    enum FulfillmentMethodTypes {
        Shipping = 'Shipping',
        PickUp = 'PickUp'
    }

    export class ShippingSingleCheckoutController extends Orckestra.Composer.BaseSingleCheckoutController {

        public initialize() {
            super.initialize();
            let self: ShippingSingleCheckoutController = this;
            self.viewModelName = 'ShippingMethod';

            let vueShippingMixin = {
                data: {
                },
                mounted() {
                    let selectedProviderId = this.Cart.ShippingMethod ? this.Cart.ShippingMethod.ShippingProviderId : undefined;
                    this.ShippingMethodTypes.forEach(type => {
                        type.IsModified = type.ShippingMethods.length > 1;

                        let selectedInCart = type.ShippingMethods.find(method => method.ShippingProviderId === selectedProviderId);
                        type.SelectedMethod = selectedInCart || type.ShippingMethods.find(method => method.IsSelected)
                    });
                },
                computed: {
                    FulfilledShipping() {
                        let fulfilled = this.Cart.ShippingMethod &&
                            this.ShippingAddress.Line1 &&
                            this.ShippingAddress.City &&
                            this.ShippingAddress.RegionCode &&
                            this.ShippingAddress.PostalCode &&
                            !this.IsLoading;
                       
                        return fulfilled;
                    },
                    SelectedMethodTypeString() {
                        return this.Cart.ShippingMethod ? this.Cart.ShippingMethod.FulfillmentMethodTypeString : '';
                    }
                },
                methods: {
                    processShipping() {

                        return true;
                    },
                    selectShippingMethod(methodEntity: any) {
                        this.ShippingMethodTypes = this.ShippingMethodTypes.map(x =>
                            x.FulfillmentMethodTypeString === methodEntity.FulfillmentMethodTypeString ? { ...x, SelectedMethod: methodEntity } : x
                        );

                        this.changeMethodsCollapseState(methodEntity.FulfillmentMethodTypeString, 'hide');
                        this.onChangeShippingMethodTypeAction(methodEntity.FulfillmentMethodTypeString);
                       this.updateShippingMethodProcess(methodEntity);
                    },
                    changeShippingMethodType(e: any) {
                        const { value } = e.target;
                        let shippingMethodType = this.ShippingMethodTypes.find(method =>
                            method.FulfillmentMethodTypeString === value
                        );

                        if(this.Cart.ShippingMethod) {
                            this.changeMethodsCollapseState(this.Cart.ShippingMethod.FulfillmentMethodTypeString, 'hide');
                        }
                        this.onChangeShippingMethodTypeAction(value);
                        this.updateShippingMethodProcess(shippingMethodType.SelectedMethod);
                    },
                    onChangeShippingMethodTypeAction(value: any) {
                        if(value === FulfillmentMethodTypes.Shipping) {
                            $('#ShippingAddress').collapse('show')
                        } else {
                            $('#ShippingAddress').collapse('hide')
                        }
                    },
                    changeMethodsCollapseState(shippingMethodType: string, command: string) {
                        let shippingMethodCollapse = $(`#ShippingMethod${shippingMethodType}`);
                        if(shippingMethodCollapse) {
                            shippingMethodCollapse.collapse(command);
                        }
                    },
                    updateShippingMethodProcess(methodEntity: any){
                        let oldValue = { ...this.Cart.ShippingMethod };
                        this.Cart.ShippingMethod = methodEntity;

                        if(methodEntity.ShippingProviderId === oldValue.ShippingProviderId) return;

                        self.checkoutService.updateCart()
                            .then(({ Cart }) => {
                                this.Cart = Cart;
                            }).catch(e => {
                                this.Cart.ShippingMethod = oldValue;
                            })
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
