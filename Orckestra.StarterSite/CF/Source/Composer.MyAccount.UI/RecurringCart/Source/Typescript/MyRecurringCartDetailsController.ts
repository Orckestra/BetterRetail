///<reference path='../../../../Composer.UI/Source/Typings/tsd.d.ts' />
///<reference path='../../../../Composer.UI/Source/TypeScript/Mvc/Controller.ts' />
///<reference path='../../../../Composer.UI/Source/TypeScript/Mvc/IControllerActionContext.ts' />
///<reference path='./RecurringCartDetailsController.ts' />
///<reference path='../../../../Composer.Cart.UI/RecurringOrder/source/TypeScript/Services/RecurringOrderService.ts' />
///<reference path='../../../../Composer.Cart.UI/RecurringOrder/source/TypeScript/Repositories/RecurringOrderRepository.ts' />
///<reference path='./RecurringCartAddressRegisteredService.ts' />


module Orckestra.Composer {

    enum EditSection {
        NextOccurence = 0,
        ShippingMethod = 1,
        Address = 2,
        Payment = 3
    };

    export class MyRecurringCartDetailsController extends Orckestra.Composer.RecurringCartDetailsController {
        private recurringOrderService: IRecurringOrderService = new RecurringOrderService(new RecurringOrderRepository(), this.eventHub);
        private editNextOcurrence = false;
        private editShippingMethod = false;
        private editAddress = false;
        private editPayment = false;
        private originalShippingMethodType = '';
        private hasShippingMethodTypeChanged = false ;
        private viewModelName = '';
        private viewModel;
        private updateQtyTimer;
        private updateWaitTime = 300;

        private debounceUpdateLineItem: (args: any) => void;

        protected customerService: ICustomerService = new CustomerService(new CustomerRepository());
        protected recurringCartAddressRegisteredService: RecurringCartAddressRegisteredService =
            new RecurringCartAddressRegisteredService(this.customerService);

        public initialize() {

            super.initialize();

            this.viewModelName = 'MyRecurringCartDetails';

            console.log(this.context.viewModel);

            //TODO : render page after get cart
            this.getRecurringCart();
        }

        public getRecurringCart() {

            var data = {
                cartName: this.context.viewModel.Name
            };

            this.recurringOrderService.getRecurringCart(data)
                .then(result => {
                    console.log(result);
                    this.viewModel = result;
                })
                .fail((reason) => {
                    console.error(reason);
                });
        }

        public toggleEditNextOccurence(actionContext: IControllerActionContext) {
            var context: JQuery = actionContext.elementContext;

            this.editNextOcurrence = !this.editNextOcurrence;

            if (this.editNextOcurrence) {
                this.closeOtherEditSections(actionContext, EditSection.NextOccurence);
            }

            let nextOccurence = context.data('next-occurence');
            let formatedNextOccurence = context.data('formated-next-occurence');
            let nextOccurenceValue = context.data('next-occurence-value');
            let total = context.data('total');

            let vm = {
                EditMode: this.editNextOcurrence,
                NextOccurence: nextOccurence,
                FormatedNextOccurence: formatedNextOccurence,
                NextOccurenceValue: nextOccurenceValue,
                OrderSummary: {
                    Total: total
                }
            };

            this.render('RecurringCartDetailsSummary', vm);
        }

        public saveEditNextOccurence(actionContext: IControllerActionContext) {
            var context: JQuery = actionContext.elementContext;

            let element = <HTMLInputElement>$('#NextOcurrence')[0];
            let newDate = element.value;
            let isValid = this.nextOcurrenceIsValid(newDate);

            if (isValid) {
                let cartName = this.context.viewModel.Name;
                let data: IRecurringOrderLineItemsUpdateDateParam = {
                    CartName: cartName,
                    NextOccurence: newDate
                };

                this.recurringOrderService.updateLineItemsDate(data)
                    .then((viewModel) => {

                        let hasMerged = viewModel.RescheduledCartHasMerged;

                        if (hasMerged) {
                            //Redirect to my orders
                            //TODO
                        } else if (!_.isEmpty(viewModel)) {
                            //Render TODO
                            //this.render('MyRecurringCarts', viewModel);
                        }

                        //busyHandle.done();
                    })
                    .fail((reason) => {
                        console.error(reason);
                    //  busyHandle.done();
                    });
            } else {
                console.log('Error: invalid date');
            }
        }

