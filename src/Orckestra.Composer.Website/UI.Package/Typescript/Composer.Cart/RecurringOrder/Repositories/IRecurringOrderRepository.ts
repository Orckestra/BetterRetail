///<reference path='../../../../Typings/tsd.d.ts' />
///<reference path='../IRecurringOrderParameters.ts' />
///<reference path='../ViewModels/IRecurringOrderViewModel.ts' />


namespace Orckestra.Composer {
    export interface IRecurringOrderRepository {
        updateLineItemsDate(updateLineItemParam: IRecurringOrderLineItemsUpdateDateParam) : Promise<any>;
        deleteLineItem(deleteLineItemParam: IRecurringOrderLineItemDeleteParam) : Promise<any>;
        deleteLineItems(deleteLineItemsParam: IRecurringOrderLineItemsDeleteParam) : Promise<any>;
        getCustomerAddresses(): Promise<any>;
        getCustomerPaymentMethods(): Promise<any>;
        updateCartShippingAddress(updateCartAddressParam: IRecurringOrderUpdateCartAddressParam): Promise<any>;
        updateCartBillingAddress(updateTemplateAddressParam: IRecurringOrderUpdateTemplateAddressParam): Promise<any>;
        updateTemplatePaymentMethod(updateTemplatePaymentMethodParam: IRecurringOrderUpdateTemplatePaymentMethodParam): Promise<any>;
        updateLineItemQuantity(updateLineItemQuantityParam: IRecurringOrderUpdateLineItemQuantityParam) : Promise<any>;
        getRecurringOrderCartsByUser(): Promise<any>;
        getRecurringOrderTemplatesByUser(): Promise<any>;
        updateTemplateLineItemQuantity(updateLineItemQuantityParam: IRecurringOrderTemplateUpdateLineItemQuantityParam) : Promise<any>;
        getRecurringOrderProgramsByUser(): Promise<any>;
        getRecurringOrderProgramsByNames(programsByNamesParam : IRecurringOrderProgramsByNamesParam): Promise<any>;
        updateTemplateLineItem(templateLineItemUpdateParam: IRecurringOrderTemplateLineItemUpdateParam): Promise<any>;
        deleteTemplateLineItem(deleteTemplateLineItemParam: IRecurringOrderTemplateLineItemDeleteParam): Promise<any>;
        deleteTemplateLineItems(deleteTemplateLineItemsParam: IRecurringOrderTemplateLineItemsDeleteParam): Promise<any>;
        getCartContainsRecurrence(): Promise<any>;
        getRecurrenceConfigIsActive(): Promise<any>;
        getCanRemovePaymentMethod(paymentMethodId: string): Promise<any>;
        getRecurringOrderCartSummaries(): Promise<any>;
        addRecurringOrderCartLineItem(addRecurringOrderCartLineItemParam: IAddRecurringOrderCartLineItemParam): Promise<any>;
        getAnonymousCartSignInUrl(): Promise<any>;
        updateCartShippingMethod(updateCartShippingMethodParam: IRecurringOrderCartUpdateShippingMethodParam): Promise<any>;
        getCartShippingMethods(getCartShippingMethodsParam: IRecurringOrderGetCartShippingMethods): Promise<any>;
        getOrderTemplateShippingMethods(): Promise<any>;
        getInactifProductsFromCustomer(): Promise<any>;
        clearCustomerInactifItems(): Promise<any>;
        getRecurringCart(getRecurringCartParam: IRecurringOrderCartParam): Promise<any>;
        getCartPaymentMethods(getCartPaymentMethodsParam: IRecurringOrderGetCartPaymentMethods): Promise<any>;
        updateCartPaymentMethod(updateCartPaymentMethodParam: IRecurringOrderCartUpdatePaymentMethodParam): Promise<any>;
        getRecurringTemplateDetail(recurringOrderTemplateId: string): Promise<any>;
        getTemplatePaymentMethods(getTemplatePaymentMethodsParam: IRecurringOrderGetTemplatePaymentMethods): Promise<any>;
    }
}
