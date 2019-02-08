///<reference path='../../../../Composer.UI/Source/Typings/tsd.d.ts' />
///<reference path='../../../../Composer.UI/Source/Typescript/Mvc/ComposerClient.ts' />
///<reference path='../../../../Composer.MyAccount.UI/Common/Source/Typescript/CustomerService.ts' />
///<reference path='../../../CheckoutCommon/Source/Typescript/AddressDto.ts' />

module Orckestra.Composer {
    'use strict';
    export class ShippingAddressRegisteredService {

        protected customerService: ICustomerService;

        public constructor(customerService: ICustomerService) {
            this.customerService = customerService;
        }

          /**
         * Get the customer addresses. The selected shipping address is taken from the cart by default.
         * If no address has been set in the cart, the selected shipping address corresponds to the preferred address.
         */
        public getShippingAddresses(cart: any): Q.Promise<any> {

            if (!cart) {
                throw new Error('The cart is required');
            }

            return this.customerService.getAddresses()
                .then(addresses => {
                    addresses.AddressesLoaded = true;
                    addresses.SelectedShippingAddressId = this.getSelectedShippingAddressId(cart, addresses);

                    return addresses;
                });
        }

        public getSelectedShippingAddressId(cart: any, addressList: any) : string {

            if (this.isShippingAddressFromCartValid(cart, addressList)) {
                return cart.ShippingAddress.AddressBookId;
            }

            return this.getPreferredShippingAddressId(addressList);
        }

        private isShippingAddressFromCartValid(cart: any, addressList: any) : boolean {

            if (cart.ShippingAddress === undefined) {
                return false;
            }

            return _.some(addressList.Addresses, (address: AddressDto) => address.Id === cart.ShippingAddress.AddressBookId);
        }

        private getPreferredShippingAddressId(addressList: any) : string {

            var preferredShippingAddress = _.find(addressList.Addresses, (address: AddressDto) => address.IsPreferredShipping);

            return preferredShippingAddress === undefined ? undefined : preferredShippingAddress.Id;
        }
    }
}