        public nextOcurrenceIsValid(value) {
            let newDate = this.convertDateToUTC(new Date(value));
            let today = this.convertDateToUTC(new Date(new Date().setHours(0, 0, 0, 0)));

            if (newDate > today) {
                return true;
            }
            return false;
        }

        private convertDateToUTC(date) {
            return new Date(date.getUTCFullYear(), date.getUTCMonth(), date.getUTCDate(),
            date.getUTCHours(), date.getUTCMinutes(), date.getUTCSeconds());
        }

        public toggleEditShippingMethod (actionContext: IControllerActionContext) {
            var context: JQuery = actionContext.elementContext;

            this.editShippingMethod = !this.editShippingMethod;

            let shippingMethodDisplayName = context.data('shipping-method-display-name');
            let shippingMethodCost = context.data('shipping-method-cost');
            let shippingMethodName = context.data('selected-shipping-method-name');
            let shippingMethodFulfillmentType = context.data('selected-shipping-method-fulfillment-type');

            //TODO
            this.originalShippingMethodType = shippingMethodFulfillmentType;

            if (this.editShippingMethod) {
                this.closeOtherEditSections(actionContext, EditSection.ShippingMethod);

                this.getShippingMethods(this.context.viewModel.Name)
                    .then(shippingMethods => {

                        if (!shippingMethods) {
                            throw new Error('No viewModel received');
                        }

                        if (_.isEmpty(shippingMethods.ShippingMethods)) {
                            throw new Error('No shipping method was found.');
                        }

                        let selectedShippingMethodName = shippingMethodName;
                        shippingMethods.ShippingMethods.forEach(shippingMethod => {

                            if (shippingMethod.Name === selectedShippingMethodName) {
                                shippingMethods.SelectedShippingProviderId = shippingMethod.ShippingProviderId;
                            }
                        });

                        var vm = {
                            EditMode: this.editShippingMethod,
                            ShippingMethods: shippingMethods,
                            SelectedMethod: selectedShippingMethodName,
                            ShippingMethod: {
                                DisplayName: shippingMethodDisplayName,
                                Cost: shippingMethodCost,
                                Name: shippingMethodName,
                                FulfillmentMethodTypeString: shippingMethodFulfillmentType
                            }
                        };

                        this.render('RecurringCartDetailsShippingMethod', vm);
                    });
            } else {
                var vm = {
                    EditMode: this.editShippingMethod,
                    ShippingMethod: {
                        DisplayName: shippingMethodDisplayName,
                        Cost: shippingMethodCost,
                        Name: shippingMethodName,
                        FulfillmentMethodTypeString: shippingMethodFulfillmentType
                    }
                };
                this.render('RecurringCartDetailsShippingMethod', vm);
            }
        }

        private closeOtherEditSections(actionContext: IControllerActionContext, type: EditSection) {

            if (this.editNextOcurrence && type !== EditSection.NextOccurence) {
                this.toggleEditNextOccurence(actionContext);
            }
            if (this.editShippingMethod && type !== EditSection.ShippingMethod) {
                this.toggleEditShippingMethod(actionContext);
            }
        }

        public getShippingMethods(cartName) : Q.Promise<any> {
            let param: IRecurringOrderGetCartShippingMethods = {
                CartName: cartName
            };
            return this.recurringOrderService.getCartShippingMethods(param)
                    .fail((reason) => {
                        console.error('Error while retrieving shipping methods', reason);
                    });
        }

        public saveEditShippingMethod(actionContext: IControllerActionContext) {
            var context: JQuery = actionContext.elementContext;

            //var newType = context.data('fulfillment-method-type');
            let element = $('#ShippingMethod').find('input[name=ShippingMethod]:checked')[0];

            var newType = element.dataset['fulfillmentMethodType'];


            this.manageSaveShippingMethod(newType);
        }

        public methodSelected(actionContext: IControllerActionContext) {
            var shippingProviderId = actionContext.elementContext.data('shipping-provider-id');
             $('#ShippingProviderId').val(shippingProviderId.toString());
        }

