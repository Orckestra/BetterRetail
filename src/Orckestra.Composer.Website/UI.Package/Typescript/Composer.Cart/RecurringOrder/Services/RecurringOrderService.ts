///<reference path='../../../../Typings/tsd.d.ts' />
///<reference path='../../../Events/IEventHub.ts' />
///<reference path='./IRecurringOrderService.ts' />

namespace Orckestra.Composer {
    'use strict';

    export class RecurringOrderService implements IRecurringOrderService {
        protected repository: IRecurringOrderRepository;
        protected eventHub: Orckestra.Composer.IEventHub;

        public constructor(repository: IRecurringOrderRepository, eventHub: Orckestra.Composer.IEventHub) {
            if (!eventHub) {
                throw new Error('Error: eventHub is required');
            }
            this.repository = repository;
            this.eventHub = eventHub;
        }

        public updateLineItemsDate(updateLineItemsParam: IRecurringOrderLineItemsUpdateDateParam): Promise<any> {
            return this.repository.updateLineItemsDate(updateLineItemsParam);
        }

        public deleteLineItem(deleteLineItemParam: IRecurringOrderLineItemDeleteParam): Promise<any> {
            return this.repository.deleteLineItem(deleteLineItemParam);
        }

        public deleteLineItems(deleteLineItemsParam: IRecurringOrderLineItemsDeleteParam): Promise<any> {
            return this.repository.deleteLineItems(deleteLineItemsParam);
        }

        public getCustomerAddresses(): Promise<any> {
            return this.repository.getCustomerAddresses();
        }

        public getCustomerPaymentMethods(): Promise<any> {
            return this.repository.getCustomerPaymentMethods();
        }

        public updateCartShippingAddress(updateCartAddressParam: IRecurringOrderUpdateCartAddressParam): Promise<any> {
            return this.repository.updateCartShippingAddress(updateCartAddressParam);
        }

        public updateCartBillingAddress(updateTemplateAddressParam: IRecurringOrderUpdateTemplateAddressParam): Promise<any> {
            return this.repository.updateCartBillingAddress(updateTemplateAddressParam);
        }


        public updateTemplatePaymentMethod(updateTemplatePaymentMethodParam: IRecurringOrderUpdateTemplatePaymentMethodParam): Promise<any> {
            return this.repository.updateTemplatePaymentMethod(updateTemplatePaymentMethodParam);
        }

        public updateLineItemQuantity(updateLineItemQuantityParam: IRecurringOrderUpdateLineItemQuantityParam): Promise<any> {
            return this.repository.updateLineItemQuantity(updateLineItemQuantityParam);
        }

        public getRecurringOrderCartsByUser() {
            return this.repository.getRecurringOrderCartsByUser();
        }

        public getRecurringOrderTemplatesByUser() {
            return this.repository.getRecurringOrderTemplatesByUser();
        }

        public updateTemplateLineItemQuantity(updateLineItemQuantityParam: IRecurringOrderTemplateUpdateLineItemQuantityParam): Promise<any> {
            return this.repository.updateTemplateLineItemQuantity(updateLineItemQuantityParam);
        }

        public getRecurringOrderProgramsByUser(): Promise<any> {
            return this.repository.getRecurringOrderProgramsByUser();
        }

        public getRecurringOrderProgramsByNames(programsByNamesParam: IRecurringOrderProgramsByNamesParam): Promise<any> {
            return this.repository.getRecurringOrderProgramsByNames(programsByNamesParam);
        }

        public updateTemplateLineItem(templateLineItemUpdateParam: IRecurringOrderTemplateLineItemUpdateParam): Promise<any> {
            return this.repository.updateTemplateLineItem(templateLineItemUpdateParam);
        }

        public deleteTemplateLineItem(deleteTemplateLineItemParam: IRecurringOrderTemplateLineItemDeleteParam): Promise<any> {
            return this.repository.deleteTemplateLineItem(deleteTemplateLineItemParam);
        }

        public deleteTemplateLineItems(deleteTemplateLineItemsParam: IRecurringOrderTemplateLineItemsDeleteParam): Promise<any> {
            return this.repository.deleteTemplateLineItems(deleteTemplateLineItemsParam);
        }

        public getCartContainsRecurrence(): Promise<any> {
            return this.repository.getCartContainsRecurrence();
        }

        public getRecurrenceConfigIsActive(): Promise<any> {
            return this.repository.getRecurrenceConfigIsActive();
        }

        public getCanRemovePaymentMethod(paymentMethodId: string): Promise<any> {
            return this.repository.getCanRemovePaymentMethod(paymentMethodId);
        }

        public getRecurringOrderCartSummaries(): Promise<any> {
            return this.repository.getRecurringOrderCartSummaries();
        }

        public addRecurringOrderCartLineItem(addLineItemQuantityParam: IAddRecurringOrderCartLineItemParam): Promise<any> {
            return this.repository.addRecurringOrderCartLineItem(addLineItemQuantityParam);
        }

        public getAnonymousCartSignInUrl(): Promise<any> {
            return this.repository.getAnonymousCartSignInUrl();
        }

        public updateCartShippingMethod(updateCartShippingMethodParam: IRecurringOrderCartUpdateShippingMethodParam): Promise<any> {
            return this.repository.updateCartShippingMethod(updateCartShippingMethodParam);
        }

        public getCartShippingMethods(getCartShippingMethodsParam: IRecurringOrderGetCartShippingMethods): Promise<any> {
            return this.repository.getCartShippingMethods(getCartShippingMethodsParam);
        }

        public getOrderTemplateShippingMethods(): Promise<any> {
            return this.repository.getOrderTemplateShippingMethods();
        }

        public getInactifProductsFromCustomer(): Promise<any> {
            return this.repository.getInactifProductsFromCustomer();
        }

        public clearCustomerInactifItems(): Promise<any> {
            return this.repository.clearCustomerInactifItems();
        }

        public getRecurringCart(getRecurringCartParam: IRecurringOrderCartParam): Promise<any> {
            return this.repository.getRecurringCart(getRecurringCartParam);
        }

        public getCartPaymentMethods(getCartPaymentMethodsParam: IRecurringOrderGetCartPaymentMethods): Promise<any> {
            return this.repository.getCartPaymentMethods(getCartPaymentMethodsParam);
        }

        public updateCartPaymentMethod(updateCartPaymentMethodParam: IRecurringOrderCartUpdatePaymentMethodParam): Promise<any> {
            return this.repository.updateCartPaymentMethod(updateCartPaymentMethodParam);
        }

        public getRecurringTemplateDetail(recurringOrderTemplateId: string): Promise<any> {
            return this.repository.getRecurringTemplateDetail(recurringOrderTemplateId);
        }

        public getTemplatePaymentMethods(getTemplatePaymentMethodsParam: IRecurringOrderGetTemplatePaymentMethods): Promise<any> {
            return this.repository.getTemplatePaymentMethods(getTemplatePaymentMethodsParam);
        }

        private _mapLineItemToRequest(lineItem: any): any {
            return {
                CategoryId: lineItem.ProductSummary.CategoryId,
                ProductInfo: {
                    ProductId: lineItem.ProductId,
                    Sku: lineItem.Sku,
                    VariantId: lineItem.VariantId
                }
            };
        }

        private _mapRecurringLineItemToRequest(lineItem: any): any {
            return lineItem.SelectedRecurringOrderFrequencyId;
        }
    }
}
