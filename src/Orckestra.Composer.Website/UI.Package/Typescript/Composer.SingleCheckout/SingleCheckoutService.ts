///<reference path='../../Typings/tsd.d.ts' />
///<reference path='../../Typings/vue/index.d.ts' />
///<reference path='../Composer.MyAccount/Common/MembershipService.ts' />
///<reference path='../ErrorHandling/ErrorHandler.ts' />
///<reference path='../Repositories/CartRepository.ts' />
///<reference path='../Composer.Cart/CheckoutShippingMethod/ShippingMethodService.ts' />
///<reference path='../Composer.Cart/CartSummary/CartService.ts' />
///<reference path='./IBaseSingleCheckoutController.ts' />
///<reference path='../Composer.Cart/CheckoutCommon/RegionService.ts' />
///<reference path='../Composer.Cart/CheckoutCommon/ICheckoutService.ts' />
///<reference path='../Composer.Cart/CheckoutCommon/ICheckoutContext.ts' />
///<reference path='../Composer.Cart/CheckoutCommon/IRegisterOptions.ts' />
///<reference path='./ISingleCheckoutService.ts' />
///<reference path='./ISingleCheckoutContext.ts' />

module Orckestra.Composer {
    'use strict';

    export class SingleCheckoutService implements ISingleCheckoutService {

        private static instance: ISingleCheckoutService;

        public VueCheckout: Vue;
        public VueCheckoutMixins: any = [];

        public static checkoutStep: number;

        private orderConfirmationCacheKey = 'orderConfirmationCacheKey';
        private orderCacheKey = 'orderCacheKey';

        private window: Window;
        private readonly eventHub: IEventHub;
        private registeredControllers: any = {};
        private allControllersReady: Q.Deferred<boolean>;
        private cacheProvider: ICacheProvider;

        protected cartService: ICartService;
        protected membershipService: IMembershipService;
        protected regionService: IRegionService;
        protected shippingMethodService: ShippingMethodService;

        public static getInstance(): ISingleCheckoutService {

            if (!SingleCheckoutService.instance) {
                SingleCheckoutService.instance = new SingleCheckoutService();
            }

            return SingleCheckoutService.instance;
        }

        public constructor() {

            if (SingleCheckoutService.instance) {
                throw new Error('Instantiation failed: Use SingleCheckoutService.instance() instead of new.');
            }

            this.eventHub = EventHub.instance();
            this.window = window;
            this.allControllersReady = Q.defer<boolean>();
            this.cacheProvider = CacheProvider.instance();

            this.cartService = new CartService(new CartRepository(), this.eventHub);
            this.membershipService = new MembershipService(new MembershipRepository());
            this.regionService = new RegionService();
            this.shippingMethodService = new ShippingMethodService();
            this.registerAllControllersInitialized();

            SingleCheckoutService.instance = this;
        }

        protected registerAllControllersInitialized(): void {

            this.eventHub.subscribe('allControllersInitialized', () => {
                this.initialize();
            });
        }

        private initialize() {

            let authenticatedPromise = this.membershipService.isAuthenticated();
            let getCartPromise = this.getCart();
            let regionsPromise: Q.Promise<any> = this.regionService.getRegions();
            let shippingMethodTypesPromise: Q.Promise<any> = this.shippingMethodService.getShippingMethodTypes();

            Q.all([authenticatedPromise, getCartPromise, regionsPromise, shippingMethodTypesPromise])
                .spread((authVm, cartVm, regionsVm, shippingMethodTypesVm) => {

                    if (!cartVm.Customer) {
                        cartVm.Customer = {};
                    }

                    let results: ISingleCheckoutContext = {
                        IsAuthenticated: authVm,
                        Cart: cartVm,
                        Regions: regionsVm,
                        ShippingMethodTypes: shippingMethodTypesVm.ShippingMethodTypes,
                        StartStep: this.calculateStartStep(cartVm),
                        IsLoading: false
                    };

                    this.initializeVueComponent(results);
                })
                .then(() => {
                    this.allControllersReady.resolve(true);
                })
                .fail((reason: any) => {
                    console.error('Error while initializing SingleCheckoutService.', reason);
                    ErrorHandler.instance().outputErrorFromCode('CheckoutRenderFailed');
                });
        }

