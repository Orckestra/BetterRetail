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

            let vueShippingMixin = {
                data: {
                    SelectedShippingMethodType: null
                },
                mounted() {
                    let selectedProviderId = this.Cart.ShippingMethod ? this.Cart.ShippingMethod.ShippingProviderId : undefined;
                    this.ShippingMethodTypes.forEach(type => {
                        type.IsModified = type.ShippingMethods.length > 1;

                        let selectedInCart = type.ShippingMethods.find(method => method.ShippingProviderId === selectedProviderId);
                        if(selectedInCart) {
                            this.SelectedShippingMethodType = type.FulfillmentMethodTypeString;
                        }
                        type.SelectedMethod = selectedInCart || type.ShippingMethods.find(method => method.IsSelected)
                    });
                },
                computed: {
                },
                methods: {
                    processShipping() {
                        this.Cart.ShippingMethod = this.ShippingMethodTypes.find(type => type.FulfillmentMethodTypeString === this.SelectedShippingMethodType);
                        return !!this.Cart.ShippingMethod;
                    },
                    selectShippingMethod(methodEntity: any) {
                        this.ShippingMethodTypes = this.ShippingMethodTypes.map(x =>
                            x.FulfillmentMethodTypeString === methodEntity.FulfillmentMethodTypeString ? { ...x, SelectedMethod: methodEntity } : x
                        );
                        this.SelectedShippingMethodType = methodEntity.FulfillmentMethodTypeString;
                        this.onChangeShippingMethodTypeAction(methodEntity.FulfillmentMethodTypeString);
                    },
                    changeShippingMethodType(e: any) {
                        const { value } = e.target;
                        this.onChangeShippingMethodTypeAction(value);
                    },
                    onChangeShippingMethodTypeAction(value: any) {
                        if(value === FulfillmentMethodTypes.Shipping) {
                            $('#ShippingAddress').collapse('show')
                        } else {
                            $('#ShippingAddress').collapse('hide')
                        }
                    }
                }
            };

            this.checkoutService.VueCheckoutMixins.push(vueShippingMixin);
        }

    }
}
