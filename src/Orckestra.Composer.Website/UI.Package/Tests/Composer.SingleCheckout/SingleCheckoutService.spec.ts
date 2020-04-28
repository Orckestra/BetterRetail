///<reference path='../../Typings/tsd.d.ts' />
///<reference path='../../Typescript/Repositories/CartRepository.ts' />
///<reference path='../../Typescript/Mvc/ComposerClient.ts' />
///<reference path='../../Typescript/Events/EventHub.ts' />
///<reference path='../../Typescript/Composer.SingleCheckout/Services/SingleCheckoutService.ts' />

(() => {

    'use strict';

    describe('Getting an instance of CheckoutService', () => {
        var checkoutService: Orckestra.Composer.ISingleCheckoutService;
        let CheckoutStepNumbers = Orckestra.Composer.CheckoutStepNumbers;

        beforeEach(() => {
            //
        });

        afterEach(() => {
            //
        });


        describe('calculateStartStep function', () => {
            checkoutService = Orckestra.Composer.SingleCheckoutService.getInstance();
            describe('Guest Cutomer', () => {
                let isAuthenticated = false;
                it('SHOULD return Information(0) step when Customer data is missing', () => {
                    //Arrange
                    let cart = {
                        Customer: {
                            FirstName: '',
                            LastName: '',
                            Email: '',
                        }
                    }
                    ///Act
                    var step = checkoutService.calculateStartStep(cart, isAuthenticated);

                    ///Assert
                    expect(step).toEqual(CheckoutStepNumbers.Information);
                });

                it('SHOULD return Information (0) step when Customer Email is missing', () => {
                    //Arrange
                    let cart = {
                        Customer: {
                            FirstName: 'Stepan',
                            LastName: 'Bour',
                            Email: '',
                        }
                    }
                    ///Act
                    var step = checkoutService.calculateStartStep(cart, isAuthenticated);

                    ///Assert
                    expect(step).toEqual(CheckoutStepNumbers.Information);
                });

                it('SHOULD return Shipping (1) step when Customer data is present', () => {
                    //Arrange
                    let cart = {
                        Customer: {
                            FirstName: 'Stepan',
                            LastName: 'Bour',
                            Email: 'email@domain.com',
                        }
                    }
                    ///Act
                    var step = checkoutService.calculateStartStep(cart, isAuthenticated);

                    ///Assert
                    expect(step).toEqual(CheckoutStepNumbers.Shipping);
                });

                it('SHOULD return Shipping (1) step when ShippingMethod is undefined', () => {
                    //Arrange
                    let cart = {
                        Customer: {
                            FirstName: 'Stepan',
                            LastName: 'Bour',
                            Email: 'email@domain.com',
                        },
                        ShippingMethod: undefined
                    }
                    ///Act
                    var step1 = checkoutService.calculateStartStep(cart, isAuthenticated);
                    cart.ShippingMethod = null;
                    var step2 = checkoutService.calculateStartStep(cart, isAuthenticated);
                    cart.ShippingMethod = '';
                    var step3 = checkoutService.calculateStartStep(cart, isAuthenticated);

                    ///Assert
                    expect(step1).toEqual(CheckoutStepNumbers.Shipping);
                    expect(step2).toEqual(CheckoutStepNumbers.Shipping);
                    expect(step3).toEqual(CheckoutStepNumbers.Shipping);
                });

                it('SHOULD return Shipping (1) step when ShippingAddress is not fulfilled for Shipping Method type', () => {
                    //Arrange
                    let cart = {
                        Customer: {
                            FirstName: 'Stepan',
                            LastName: 'Bour',
                            Email: 'email@domain.com',
                        },
                        ShippingMethod: {
                            FulfillmentMethodTypeString: Orckestra.Composer.FulfillmentMethodTypes.Shipping
                        },
                        ShippingAddress: {
                            Line1: 'Line1',
                            City: '',
                            RegionCode: '',
                            PostalCode: ''
                        }
                    }
                    ///Act
                    var step1 = checkoutService.calculateStartStep(cart, isAuthenticated);
                    cart.ShippingAddress.City = 'City';
                    var step2 = checkoutService.calculateStartStep(cart, isAuthenticated);
                    cart.ShippingAddress.RegionCode = '';
                    var step3 = checkoutService.calculateStartStep(cart, isAuthenticated);

                    ///Assert
                    expect(step1).toEqual(CheckoutStepNumbers.Shipping);
                    expect(step2).toEqual(CheckoutStepNumbers.Shipping);
                    expect(step3).toEqual(CheckoutStepNumbers.Shipping);
                });

                it('SHOULD return Billing step when ShippingAddress is fulfilled for Shipping method type', () => {
                    //Arrange
                    let cart = {
                        Customer: {
                            FirstName: 'Stepan',
                            LastName: 'Bour',
                            Email: 'email@domain.com',
                        },
                        ShippingMethod: {
                            FulfillmentMethodTypeString: Orckestra.Composer.FulfillmentMethodTypes.Shipping
                        },
                        ShippingAddress: {
                            Line1: 'Line1',
                            City: 'City',
                            RegionCode: 'RegionCode',
                            PostalCode: 'PostalCode',
                            PhoneNumber: 'PhoneNumber'
                        }
                    }
                    ///Act
                    var step = checkoutService.calculateStartStep(cart, isAuthenticated);

                    ///Assert
                    expect(step).toEqual(CheckoutStepNumbers.Billing);
                });

                it('SHOULD return Shipping step when PickUpLocationId is not set for PickUp method type', () => {
                    //Arrange
                    let cart = {
                        Customer: {
                            FirstName: 'Stepan',
                            LastName: 'Bour',
                            Email: 'email@domain.com',
                        },
                        ShippingMethod: {
                            FulfillmentMethodTypeString: Orckestra.Composer.FulfillmentMethodTypes.PickUp
                        },
                        ShippingAddress: {
                            AddressName: '',
                            Line1: 'Line1',
                            City: 'City',
                            RegionCode: 'RegionCode',
                            PostalCode: 'PostalCode'
                        }
                    }
                    ///Act
                    var step = checkoutService.calculateStartStep(cart, isAuthenticated);

                    ///Assert
                    expect(step).toEqual(CheckoutStepNumbers.Shipping);
                });

                it('SHOULD return Billing  step when PickUpLocationId is set for PickUp method type', () => {
                    //Arrange
                    let cart = {
                        Customer: {
                            FirstName: 'Stepan',
                            LastName: 'Bour',
                            Email: 'email@domain.com',
                        },
                        ShippingMethod: {
                            FulfillmentMethodTypeString: Orckestra.Composer.FulfillmentMethodTypes.PickUp
                        },
                        PickUpLocationId: '123',
                        ShippingAddress: {
                            AddressName: 'Store 1',
                            Line1: 'Line1',
                            City: 'City',
                            RegionCode: 'RegionCode',
                            PostalCode: 'PostalCode'

                        }
                    }
                    ///Act
                    var step = checkoutService.calculateStartStep(cart, isAuthenticated);

                    ///Assert
                    expect(step).toEqual(CheckoutStepNumbers.Billing);
                });
            });

            describe('Registered Cutomer', () => {
                let isAuthenticated = true;
                it('SHOULD return Information step when Customer data is missing', () => {
                    //Arrange
                    let cart = {
                        Customer: {
                            FirstName: '',
                            LastName: '',
                            Email: '',
                        }
                    }
                    ///Act
                    var step = checkoutService.calculateStartStep(cart, isAuthenticated);

                    ///Assert
                    expect(step).toEqual(CheckoutStepNumbers.Information);
                });

                it('SHOULD return Information step when Customer Email is missing', () => {
                    //Arrange
                    let cart = {
                        Customer: {
                            FirstName: 'Stepan',
                            LastName: 'Bour',
                            Email: '',
                        }
                    }
                    ///Act
                    var step = checkoutService.calculateStartStep(cart, isAuthenticated);

                    ///Assert
                    expect(step).toEqual(CheckoutStepNumbers.Information);
                });

                it('SHOULD return Shipping step when Customer data is present', () => {
                    //Arrange
                    let cart = {
                        Customer: {
                            FirstName: 'Stepan',
                            LastName: 'Bour',
                            Email: 'email@domain.com',
                        }
                    }
                    ///Act
                    var step = checkoutService.calculateStartStep(cart, isAuthenticated);

                    ///Assert
                    expect(step).toEqual(CheckoutStepNumbers.Shipping);
                });

                it('SHOULD return Shipping step when ShippingMethod is undefined', () => {
                    //Arrange
                    let cart = {
                        Customer: {
                            FirstName: 'Stepan',
                            LastName: 'Bour',
                            Email: 'email@domain.com',
                        },
                        ShippingMethod: undefined
                    }
                    ///Act
                    var step1 = checkoutService.calculateStartStep(cart, isAuthenticated);
                    cart.ShippingMethod = null;
                    var step2 = checkoutService.calculateStartStep(cart, isAuthenticated);
                    cart.ShippingMethod = '';
                    var step3 = checkoutService.calculateStartStep(cart, isAuthenticated);

                    ///Assert
                    expect(step1).toEqual(CheckoutStepNumbers.Shipping);
                    expect(step2).toEqual(CheckoutStepNumbers.Shipping);
                    expect(step3).toEqual(CheckoutStepNumbers.Shipping);
                });

                it('SHOULD return Shipping step when ShippingAddress is not fulfilled for Shipping Method type', () => {
                    //Arrange
                    let cart = {
                        Customer: {
                            FirstName: 'Stepan',
                            LastName: 'Bour',
                            Email: 'email@domain.com',
                        },
                        ShippingMethod: {
                            FulfillmentMethodTypeString: Orckestra.Composer.FulfillmentMethodTypes.Shipping
                        },
                        ShippingAddress: {
                            AddressBookId: '',
                            Line1: 'Line1',
                            City: '',
                            RegionCode: '',
                            PostalCode: ''
                        }
                    }
                    ///Act
                    var step1 = checkoutService.calculateStartStep(cart, isAuthenticated);
                    cart.ShippingAddress.City = 'City';
                    var step2 = checkoutService.calculateStartStep(cart, isAuthenticated);
                    cart.ShippingAddress.RegionCode = 'RegionCode';
                    var step3 = checkoutService.calculateStartStep(cart, isAuthenticated);

                    ///Assert
                    expect(step1).toEqual(CheckoutStepNumbers.Shipping);
                    expect(step2).toEqual(CheckoutStepNumbers.Shipping);
                    expect(step3).toEqual(CheckoutStepNumbers.Shipping);
                });

                it('SHOULD return Billing step when ShippingAddress is fulfilled for Shipping method type', () => {
                    //Arrange
                    let cart = {
                        Customer: {
                            FirstName: 'Stepan',
                            LastName: 'Bour',
                            Email: 'email@domain.com',
                        },
                        ShippingMethod: {
                            FulfillmentMethodTypeString: Orckestra.Composer.FulfillmentMethodTypes.Shipping
                        },
                        ShippingAddress: {
                            AddressBookId: '123456',
                            Line1: 'Line1',
                            City: 'City',
                            RegionCode: 'RegionCode',
                            PostalCode: 'PostalCode',
                            PhoneNumber: 'PhoneNumber'
                        }
                    }
                    ///Act
                    var step = checkoutService.calculateStartStep(cart, isAuthenticated);

                    ///Assert
                    expect(step).toEqual(CheckoutStepNumbers.Billing);
                });

                it('SHOULD return Shipping step when PickUpLocationId is not set for PickUp method type', () => {
                    //Arrange
                    let cart = {
                        Customer: {
                            FirstName: 'Stepan',
                            LastName: 'Bour',
                            Email: 'email@domain.com',
                        },
                        ShippingMethod: {
                            FulfillmentMethodTypeString: Orckestra.Composer.FulfillmentMethodTypes.PickUp
                        },
                        ShippingAddress: {
                            AddressName: 'Some',
                            Line1: 'Line1',
                            City: 'City',
                            RegionCode: 'RegionCode',
                            PostalCode: 'PostalCode'
                        }
                    }
                    ///Act
                    var step = checkoutService.calculateStartStep(cart, isAuthenticated);

                    ///Assert
                    expect(step).toEqual(CheckoutStepNumbers.Shipping);
                });

                it('SHOULD return Billing step when PickUpLocationId is set for PickUp method type', () => {
                    //Arrange
                    let cart = {
                        Customer: {
                            FirstName: 'Stepan',
                            LastName: 'Bour',
                            Email: 'email@domain.com',
                        },
                        ShippingMethod: {
                            FulfillmentMethodTypeString: Orckestra.Composer.FulfillmentMethodTypes.PickUp
                        },
                        PickUpLocationId: '123',
                        ShippingAddress: {
                            AddressName: 'Store 1',
                            Line1: 'Line1',
                            City: 'City',
                            RegionCode: 'RegionCode',
                            PostalCode: 'PostalCode'

                        }
                    }
                    ///Act
                    var step = checkoutService.calculateStartStep(cart, isAuthenticated);

                    ///Assert
                    expect(step).toEqual(CheckoutStepNumbers.Billing);
                });
            })
        });
    });

})();