        private manageSaveShippingMethod(newType) {
            //When shipping method is changed from ship to store and ship to home, address must correspond to 
            //store adress/home address.
            //When the type change, we wait to save shipping method and open adresse section. Then, when saving valid address,
            //also save the shipping method.
            //When cancel in one of the two steps, revert to original values.
            //If saving shipping method and the method type doesn't change, save immediatly.


            this.hasShippingMethodTypeChanged = this.originalShippingMethodType !== newType;

            if (this.hasShippingMethodTypeChanged) {
//TODO
            } else {
                //Do the save

                let shippingProviderId = $('#ShippingProviderId').val();

                let element = $('#ShippingMethod').find('input[name=ShippingMethod]:checked');
                let shippingMethodName = element.val();

                //var busy = this.asyncBusy({ elementContext: actionContext.elementContext });

                let cartName = this.context.viewModel.Name;

                let data: IRecurringOrderCartUpdateShippingMethodParam = {
                    shippingProviderId: shippingProviderId,
                    shippingMethodName: shippingMethodName,
                    cartName: cartName
                };

                this.recurringOrderService.updateCartShippingMethod(data)
                    .then(result => {

                        this.reRenderCartPage(result);
                    })
                    .fail((reason) => {
                        console.error(reason);
                    });
                    //.fin(() => busy.done());
            }
        }

        private reRenderCartPage(vm) {
            this.viewModel = vm;
            this.render(this.viewModelName, vm);
        }

        public toggleEditAddress (actionContext: IControllerActionContext) {
            var context: JQuery = actionContext.elementContext;

            this.editAddress = !this.editAddress;

            if (this.editAddress) {
                this.closeOtherEditSections(actionContext, EditSection.Address);

                this.recurringCartAddressRegisteredService.getRecurringCartAddresses(this.viewModel)
                    .then((addressesVm) => {

                        console.log(addressesVm);
                        addressesVm.EditMode = this.editAddress;
                        addressesVm.Payment = {
                            BillingAddress: {
                                UseShippingAddress: this.viewModel.Payment.BillingAddress.UseShippingAddress
                            }
                        };

                        this.render('RecurringCartDetailsAddress', addressesVm);
                    });
                    //.fail(reason => this.handleError(reason))
                    //fin(() => this.eventHub.publish(`${this.viewModelName}Rendered`, checkoutContext.cartViewModel));
            } else {
                this.render('RecurringCartDetailsAddress', this.viewModel);
            }
        }

        public changeShippingAddress(actionContext: IControllerActionContext) {
            //maybe dont need
        }

        public saveEditAddress (actionContext: IControllerActionContext) {
            //If shipping method type has changed, save address and shipping method

            var context: JQuery = actionContext.elementContext;

            let shippingAddressId = $(this.context.container).find('input[name=ShippingAddressId]:checked').val();
            let billingAddressId = $(this.context.container).find('input[name=BillingAddressId]:checked').val();
            let useSameForShippingAndBilling = $(this.context.container).find('input[name=UseShippingAddress]:checked').val();
            let cartName = this.context.viewModel.Name;

            let data: IRecurringOrderUpdateTemplateAddressParam = {
                shippingAddressId: shippingAddressId,
                billingAddressId: null,
                cartName: cartName,
                useSameForShippingAndBilling: useSameForShippingAndBilling
            };

            if (useSameForShippingAndBilling) {
                data.billingAddressId = billingAddressId;
            }

            this.recurringOrderService.updateCartShippingAddress(data)
                .then(result => {

                    console.log(result);

                    this.viewModel = result;

                    this.reRenderCartPage(result);
                })
                .fail((reason) => {
                    console.error(reason);
                });
                //.fin(() => busy.done());
        }

        private useShippingAddress() : Boolean {
            var useShippingAddress = $(this.context.container).find('input[name=UseShippingAddress]:checked').val() === 'true';
            return useShippingAddress;
        }


        public changeUseShippingAddress() {

            this.setBillingAddressFormVisibility();
            this.setSelectedBillingAddress();
            //TODO: form validation?
        }

        private setBillingAddressFormVisibility() {

            var useShippingAddress: Boolean = this.useShippingAddress();
            if (useShippingAddress) {
                $('#BillingAddressContent').addClass('hide');
            } else {
                $('#BillingAddressContent').removeClass('hide');
            }
        }

        protected setSelectedBillingAddress() {

            var selectedBillingAddressId: string = $(this.context.container).find('input[name=BillingAddressId]:checked').val();

            if (!selectedBillingAddressId) {
                return;
            }
        }

        public toggleEditPayment (actionContext: IControllerActionContext) {
            var context: JQuery = actionContext.elementContext;

            this.editPayment = !this.editPayment;

            if (this.editPayment) {
                this.closeOtherEditSections(actionContext, EditSection.Payment);

                //TODO

                this.render('RecurringCartDetailsPayment', this.viewModel);
            } else {
                this.render('RecurringCartDetailsPayment', this.viewModel);
            }
        }