        public calculateStartStep(cart: any): number {
            if (!(cart.Customer.FirstName &&
                cart.Customer.LastName &&
                cart.Customer.Email)) {
                return 0; // Inforamtion
            } else {
                if(!(cart.ShippingMethod &&
                    cart.ShippingAddress.Line1 &&
                    cart.ShippingAddress.City &&
                    cart.ShippingAddress.RegionCode &&
                    cart.ShippingAddress.PostalCode)) {
                    return 1; // Shipping
                } else {
                    return 2; // Review Cart
                }
            }
        }

        public initializeVueComponent(checkoutContext: ISingleCheckoutContext) {
            this.VueCheckout = new Vue({
                el: '#vueSingleCheckout',
                data: checkoutContext,
                mixins: this.VueCheckoutMixins,
                components: {
                    'checkout-step': (<any>window).httpVueLoader('/UI.Package/Vue/CheckoutStep.vue'),
                    'single-page-checkout': (<any>window).httpVueLoader('/UI.Package/Vue/Checkout.vue'),
                },
                mounted() {

                },
                computed: {
                    Customer() {
                        return this.Cart.Customer;
                    },
                    ShippingAddress() {
                        return this.Cart.ShippingAddress;
                    },
                    Rewards() {
                        return this.Cart.Rewards;
                    },
                    OrderSummary() {
                        return this.Cart.OrderSummary;
                    },
                    CartEmpty() {
                        return this.Cart.LineItemDetailViewModels.length == 0;
                    },
                    OrderCanBePlaced() {
                        return false;
                    }
                },
                methods: {

                }
            });
        }

        public registerController(controller: IBaseSingleCheckoutController) {

            if (this.allControllersReady.promise.isPending()) {
                this.allControllersReady.resolve(false);
            }

            this.allControllersReady.promise
                .then(allControllersReady => {

                    if (allControllersReady) {
                        throw new Error('Too late to register all controllers are ready.');
                    } else {
                        var controllerName = controller.viewModelName;
                        this.registeredControllers[controllerName] = controller;
                    }
                });
        }

        public unregisterController(controllerName: string) {

            delete this.registeredControllers[controllerName];
        }


        public updatePostalCode(postalCode: string): Q.Promise<void> {

            return this.cartService.updateBillingMethodPostalCode(postalCode);
        }

        public invalidateCache(): Q.Promise<void> {

            return this.cartService.invalidateCache();
        }

        public getCart(): Q.Promise<any> {

            return this.invalidateCache()
                .then(() => this.cartService.getCart())
                .fail(reason => {
                    this.handleError(reason);
                });
        }

        public removeCartItem(id: any, productId: any): Q.Promise<any> {

            return this.invalidateCache().
                then(() => this.cartService.deleteLineItem(id, productId))
                .fail(reason => {
                    this.handleError(reason);
                });
        }

        public updateCartItem(id: string, quantity: number, productId: string,
            recurringOrderFrequencyName?: string,
            recurringOrderProgramName?: string): Q.Promise<any> {

            return this.invalidateCache().
                then(() => this.cartService.updateLineItem(id, quantity, productId, recurringOrderFrequencyName, recurringOrderProgramName))
                .fail(reason => {
                    this.handleError(reason);
                });
        }

        public updateCart(controllerName: string = null): Q.Promise<IUpdateCartResult> {

            var emptyVm = {
                UpdatedCart: {}
            };

            return this.buildCartUpdateViewModel(emptyVm, controllerName)
                .then(vm => {
                    return this.cartService.updateCart(vm);
                });
        }