        public updateLineItem(actionContext: IControllerActionContext): void {

            if (!this.debounceUpdateLineItem) {
                this.debounceUpdateLineItem =
                    _.debounce((args) =>
                        this.applyUpdateLineItemQuantity(args), this.updateWaitTime);
            }

            let context: JQuery = actionContext.elementContext;
            let cartQuantityElement = actionContext.elementContext
                .parents('.cart-item')
                .find('.cart-quantity');

            let incrementButtonElement = actionContext.elementContext
                .parents('.cart-item')
                .find('.increment-quantity');

            let decrementButtonElement = actionContext.elementContext
                .parents('.cart-item')
                .find('.decrement-quantity');

            const action: string = <any>context.data('action');
            const currentQuantity: number = parseInt(cartQuantityElement.text(), 10);

            let frequencyName = context.data('recurringorderfrequencyname');
            let programName = context.data('recurringorderprogramname');

            const updatedQuantity = this.updateQuantity(action, currentQuantity);
            var quantity: number = parseInt(<any>context.data('quantity'), 10);

            updatedQuantity === 1 ? decrementButtonElement.attr('disabled', 'disabled') : decrementButtonElement.removeAttr('disabled');
            //updatedQuantity === 99 ? incrementButtonElement.attr('disabled', 'disabled') : incrementButtonElement.removeAttr('disabled');

            cartQuantityElement.text(updatedQuantity);
            let cartName = this.context.viewModel.Name;

            var args: any = {
                actionContext: actionContext,
                context: context,
                cartQuantityElement: cartQuantityElement,
                cartName: cartName,
                frequencyName: frequencyName,
                programName: programName
            };

            if (quantity !== updatedQuantity) {
                //use only debouced function when incrementing/decrementing quantity
                this.debounceUpdateLineItem(args);
            }
        }

        private applyUpdateLineItemQuantity(args: any) {

            var busy = this.asyncBusy({ elementContext: args.actionContext.elementContext });
            let actionElementSpan =  args.context.find('span.fa').not('.loading-indicator');

            const updateLineItemQuantityParam: IRecurringOrderUpdateLineItemQuantityParam = {
                lineItemId:  args.context.data('lineitemid'),
                quantity: Number( args.cartQuantityElement.text()),
                cartName:  args.cartName,
                recurringProgramName:  args.programName,
                recurringFrequencyName:  args.frequencyName
            };
            args.cartQuantityElement.parents('.cart-item').addClass('is-loading');
            actionElementSpan.hide();
            this.recurringOrderService.updateLineItemQuantity(updateLineItemQuantityParam)
                .then(result => {
                    args.cartQuantityElement.parents('.cart-item').removeClass('is-loading');
                    actionElementSpan.show();

                    this.reRenderCartPage(result);

                    //This is to reinitialize the popover       
                    //this.initializePopOver();
                })
                .fail((reason: any) => console.error('Error while updating line item quantity.', reason))
                .fin(() => busy.done());
        }

        public updateQuantity(action: string, quantity: number): number {
            if (!action) {
                return quantity;
            }

            switch (action.toUpperCase()) {
                case 'INCREMENT':
                    quantity++;
                    break;

                case 'DECREMENT':
                    quantity--;
                    if (quantity < 1) {
                        quantity = 1;
                    }
                    break;
            }

            return quantity;
        }

        public deleteLineItem(actionContext: IControllerActionContext) {
            var context: JQuery = actionContext.elementContext;
            var lineItemId: string = <any>context.data('lineitemid');
            var productId: string = context.attr('data-productid');

            context.closest('.cart-row').addClass('is-loading');
            var cartName = this.viewModel.Name;

            const deleteLineItemParam: IRecurringOrderLineItemDeleteParam = {
                lineItemId: lineItemId,
                cartName: cartName
            };

            var busy = this.asyncBusy({ elementContext: actionContext.elementContext });

            this.recurringOrderService.deleteLineItem(deleteLineItemParam)
                .then(result => {
                    this.reRenderCartPage(result);

                    //TODO: Manage if last item?
                    //Deleting the last recurring item will reschedule the cartName to the line item next occurence
                })
                .fail((reason: any) => this.onLineItemDeleteFailed(context, reason))
                .fin(() => busy.done());
        }

        protected onLineItemDeleteFailed(context: JQuery, reason: any): void {
            console.error('Error while deleting line item.', reason);
            context.closest('.cart-row').removeClass('is-loading');

            //ErrorHandler.instance().outputErrorFromCode('LineItemDeleteFailed');
        }
    }
}