        public completeCheckout(): Q.Promise<ICompleteCheckoutResult> {

            let emptyVm = {
                UpdatedCart: {}
            };

            return this.buildCartUpdateViewModel(emptyVm)
                .then(vm => {

                    if (_.isEmpty(vm.UpdatedCart)) {
                        console.log('No modification required to the cart.');
                        return vm;
                    }

                    return this.cartService.updateCart(vm);
                })
                .then(result => {

                    if (result && result.HasErrors) {
                        throw new Error('Error while updating the cart. Complete Checkout will not complete.');
                    }

                    console.log('Publishing the cart!');

                    return this.cartService.completeCheckout(SingleCheckoutService.checkoutStep);
                });
        }

        private buildCartUpdateViewModel(vm: any, controllerName: string = null): Q.Promise<any> {

            //var validationPromise: Q.Promise<any>;
            //var viewModelUpdatePromise: Q.Promise<any>;

            // validationPromise = Q(vm).then(vm => {
            //     return this.getCartValidation(vm);
            // });

            //  viewModelUpdatePromise = validationPromise.then(vm => {
            return this.getCartUpdateViewModel(vm, controllerName);
            // });

            ///return viewModelUpdatePromise;
        }

        private getCartValidation(vm: any): Q.Promise<any> {

            let validationPromise = this.collectValidationPromises();

            return validationPromise.then((results: Array<boolean>) => {
                console.log('Aggregating all validation results');
                let hasFailedValidation = _.any(results, r => !r);

                if (hasFailedValidation) {
                    throw new Error('There were validation errors.');
                }

                return vm;
            });
        }

        private getCartUpdateViewModel(vm: any, controllerName: string = null): Q.Promise<any> {

            let updateModelPromise = this.collectUpdateModelPromises(controllerName);

            return updateModelPromise.then((updates: Array<any>) => {
                console.log('Aggregating all ViewModel updates.');

                _.each(updates, update => {

                    if (update) {
                        var keys = _.keys(update);
                        _.each(keys, key => {
                            vm.UpdatedCart[key] = update[key];
                        });
                    }
                });

                return vm;
            });
        }

        private collectValidationPromises(): Q.Promise<any> {

            let promises: Q.Promise<any>[] = [];
            let controllerInstance: IBaseSingleCheckoutController;

            for (let controllerName in this.registeredControllers) {

                if (this.registeredControllers.hasOwnProperty(controllerName)) {
                    controllerInstance = <IBaseSingleCheckoutController>this.registeredControllers[controllerName];
                    promises.push(controllerInstance.getValidationPromise());
                }
            }

            return Q.all(promises);
        }

        private collectUpdateModelPromises(controllerName: string = null): Q.Promise<any> {

            let promises: Q.Promise<any>[] = [];
            let controllerInstance: IBaseSingleCheckoutController;

            if (controllerName) {
                if (this.registeredControllers.hasOwnProperty(controllerName)) {
                    controllerInstance = <IBaseSingleCheckoutController>this.registeredControllers[controllerName];
                    promises.push(controllerInstance.getUpdateModelPromise());
                }
            } else {
                for (let controllerName in this.registeredControllers) {

                    if (this.registeredControllers.hasOwnProperty(controllerName)) {
                        controllerInstance = <IBaseSingleCheckoutController>this.registeredControllers[controllerName];
                        promises.push(controllerInstance.getUpdateModelPromise());
                    }
                }
            }

            return Q.all(promises);
        }

        private handleError(reason: any): void {

            console.error('Unable to retrieve the cart for the checkout', reason);

            throw reason;
        }

        public setOrderConfirmationToCache(orderConfirmationViewModel: any): void {

            this.cacheProvider.defaultCache.set(this.orderConfirmationCacheKey, orderConfirmationViewModel).done();
        }

        public getOrderConfirmationFromCache(): Q.Promise<any> {

            return this.cacheProvider.defaultCache.get<any>(this.orderConfirmationCacheKey);
        }

        public clearOrderConfirmationFromCache(): void {

            this.cacheProvider.defaultCache.clear(this.orderConfirmationCacheKey).done();
        }

        public setOrderToCache(orderConfirmationViewModel: any): void {

            this.cacheProvider.defaultCache.set(this.orderCacheKey, orderConfirmationViewModel).done();
        }
    }
}
